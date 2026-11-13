using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Measurement.Generators;

// Square/Cubic area & volume modifier: `.Square.Milli.Meters`, `.Cubic.Centi.Meters`.
public partial class MeasurementGenerator {
    // Read-out mirror of EmitSquareCubic: `reader.To.Square.Milli.Meters` / `.To.Cubic.Centi.Meters`
    // read a double. The areal/cubic base (Meters) accepts the FULL SI prefix range, folded into the
    // reader's running factor as an exact power of ten (a prefix of ten-exponent e scales an n-th-power
    // unit by 1e{n·e}) via a shared prefixed entry, so `.Square.Yotta.Meters` etc. compose too.
    // Non-metric spellings (Square.Miles, Cubic.Feet) remain direct terminals.
    private static void EmitSquareCubicRead(SourceProductionContext ctx, ImmutableArray<MeasurementInfo> all) {
        string ns = ResolveNamespace(all);
        var bases = SingleWordBases(all);

        var starts = new StringBuilder();
        var structs = new StringBuilder();
        bool any = false;
        foreach (var info in all)
            foreach (var modifier in new[] { "Square", "Cubic" }) {
                string T = info.Name;
                int power = modifier == "Square" ? 2 : 3;
                var terminal = new Dictionary<string, string>(); // path -> full unit
                var children = new Dictionary<string, SortedSet<string>>();
                var nodes = new HashSet<string> { "" };
                foreach (var u in EnumerateUnitNames(info.Units)) {
                    var ws = SplitWords(u);
                    if (ws.Count < 2 || ws[0] != modifier || ws.Contains("Per")) continue;
                    var rest = new List<string>();
                    for (int i = 1; i < ws.Count; i++) rest.AddRange(DecomposeDenom(ws[i], bases));
                    string key = string.Join("_", rest);
                    if (!terminal.ContainsKey(key)) terminal[key] = u;
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
                any = true;
                string Node(string p) => "R" + modifier + "_" + T + (p.Length == 0 ? "" : "_" + p);
                string preName = "R" + modifier + "_" + T + "__Pre";
                children.TryGetValue("", out var rootKids);

                bool IsPrefixNode(string p) => p.Length > 0 && PrefixExponentsWithoutNone.ContainsKey(p.Split('_')[0]);

                // base words reachable via a declared SI prefix (Meters) and also present unprefixed:
                // these get the full SI range via the shared prefixed entry.
                var prefixable = new SortedSet<string>(StringComparer.Ordinal);
                if (rootKids != null)
                    foreach (var w in rootKids)
                        if (PrefixExponentsWithoutNone.ContainsKey(w) && children.TryGetValue(w, out var gk))
                            foreach (var b in gk)
                                if (terminal.ContainsKey(b))
                                    prefixable.Add(b);
                starts.Append(
                    $"        extension(Reader<{T}> r) {{ public {Node("")} {modifier} => new(r.Value, r.Factor); }}\n");
                var ordered = new List<string>(nodes);
                ordered.Sort(StringComparer.Ordinal);
                foreach (var p in ordered) {
                    if (IsPrefixNode(p)) continue; // per-prefix nodes replaced by the shared entry
                    string nn = Node(p);
                    structs.Append(
                        $"    public readonly struct {nn} {{\n        private readonly {T} v; private readonly double f;\n        internal {nn}({T} v, double f) {{ this.v = v; this.f = f; }}\n");
                    if (children.TryGetValue(p, out var set))
                        foreach (var w in set)
                            if (!PrefixExponentsWithoutNone.ContainsKey(w)) // skip SI-prefix children (handled below)
                                structs.Append(
                                    $"        public {Node(p.Length == 0 ? w : p + "_" + w)} {w} => new(v, f);\n");
                    if (p.Length == 0 && prefixable.Count > 0) // full SI prefix range on the modifier entry
                        foreach (var px in PrefixExponentsWithoutNone.Keys)
                            structs.Append(
                                $"        public {preName} {px} => new(v, f * 1e{power * PrefixExponents[px]});\n");
                    if (p.Length > 0 && terminal.TryGetValue(p, out var u))
                        structs.Append(
                            $"        public static implicit operator double({nn} n) => n.v.To{u}() / n.f;\n");
                    structs.Append("    }\n");
                }

                if (prefixable.Count > 0) { // shared prefixed entry: base units, factor pre-scaled
                    structs.Append(
                        $"    public readonly struct {preName} {{\n        private readonly {T} v; private readonly double f;\n        internal {preName}({T} v, double f) {{ this.v = v; this.f = f; }}\n");
                    foreach (var w in prefixable)
                        structs.Append($"        public {Node(w)} {w} => new(v, f);\n");
                    structs.Append("    }\n");
                }
            }

        if (!any) return;
        string src = $@"// <auto-generated/>
#nullable disable

namespace {ns} {{
    public static partial class Units {{
{starts}    }}
{structs}}}
";
        ctx.AddSource("_SquareCubicRead.g.cs", SourceText.From(src, Encoding.UTF8));
    }
}
