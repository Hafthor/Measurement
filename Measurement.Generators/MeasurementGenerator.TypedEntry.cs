using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Measurement.Generators;

// Type-constrained fluent entry: `Area.Of(4).Square.Milli.Meters` — only T's own units.
public partial class MeasurementGenerator {
    // is limited to ways of constructing T (e.g. `Area.Of(4).Square.Milli.Meters`). Each unit name is
    // decomposed into fluent path segments (SplitWords + SI-prefix splitting: `SquareMillimeters` →
    // Square·Milli·Meters, `Kilometers` → Kilo·Meters, `MetersPerSecond` → Meters·Per·Second); a
    // completed path returns `T.From<Unit>(value)`. Purely reuses the existing From factories.
    private static void EmitTypedEntry(SourceProductionContext ctx,
        ImmutableArray<MeasurementInfo> all) {
        string ns = ResolveNamespace(all);
        // Start from the removable prefix aliases (Kilometers→Meters, Hectares→Ares, …); the typed
        // enumerator extends this with the extra all-prefix numerator variants it introduces.
        var construct =
            new Dictionary<string, (string Base, int Exp)>(
                BuildRemovable(all)); // + compound-expandable entries below
        var bases = new HashSet<string>();
        foreach (var info in all)
            foreach (var u in EnumerateUnitNames(info.Units))
                if (SplitWords(u).Count == 1) {
                    bases.Add(u);
                    bases.Add(Depluralize(u)); // singular, for denominators (Kilogram → Kilo.Gram)
                }

        var structs = new StringBuilder();
        var partials = new StringBuilder();
        foreach (var info in all.OrderBy(a => a.Name, StringComparer.Ordinal)) {
            string T = info.Name;
            // Trie of unprefixed unit paths (plus full-range paths for compound expandables). SIMPLE
            // prefix-expandable units get the whole SI range layered on via a shared prefixed-entry
            // struct that folds the scaling into the value and re-enters this trie — no per-prefix nodes.
            var children = new Dictionary<string, SortedDictionary<string, string>>();
            var terminal = new Dictionary<string, string>();
            var nodes = new HashSet<string> { "" };
            foreach (var u in EnumerateTypedEntryUnits(info.Units, T, construct)) {
                var path = new List<string>();
                foreach (var w in SplitWords(u)) path.AddRange(DecomposeDenom(w, bases));
                if (path.Count == 0) continue;
                string acc = "";
                foreach (var w in path) {
                    string child = acc.Length == 0 ? w : acc + "_" + w;
                    nodes.Add(child);
                    if (!children.TryGetValue(acc, out var m))
                        children[acc] = m = new SortedDictionary<string, string>(StringComparer.Ordinal);
                    m[w] = child;
                    acc = child;
                }

                if (!terminal.ContainsKey(acc)) terminal[acc] = u; // first unit wins on collision
            }

            var erWords = ExpandableRootWords(info.Units, bases);
            string NodeName(string p) => "Of_" + T + (p.Length == 0 ? "" : "_" + p);
            bool HasChildren(string p) => children.TryGetValue(p, out var m) && m.Count > 0;
            string qt = "global::" + ns + "." + T; // a child word may equal the type name, shadowing it
            string
                preName = "Of_" + T + "__Pre"; // shared prefixed-entry struct (double underscore avoids path clash)

            // The member stepping into root child `w` from a node carrying value `val` (structural child →
            // next node; terminal → construct T). SI prefixing reuses this by passing a scaled `val`.
            string Member(string w, string val) => HasChildren(children[""][w])
                ? $"        public {NodeName(children[""][w])} {w} => new({val});\n"
                : $"        public {qt} {w} => {Cons(qt, T, terminal[children[""][w]], val, construct)};\n";

            // Member for an arbitrary node's child (unscaled) — used for the normal trie transitions.
            string ChildMember(string w, string child) => HasChildren(child)
                ? $"        public {NodeName(child)} {w} => new(v);\n"
                : $"        public {qt} {w} => {Cons(qt, T, terminal[child], "v", construct)};\n";

            // Route each SI prefix: onto the root as a `.Prefix` property re-entering the shared Pre
            // entry (value scaled), unless it collides with a declared prefix-leading unit's node (e.g.,
            // Kilocalories' `.Kilo`), in which case the scaled expandable roots are merged into that node.
            var rootPrefixProps = new StringBuilder();
            var injected = new Dictionary<string, StringBuilder>();
            bool usePre = false;
            if (erWords.Count > 0)
                foreach (var px in PrefixExponentsWithoutNone.Keys) {
                    int exp = PrefixExponents[px];
                    if (children.TryGetValue("", out var rc) && rc.ContainsKey(px)) {
                        if (!injected.TryGetValue(px, out var inj)) injected[px] = inj = new StringBuilder();
                        foreach (var w in erWords.OrderBy(s => s, StringComparer.Ordinal))
                            if (!(children.TryGetValue(px, out var nc) &&
                                  nc.ContainsKey(w))) // don't shadow an existing child
                                inj.Append(Member(w, $"v * 1e{exp}"));
                    } else {
                        rootPrefixProps.Append($"        public {preName} {px} => new(v * 1e{exp});\n");
                        usePre = true;
                    }
                }

            partials.Append(
                $"    public readonly partial struct {T} {{ public static {NodeName("")} Of(double value) => new(value); }}\n");
            // Square/Cubic subentries: collapse the declared per-prefix areal/cubic nodes
            // (Of_T_Square_Milli → …SquareMillimeters) into a shared prefixed entry offering the FULL SI
            // range, folding the scaling into the value as an exact power of ten (square ⇒ 2·e, cubic ⇒
            // 3·e) — mirroring the untyped Square/Cubic and read builders.
            var scPre = new Dictionary<string, (List<string> Bases, int Power)>();
            var scSkip = new HashSet<string>();
            foreach (var mp in new[] { "Square", "Cubic" }) {
                if (!children.TryGetValue(mp, out var kids)) continue;
                int power = mp == "Square" ? 2 : 3;
                var bs = new List<string>();
                foreach (var w in kids.Keys)
                    if (PrefixExponentsWithoutNone.ContainsKey(w)) {
                        string pnode = mp + "_" + w;
                        if (children.TryGetValue(pnode, out var gk))
                            foreach (var b in gk.Keys)
                                if (kids.ContainsKey(b) && !bs.Contains(b))
                                    bs.Add(b); // base also a direct terminal child
                        foreach (var np in nodes)
                            if (np == pnode || np.StartsWith(pnode + "_"))
                                scSkip.Add(np);
                    }

                if (bs.Count > 0) {
                    bs.Sort(StringComparer.Ordinal);
                    scPre[mp] = (bs, power);
                }
            }

            string ScPreName(string p) => NodeName(p) + "__Pre";
            var ordered = new List<string>(nodes);
            ordered.Sort(StringComparer.Ordinal);
            foreach (var p in ordered) {
                if (!HasChildren(p) || scSkip.Contains(p))
                    continue; // leaf terminals are parent properties; skip collapsed prefix nodes
                string nn = NodeName(p);
                structs.Append($"    public readonly struct {nn} {{\n");
                structs.Append($"        private readonly double v;\n");
                structs.Append($"        internal {nn}(double v) => this.v = v;\n");
                bool scHere = scPre.ContainsKey(p);
                foreach (var kv in children[p])
                    if (!(scHere && PrefixExponentsWithoutNone.ContainsKey(kv.Key))) // skip collapsed SI-prefix children
                        structs.Append(ChildMember(kv.Key, kv.Value));
                if (scHere) // full SI range on the Square/Cubic node
                    foreach (var px in PrefixExponentsWithoutNone.Keys)
                        structs.Append(
                            $"        public {ScPreName(p)} {px} => new(v * 1e{scPre[p].Power * PrefixExponents[px]});\n");
                if (injected.TryGetValue(p, out var inj)) structs.Append(inj); // merged SI-prefix expandable roots
                if (p.Length == 0) structs.Append(rootPrefixProps); // non-colliding SI-prefix properties
                if (terminal.TryGetValue(p, out var tu)) // node both internal and a unit
                    structs.Append(
                        $"        public static implicit operator {qt}({nn} n) => {Cons(qt, T, tu, "n.v", construct)};\n");
                structs.Append("    }\n");
            }

            foreach (var kv in scPre) { // shared Square/Cubic prefixed entries (value pre-scaled)
                structs.Append($"    public readonly struct {ScPreName(kv.Key)} {{\n");
                structs.Append($"        private readonly double v;\n");
                structs.Append($"        internal {ScPreName(kv.Key)}(double v) => this.v = v;\n");
                foreach (var b in kv.Value.Bases) structs.Append(ChildMember(b, children[kv.Key][b]));
                structs.Append("    }\n");
            }

            if (usePre) { // shared prefixed entry: the prefixable roots (v pre-scaled)
                structs.Append($"    public readonly struct {preName} {{\n");
                structs.Append($"        private readonly double v;\n");
                structs.Append($"        internal {preName}(double v) => this.v = v;\n");
                foreach (var w in erWords.OrderBy(s => s, StringComparer.Ordinal))
                    structs.Append(Member(w, "v"));
                structs.Append("    }\n");
            }
        }

        string src = $@"// <auto-generated/>
#nullable disable

namespace {ns} {{
{partials}{structs}}}
";
        ctx.AddSource("_Of.g.cs", SourceText.From(src, Encoding.UTF8));
    }

