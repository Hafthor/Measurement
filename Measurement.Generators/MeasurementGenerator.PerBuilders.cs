using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using static Measurement.Generators.Utils;

namespace Measurement.Generators;

// `.Per` denominator-walk builders hung off numerator measurement types (input side).
public class MeasurementGenerator_PerBuilders {
    // Emits the compositional `.Per` denominator walk (input side). `.Per` on a numerator measurement
    // enters a running-value trie; each denominator step divides the running coherent value by one unit
    // of the chosen dimension, so ANY (non-affine) unit of a slot's dimension composes — not only the
    // ones literally spelled in a compound unit. A completed denominator type-sequence implicitly
    // converts to the measurement that names it. `.Squared`/`.Cubed` repeat the last unit; a further
    // `.Per` (or, for a differently typed factor, a bare unit word) appends another denominator.
    public static void EmitPerBuilders(SourceProductionContext ctx, ImmutableArray<MeasurementInfo> all) {
        string ns = ResolveNamespace(all);
        var (meta, typeOfUnit, unitWordType, unitOpts) = BuildPerCommon(all);
        var removable = BuildRemovable(all);

        (int, double) Div(string t) => CoherentDivisor(meta[t].DF, meta[t].G);
        // Amount-of-substance (Quantity) stores an Avogadro count, but molar result types (g/mol, …)
        // treat the mole as the amount unit, so normalize the Quantity dimension to moles.
        bool hasMole = meta.ContainsKey("Quantity");

        string CoherentVal(string t, string valExpr) =>
            t == "Quantity" && hasMole
                ? $"({valExpr} / (Quantity.FromMoles(1).CanonicalValue))"
                : ToCoherentExpr(valExpr, Div(t));

        string FromCoherentVal(string t, string expr) =>
            t == "Quantity" && hasMole
                ? $"({expr} * (Quantity.FromMoles(1).CanonicalValue))"
                : FromCoherentExpr(expr, Div(t));

        string CUnit(string d, string baseUnit) =>
            CoherentVal(d, $"({Cons(d, d, baseUnit, "1", removable)}.CanonicalValue)");

        // `.Square`/`.Cubic` denominator modifiers: an areal/cubic unit spelled as a length sub-walk
        // (`.Cubic.Centi.Meter`), mapped to the Area/Volume unit for its exact coherent value.
        var mod = BuildDenomModTrie(all, meta);

        // numeratorType → (denominator type-sequence → result measurements). Squared/Cubed expand the
        // preceding factor type; the numerator split takes the longest leading unit spelling.
        var seqMap = new Dictionary<string, Dictionary<string, List<string>>>();
        foreach (var info in all)
            foreach (var u in EnumerateUnitNames(info.Units)) {
                var ws = SplitWords(u);
                if (!ws.Contains("Per") || PerExcludeUnits.Contains(u)) continue;
                int per = -1;
                for (int k = ws.Count - 1; k >= 1; k--) {
                    if (ws[k] != "Per") continue;
                    if (typeOfUnit.ContainsKey(string.Concat(ws.GetRange(0, k)))) {
                        per = k;
                        break;
                    }
                }

                if (per < 0 || per == ws.Count - 1) continue;
                string n = typeOfUnit[string.Concat(ws.GetRange(0, per))];
                var types = new List<string>();
                string lastT = "";
                bool ok = true;
                for (int k = per + 1; k < ws.Count; k++) {
                    string w = ws[k];
                    if (w == "Squared") {
                        if (lastT.Length == 0) {
                            ok = false;
                            break;
                        }

                        types.Add(lastT);
                    } else if (w == "Cubed") {
                        if (lastT.Length == 0) {
                            ok = false;
                            break;
                        }

                        types.Add(lastT);
                        types.Add(lastT);
                    } else if (w == "Square" || w == "Cubic") {
                        string t = w == "Square" ? "Area" : "Volume";
                        if (!meta.ContainsKey(t) || k + 1 >= ws.Count) {
                            ok = false;
                            break;
                        }

                        k++;
                        types.Add(t);
                        lastT = t;
                    } else {
                        if (!unitWordType.TryGetValue(w, out var t)) {
                            ok = false;
                            break;
                        }

                        types.Add(t);
                        lastT = t;
                    }
                }

                if (!ok || types.Count == 0) continue;
                string key = string.Join(",", types);
                if (!seqMap.TryGetValue(n, out var inner)) seqMap[n] = inner = new();
                if (!inner.TryGetValue(key, out var rl)) inner[key] = rl = new List<string>();
                if (!rl.Contains(info.Name)) rl.Add(info.Name);
            }

        if (seqMap.Count == 0) return;

        string Su(List<string> s) => string.Join("_", s);

        List<string> TerminalR(string n, List<string> s) =>
            seqMap[n].TryGetValue(string.Join(",", s), out var rl) ? rl : new List<string>();

        var sb = new StringBuilder();
        var emittedCol = new HashSet<string>();
        var emittedPost = new HashSet<string>();
        var emittedMod = new HashSet<string>();

        string EmitMod(string n, List<string> s, string kw) {
            var (partner, term, childs) = mod[kw];
            int power = kw == "Square" ? 2 : 3;
            string mid = "M_" + kw + "_" + n + (s.Count == 0 ? "" : "_" + Su(s));
            string root = mid + "__";
            if (!emittedMod.Add(mid)) return root;
            childs.TryGetValue("", out var rootSet);
            // areal base word(s) reached via a declared SI prefix (Meter, from SquareMillimeters →
            // Milli·Meter) → collapsed into a shared prefixed entry offering the FULL SI range, folding
            // the scaling into pfx as an exact power of ten (square ⇒ 2·e, cubic ⇒ 3·e).
            var prefixable = new SortedSet<string>(StringComparer.Ordinal);
            if (rootSet != null)
                foreach (var w in rootSet)
                    if (PrefixExponentsWithoutNone.ContainsKey(w) && childs.TryGetValue(w, out var gk))
                        foreach (var b in gk)
                            if (term.ContainsKey(b))
                                prefixable.Add(b);

            bool IsPrefixNode(string p) => p.Length > 0 && PrefixExponentsWithoutNone.ContainsKey(p.Split('_')[0]);

            string Terminal(string w, string unit) {
                string cu = CUnit(partner, unit);
                string tgt = EmitPost(n, [.. s, partner]);
                return $"        public {tgt} {w} => new(run / (pfx * {cu}), pfx * {cu}, 1.0);\n";
            }

            var progs = new HashSet<string> { "" };
            foreach (var kv in childs) {
                progs.Add(kv.Key);
                foreach (var w in kv.Value) progs.Add(kv.Key.Length == 0 ? w : kv.Key + "_" + w);
            }

            var ordered = new List<string>(progs);
            ordered.Sort(StringComparer.Ordinal);
            foreach (var prog in ordered) {
                if (IsPrefixNode(prog)) continue; // per-prefix nodes replaced by the shared entry
                var node = new StringBuilder();
                string nn = mid + "__" + prog;
                node.Append($"    public readonly struct {nn} {{\n");
                node.Append($"        private readonly double run, last, pfx;\n");
                node.Append(
                    $"        internal {nn}(double run, double last, double pfx) {{ this.run = run; this.last = last; this.pfx = pfx; }}\n");
                if (childs.TryGetValue(prog, out var set))
                    foreach (var w in set) {
                        if (prog.Length == 0 && PrefixExponentsWithoutNone.ContainsKey(w))
                            continue; // collapsed SI-prefix child
                        string cp = prog.Length == 0 ? w : prog + "_" + w;
                        node.Append(term.TryGetValue(cp, out var unit)
                            ? Terminal(w, unit)
                            : $"        public {mid}__{cp} {w} => new(run, last, pfx);\n");
                    }

                if (prog.Length == 0 && prefixable.Count > 0)
                    foreach (var px in PrefixExponentsWithoutNone.Keys)
                        node.Append(
                            $"        public {mid}__Pre {px} => new(run, last, pfx * 1e{power * PrefixExponents[px]});\n");
                node.Append("    }\n");
                sb.Append(node);
            }

            if (prefixable.Count > 0) {
                var node = new StringBuilder();
                node.Append($"    public readonly struct {mid}__Pre {{\n");
                node.Append($"        private readonly double run, last, pfx;\n");
                node.Append(
                    $"        internal {mid}__Pre(double run, double last, double pfx) {{ this.run = run; this.last = last; this.pfx = pfx; }}\n");
                foreach (var b in prefixable) node.Append(Terminal(b, term[b]));
                node.Append("    }\n");
                sb.Append(node);
            }

            return root;
        }

        string EmitPost(string n, List<string> s) {
            string post = "P_" + n + "_" + Su(s);
            if (!emittedPost.Add(post)) return post;
            var body = new StringBuilder();
            body.Append($"    public readonly struct {post} {{\n");
            body.Append($"        private readonly double run, last, pfx;\n");
            body.Append(
                $"        internal {post}(double run, double last, double pfx) {{ this.run = run; this.last = last; this.pfx = pfx; }}\n");
            var rl = TerminalR(n, s);
            if (rl.Count == 1)
                body.Append(
                    $"        public static implicit operator {rl[0]}({post} n) => {rl[0]}.FromCanonical({FromCoherentVal(rl[0], "n.run")});\n");
            else if (rl.Count > 1)
                foreach (var r in rl)
                    body.Append($"        public {r} {r} => {r}.FromCanonical({FromCoherentVal(r, "run")});\n");
            string sLast = s[s.Count - 1];
            var nxt = AllowedNext(seqMap[n].Keys, s);
            if (nxt.Contains(sLast)) {
                var s1 = new List<string>(s) { sLast };
                body.Append($"        public {EmitPost(n, s1)} Squared => new(run / last, last, 1.0);\n");
                if (AllowedNext(seqMap[n].Keys, s1).Contains(sLast)) {
                    var s2 = new List<string>(s1) { sLast };
                    body.Append($"        public {EmitPost(n, s2)} Cubed => new(run / last / last, last, 1.0);\n");
                }
            }

            var prodTypes = nxt.FindAll(t => t != sLast);
            if (prodTypes.Count > 0) {
                EmitCol(n, s, "prod", prodTypes);
                string colId = "C_" + n + "_prod" + (s.Count == 0 ? "" : "_" + Su(s));
                BuildDenomTrie(prodTypes, unitOpts, out _, out var baseC);
                if (baseC.TryGetValue("", out var bl))
                    foreach (var (w, plural, d, baseUnit) in bl) {
                        string cu = CUnit(d, baseUnit);
                        string tgt = EmitPost(n, [.. s, d]);
                        string ctor = $"new(run / (1.0 * {cu}), 1.0 * {cu}, 1.0)";
                        body.Append($"        public {tgt} {w} => {ctor};\n");
                        if (plural != w) body.Append($"        public {tgt} {plural} => {ctor};\n");
                    }

                if (baseC.Any(kv =>
                        kv.Key.Length > 0 && kv.Value.Count > 0)) // full SI range → shared prefixed entry
                    foreach (var px in PrefixExponentsWithoutNone.Keys)
                        body.Append(
                            $"        public {colId}__Pre {px} => new(run, last, 1e{PrefixExponents[px]});\n");
                if (prodTypes.Contains("Area") && mod.ContainsKey("Square"))
                    body.Append($"        public {EmitMod(n, s, "Square")} Square => new(run, last, 1.0);\n");
                if (prodTypes.Contains("Volume") && mod.ContainsKey("Cubic"))
                    body.Append($"        public {EmitMod(n, s, "Cubic")} Cubic => new(run, last, 1.0);\n");
            }

            if (nxt.Count > 0)
                body.Append($"        public {EmitCol(n, s, "chain", nxt)} Per => new(run, last, 1.0);\n");
            body.Append("    }\n");
            sb.Append(body);
            return post;
        }

        string EmitCol(string n, List<string> ss, string tag, List<string> types) {
            string colId = "C_" + n + "_" + tag + (ss.Count == 0 ? "" : "_" + Su(ss));
            string root = colId + "__";
            if (!emittedCol.Add(colId)) return root;
            BuildDenomTrie(types, unitOpts, out _, out var baseC);
            // Prefixable denominator bases = those reachable after any SI prefix (Milligrams → Grams).
            // The per-prefix nodes are collapsed into one shared `__Pre` entry offering these bases; the
            // root exposes the FULL SI range as properties that scale `pfx` and re-enter that entry.
            var prefBases = new List<BaseC>();
            var seenPb = new HashSet<string>();
            foreach (var kv in baseC)
                if (kv.Key.Length > 0)
                    foreach (var e in kv.Value)
                        if (seenPb.Add(e.W))
                            prefBases.Add(e);
            prefBases.Sort((a, b) => string.CompareOrdinal(a.W, b.W));

            string BaseChildren(IEnumerable<BaseC> bl) {
                var s = new StringBuilder();
                foreach (var (w, plural, d, baseUnit) in bl) {
                    string cu = CUnit(d, baseUnit);
                    string tgt = EmitPost(n, [.. ss, d]);
                    string ctor = $"new(run / (pfx * {cu}), pfx * {cu}, 1.0)";
                    s.Append($"        public {tgt} {w} => {ctor};\n");
                    if (plural != w) s.Append($"        public {tgt} {plural} => {ctor};\n");
                }

                return s.ToString();
            }

            var node = new StringBuilder();
            node.Append($"    public readonly struct {root} {{\n");
            node.Append($"        private readonly double run, last, pfx;\n");
            node.Append(
                $"        internal {root}(double run, double last, double pfx) {{ this.run = run; this.last = last; this.pfx = pfx; }}\n");
            if (baseC.TryGetValue("", out var bl0)) node.Append(BaseChildren(bl0));
            if (prefBases.Count > 0)
                foreach (var px in PrefixExponentsWithoutNone.Keys)
                    node.Append(
                        $"        public {colId}__Pre {px} => new(run, last, pfx * 1e{PrefixExponents[px]});\n");
            if (types.Contains("Area") && mod.ContainsKey("Square"))
                node.Append($"        public {EmitMod(n, ss, "Square")} Square => new(run, last, pfx);\n");
            if (types.Contains("Volume") && mod.ContainsKey("Cubic"))
                node.Append($"        public {EmitMod(n, ss, "Cubic")} Cubic => new(run, last, pfx);\n");
            node.Append("    }\n");
            sb.Append(node);
            if (prefBases.Count > 0) {
                node = new StringBuilder();
                node.Append($"    public readonly struct {colId}__Pre {{\n");
                node.Append($"        private readonly double run, last, pfx;\n");
                node.Append(
                    $"        internal {colId}__Pre(double run, double last, double pfx) {{ this.run = run; this.last = last; this.pfx = pfx; }}\n");
                node.Append(BaseChildren(prefBases));
                node.Append("    }\n");
                sb.Append(node);
            }

            return root;
        }

        var ext = new StringBuilder();
        foreach (var n in new SortedSet<string>(seqMap.Keys, StringComparer.Ordinal)) {
            string root = EmitCol(n, new List<string>(), "root", AllowedNext(seqMap[n].Keys, new List<string>()));
            ext.Append(
                $"        extension({n} x) {{ public {root} Per => new({CoherentVal(n, "x.CanonicalValue")}, 0.0, 1.0); }}\n");
        }

        var outSrc = new StringBuilder("// <auto-generated/>\n#nullable disable\n\nnamespace " + ns +
                                       " {\n    public static partial class Units {\n");
        outSrc.Append(ext);
        outSrc.Append("    }\n");
        outSrc.Append(sb);
        outSrc.Append("}\n");
        ctx.AddSource("_Per.g.cs", SourceText.From(outSrc.ToString(), Encoding.UTF8));
    }
}
