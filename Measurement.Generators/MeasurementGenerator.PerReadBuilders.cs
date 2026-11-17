using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using static Measurement.Generators.Utils;

namespace Measurement.Generators;

// Read-out mirror: `reader.To.<Numerator>.Per.<denominator…>` word-walk returning a double.
public class MeasurementGenerator_PerReadBuilders {
    // Read-out mirror of EmitPerBuilders: `reader.To.<flatNumerator>.Per.<denominator…>` reads a
    // double. The numerator is a flat spelling on Reader<C> (its coherent value seeds the running
    // product); each denominator step multiplies by one coherent unit of the chosen dimension, so any
    // (non-affine) unit composes. Squared/Cubed repeat the last unit; `.Square`/`.Cubic` and bare
    // product factors (`.Per.Kilo.Gram.Kelvin`) extend the denominator. A completed type-sequence
    // returns the value in those units via implicit conversion to double.
    public static void EmitPerReadBuilders(SourceProductionContext ctx,
        ImmutableArray<MeasurementInfo> all) {
        string ns = ResolveNamespace(all);
        var (meta, typeOfUnit, unitWordType, unitOpts) = BuildPerCommon(all);
        var removable = BuildRemovable(all);
        (int, double) Div(string t) => CoherentDivisor(meta[t].DF, meta[t].G);
        bool hasMole = meta.ContainsKey("Quantity");

        string CoherentVal(string t, string valExpr) =>
            t == "Quantity" && hasMole
                ? $"({valExpr} / (Quantity.FromMoles(1).CanonicalValue))"
                : ToCoherentExpr(valExpr, Div(t));

        string CUnit(string d, string baseUnit) =>
            CoherentVal(d, $"({Cons(d, d, baseUnit, "1", removable)}.CanonicalValue)");

        var mod = BuildDenomModTrie(all, meta);

        string Su(List<string> s) => string.Join("_", s);

        List<string> ParseDenomTypes(List<string> ws, int per) {
            var types = new List<string>();
            string lastT = "";
            bool ok = true;
            for (int k = per + 1; k < ws.Count; k++) {
                string w = ws[k];
                if (w == "Per") continue;
                if (w == "Squared") {
                    if (lastT.Length == 0) return new List<string>();
                    types.Add(lastT);
                } else if (w == "Cubed") {
                    if (lastT.Length == 0) return new List<string>();
                    types.Add(lastT);
                    types.Add(lastT);
                } else if (w == "Square" || w == "Cubic") {
                    string t = w == "Square" ? "Area" : "Volume";
                    if (!meta.ContainsKey(t) || k + 1 >= ws.Count) return new List<string>();
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

            return ok && types.Count > 0 ? types : new List<string>();
        }

        // readMap[C][numFlat] = (numType, set of denominator type-sequences).
        var readMap = new Dictionary<string, Dictionary<string, (string NumType, HashSet<string> Seqs)>>();
        foreach (var info in all) {
            string c = info.Name;
            var names = EnumerateUnitNames(info.Units);
            var simple = new HashSet<string>();
            foreach (var u in names)
                if (!SplitWords(u).Contains("Per"))
                    simple.Add(u);
            foreach (var u in names) {
                var ws = SplitWords(u);
                int per = ws.IndexOf("Per");
                if (per <= 0 || per == ws.Count - 1 || PerExcludeUnits.Contains(u)) continue;
                string numFlat = string.Concat(ws.GetRange(0, per));
                if (simple.Contains(numFlat) || !typeOfUnit.TryGetValue(numFlat, out var numType)) continue;
                var types = ParseDenomTypes(ws, per);
                if (types.Count == 0) continue;
                if (!readMap.TryGetValue(c, out var byNum)) readMap[c] = byNum = new();
                if (!byNum.TryGetValue(numFlat, out var e)) byNum[numFlat] = e = (numType, new HashSet<string>());
                e.Seqs.Add(string.Join(",", types));
            }
        }

        if (readMap.Count == 0) return;

        var starts = new StringBuilder();
        var sb = new StringBuilder();
        var emittedCol = new HashSet<string>();
        var emittedPost = new HashSet<string>();
        var emittedMod = new HashSet<string>();

        // per (C, numFlat) helpers close over the current cn = C + "_" + numFlat key.
        foreach (var c in new SortedSet<string>(readMap.Keys, StringComparer.Ordinal)) {
            foreach (var numFlat in new SortedSet<string>(readMap[c].Keys, StringComparer.Ordinal)) {
                var (numType, seqs) = readMap[c][numFlat];
                string cn = c + "_" + numFlat;

                bool IsTerminal(List<string> s) => seqs.Contains(string.Join(",", s));

                string EmitMod(List<string> s, string kw) {
                    var (partner, term, childs) = mod[kw];
                    int power = kw == "Square" ? 2 : 3;
                    string mid = "RM_" + kw + "_" + cn + (s.Count == 0 ? "" : "_" + Su(s));
                    string root = mid + "__";
                    if (!emittedMod.Add(mid)) return root;
                    childs.TryGetValue("", out var rootSet);
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
                        string tgt = EmitPostRef([.. s, partner]);
                        return $"        public {tgt} {w} => new(run * (pfx * {cu}), pfx * {cu}, 1.0);\n";
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

                string EmitCol(List<string> ss, List<string> types) {
                    string colId = "RC_" + cn + (ss.Count == 0 ? "" : "_" + Su(ss));
                    string root = colId + "__";
                    if (!emittedCol.Add(colId)) return root;
                    BuildDenomTrie(types, unitOpts, out _, out var baseC);
                    // SI-prefixable denominator bases collapse into one shared `__Pre`; the root exposes
                    // the full SI range as properties that scale pfx and re-enter it (read multiplies).
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
                            string tgt = EmitPostRef([.. ss, d]);
                            string ctor = $"new(run * (pfx * {cu}), pfx * {cu}, 1.0)";
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
                        node.Append($"        public {EmitMod(ss, "Square")} Square => new(run, last, pfx);\n");
                    if (types.Contains("Volume") && mod.ContainsKey("Cubic"))
                        node.Append($"        public {EmitMod(ss, "Cubic")} Cubic => new(run, last, pfx);\n");
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

                string EmitPostRef(List<string> s) {
                    string post = "RP_" + cn + "_" + Su(s);
                    if (!emittedPost.Add(post)) return post;
                    var body = new StringBuilder();
                    body.Append($"    public readonly struct {post} {{\n");
                    body.Append($"        private readonly double run, last, pfx;\n");
                    body.Append(
                        $"        internal {post}(double run, double last, double pfx) {{ this.run = run; this.last = last; this.pfx = pfx; }}\n");
                    if (IsTerminal(s))
                        body.Append($"        public static implicit operator double({post} n) => n.run;\n");
                    string sLast = s[s.Count - 1];
                    var nxt = AllowedNext(seqs, s);
                    if (nxt.Contains(sLast)) {
                        var s1 = new List<string>(s) { sLast };
                        body.Append($"        public {EmitPostRef(s1)} Squared => new(run * last, last, 1.0);\n");
                        if (AllowedNext(seqs, s1).Contains(sLast)) {
                            var s2 = new List<string>(s1) { sLast };
                            body.Append(
                                $"        public {EmitPostRef(s2)} Cubed => new(run * last * last, last, 1.0);\n");
                        }
                    }

                    if (nxt.Count > 0) {
                        EmitCol(s, nxt);
                        string colId = "RC_" + cn + "_" + Su(s);
                        BuildDenomTrie(nxt, unitOpts, out _, out var baseC);
                        if (baseC.TryGetValue("", out var bl))
                            foreach (var (w, plural, d, baseUnit) in bl) {
                                string cu = CUnit(d, baseUnit);
                                string tgt = EmitPostRef([.. s, d]);
                                string ctor = $"new(run * (1.0 * {cu}), 1.0 * {cu}, 1.0)";
                                body.Append($"        public {tgt} {w} => {ctor};\n");
                                if (plural != w) body.Append($"        public {tgt} {plural} => {ctor};\n");
                            }

                        if (baseC.Any(kv =>
                                kv.Key.Length > 0 && kv.Value.Count > 0)) // full SI range → shared prefixed entry
                            foreach (var px in PrefixExponentsWithoutNone.Keys)
                                body.Append(
                                    $"        public {colId}__Pre {px} => new(run, last, 1e{PrefixExponents[px]});\n");
                        if (nxt.Contains("Area") && mod.ContainsKey("Square"))
                            body.Append($"        public {EmitMod(s, "Square")} Square => new(run, last, 1.0);\n");
                        if (nxt.Contains("Volume") && mod.ContainsKey("Cubic"))
                            body.Append($"        public {EmitMod(s, "Cubic")} Cubic => new(run, last, 1.0);\n");
                    }

                    body.Append("    }\n");
                    sb.Append(body);
                    return post;
                }

                string start = "RS_" + cn;
                string numCoh = CUnit(numType, numFlat);
                string runStart = $"(({ToCoherentExpr("r.Value.CanonicalValue", Div(c))}) / ({numCoh}) / r.Factor)";
                starts.Append(
                    $"        extension(Reader<{c}> r) {{ public {start} {numFlat} => new({runStart}, 0.0, 1.0); }}\n");
                string colRoot = EmitCol(new List<string>(), AllowedNext(seqs, new List<string>()));
                sb.Append($"    public readonly struct {start} {{\n");
                sb.Append($"        private readonly double run, last, pfx;\n");
                sb.Append(
                    $"        internal {start}(double run, double last, double pfx) {{ this.run = run; this.last = last; this.pfx = pfx; }}\n");
                sb.Append($"        public {colRoot} Per => new(run, last, pfx);\n");
                sb.Append("    }\n");
            }
        }

        var outSrc = new StringBuilder("// <auto-generated/>\n#nullable disable\n\nnamespace " + ns +
                                       " {\n    public static partial class Units {\n");
        outSrc.Append(starts);
        outSrc.Append("    }\n");
        outSrc.Append(sb);
        outSrc.Append("}\n");
        ctx.AddSource("_PerRead.g.cs", SourceText.From(outSrc.ToString(), Encoding.UTF8));
    }
}