    // The unit names the type-constrained `.Of` entry offers in its trie. SIMPLE prefix-expandable
    // units are yielded unprefixed (the full SI range is layered on via the shared prefixed entry —
    // see ExpandableRootWords). COMPOUND prefix-expandable units (GramsPerSecond) ARE expanded to the
    // full numerator range here so their per-prefix paths merge with declared prefixed compounds
    // (KilogramsPerHour); each such prefixed name records `type|name -> (base variant, ten-exponent)`
    // in `construct` for base-factory + scaling construction. Everything else is yielded as declared.
    private static List<string> EnumerateTypedEntryUnits(List<IUnitSpec> units, string typeName,
        Dictionary<string, (string Base, int Exp)> construct) {
        var names = new List<string>();
        if (units.Count == 0) return names;
        foreach (var unit in units) {
            if (unit is not SiUnitSpec si) {
                names.Add(unit.Name);
                continue;
            }

            // si|baseName|exp|prefixes|perPrefixes
            string baseName = si.Name;
            string[] declaredNum = si.PrefixesSplit;
            string[] denPrefixes = si.PerPrefixesSplitOrNone;
            var words = SplitWords(baseName);
            int perIdx = words.IndexOf("Per");
            (int numRoot, int numPow) = FindRoot(words, 0, perIdx < 0 ? words.Count : perIdx);
            int denRoot = -1;
            if (perIdx >= 0) (denRoot, _) = FindRoot(words, perIdx + 1, words.Count);
            bool expandable = numRoot >= 0 &&
                              Array.Exists(declaredNum, p => PrefixExponentsWithoutNone.ContainsKey(p));
            bool compound = perIdx >= 0;
            // simple expandable → unprefixed (Pre adds the range); compound expandable → full range
            // here (merges with declared compounds); otherwise as declared.
            ICollection<string> numPrefixes =
                expandable ? (compound ? PrefixExponents.Keys : new[] { "None" }) : declaredNum;
            foreach (var np in numPrefixes) {
                if (np.Length == 0 || !PrefixExponents.TryGetValue(np, out int npExp)) continue;
                string[] dps = denRoot < 0 ? ["None"] : denPrefixes;
                foreach (var dp in dps) {
                    if (dp.Length == 0 || !PrefixExponents.ContainsKey(dp)) continue;
                    var w = new List<string>(words);
                    if (np != "None") w[numRoot] = np + LowerFirstChar(words[numRoot]);
                    if (denRoot >= 0 && dp != "None") w[denRoot] = dp + LowerFirstChar(words[denRoot]);
                    string full = string.Concat(w);
                    names.Add(full);
                    if (expandable && compound && np != "None") {
                        var nv = new List<string>(words); // None-numerator base variant (same denominator)
                        if (denRoot >= 0 && dp != "None") nv[denRoot] = dp + LowerFirstChar(words[denRoot]);
                        construct[typeName + "|" + full] = (string.Concat(nv), npExp * numPow);
                    }
                }
            }
        }

        return names;
    }

    // The numerator "root" words of a type's SIMPLE prefix-expandable SI units — single-word units
    // declaring ≥1 numerator prefix (Meters, Grams, Joules, Bars, …). The `.Of` entry offers the full
    // SI prefix range for these via a shared prefixed-entry struct that folds the scaling into the
    // value. Compound-expandable units (GramsPerSecond) are excluded here — they keep per-prefix trie
    // paths (see EnumerateTypedEntryUnits) so they merge correctly with declared prefixed compounds.
    private static HashSet<string> ExpandableRootWords(List<IUnitSpec> units, HashSet<string> bases) {
        var er = new HashSet<string>();
        if (units.Count == 0) return er;
        foreach (var si in units.OfType<SiUnitSpec>()) {
            var words = SplitWords(si.Name);
            if (words.Contains("Per")) continue; // compound → not collapsed here
            var declaredNum = si.PrefixesSplit;
            if (!Array.Exists(declaredNum, p => PrefixExponentsWithoutNone.ContainsKey(p))) continue;
            var path = new List<string>();
            foreach (var w in words) path.AddRange(DecomposeDenom(w, bases));
            if (path.Count > 0) er.Add(path[0]);
        }

        return er;
    }
}
