using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Measurement.Generators;

// Square/Cubic area & volume modifier: `.Square.Milli.Meters`, `.Cubic.Centi.Meters`.
public partial class MeasurementGenerator {
    // Emits `.Square`/`.Cubic` area & volume modifiers on Prefixed/double. The areal/cubic base (Meters)
    // accepts the FULL SI prefix range, folded into the value as an exact power of ten (a prefix of ten
    // exponent e scales an n-th-power unit by 1e{n·e}) via a shared prefixed entry, so `.Square.Milli.Meters`,
    // `.Cubic.Kilo.Meters`, etc. compose without per-prefix nodes. Each base terminal maps to the
    // Square<Unit>/Cubic<Unit> factory; non-metric spellings (`.Square.Miles`, `.Square.Degrees`) stay direct.
    private static void EmitSquareCubic(SourceProductionContext ctx, ImmutableArray<MeasurementInfo> all) {
        string ns = ResolveNamespace(all);
        // Known single-word unit bases (for prefix splitting Millimeters → Milli, Meters).
        var bases = SingleWordBases(all);

        // modifier ("Square"/"Cubic") → decomposed rest path → (type, full unit).
        foreach (var modifier in new[] { "Square", "Cubic" }) {
            int power = modifier == "Square" ? 2 : 3;
            var terminal = new Dictionary<string, (string Type, string Unit)>();
            var children = new Dictionary<string, SortedSet<string>>();
            var nodes = new HashSet<string> { "" };
            foreach (var info in all)
                foreach (var u in EnumerateUnitNames(info.Units)) {
                    var ws = SplitWords(u);
                    if (ws.Count < 2 || ws[0] != modifier || ws.Contains("Per")) continue;
                    var rest = new List<string>();
                    for (int i = 1; i < ws.Count; i++) rest.AddRange(DecomposeDenom(ws[i], bases));
                    string key = string.Join("_", rest);
                    if (!terminal.ContainsKey(key)) terminal[key] = (info.Name, u);
                    for (int k = 0; k <= rest.Count; k++) {
                        string p = string.Join("_", rest.GetRange(0, k));
                        nodes.Add(p);
                        if (k < rest.Count) {
                            if (!children.TryGetValue(p, out var set))
                                children[p] = set = new SortedSet<string>(StringComparer.Ordinal);
                            set.Add(rest[k]);
                        }
                    }
                }

            if (terminal.Count == 0) continue;
            string Node(string p) => modifier + "Of" + (p.Length == 0 ? "" : "_" + p);
            string preName = modifier + "Of__Pre";
            children.TryGetValue("", out var rootKids);

            bool IsPrefixNode(string p) => p.Length > 0 && PrefixExponentsWithoutNone.ContainsKey(p.Split('_')[0]);

            var prefixable = new SortedSet<string>(StringComparer.Ordinal);
            if (rootKids != null)
                foreach (var w in rootKids)
                    if (PrefixExponentsWithoutNone.ContainsKey(w) && children.TryGetValue(w, out var gk))
                        foreach (var b in gk)
                            if (terminal.ContainsKey(b))
                                prefixable.Add(b);
            var structs = new StringBuilder();
            var ordered = new List<string>(nodes);
            ordered.Sort(StringComparer.Ordinal);
            foreach (var p in ordered) {
                if (IsPrefixNode(p)) continue; // per-prefix nodes replaced by the shared entry
                string nn = Node(p);
                structs.Append(
                    $"    public readonly struct {nn} {{\n        private readonly double v;\n        internal {nn}(double v) => this.v = v;\n");
                if (children.TryGetValue(p, out var set))
                    foreach (var w in set)
                        if (!PrefixExponentsWithoutNone.ContainsKey(w)) // skip SI-prefix children (handled below)
                            structs.Append(
                                $"        public {Node(p.Length == 0 ? w : p + "_" + w)} {w} => new(v);\n");
                if (p.Length == 0 && prefixable.Count > 0) // full SI prefix range on the modifier entry
                    foreach (var px in PrefixExponentsWithoutNone.Keys)
                        structs.Append(
                            $"        public {preName} {px} => new(v * 1e{power * PrefixExponents[px]});\n");
                if (p.Length > 0 && terminal.TryGetValue(p, out var t))
                    structs.Append(
                        $"        public static implicit operator {t.Type}({nn} n) => {t.Type}.From{t.Unit}(n.v);\n");
                structs.Append("    }\n");
            }

            if (prefixable.Count > 0) { // shared prefixed entry: base units, value pre-scaled
                structs.Append(
                    $"    public readonly struct {preName} {{\n        private readonly double v;\n        internal {preName}(double v) => this.v = v;\n");
                foreach (var w in prefixable)
                    structs.Append($"        public {Node(w)} {w} => new(v);\n");
                structs.Append("    }\n");
            }

            string src = $@"// <auto-generated/>
#nullable disable

namespace {ns} {{
    public static partial class Units {{
        extension(Prefixed p) {{ public {Node("")} {modifier} => new(p.Value); }}
    }}
{structs}}}

namespace {ns}.Fluent {{
    using {ns};
    public static partial class DoubleSugar {{
        extension(double value) {{ public {Node("")} {modifier} => new(value); }}
    }}
}}
";
            ctx.AddSource("_" + modifier + ".g.cs", SourceText.From(src, Encoding.UTF8));
        }
    }
}
