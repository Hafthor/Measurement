using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using static Measurement.Generators.Utils;

namespace Measurement.Generators;

// Product-unit token algebra: `.Ampere.Hours`, `.Joule.Minutes`, `.Light.Seconds`,
// `.Newton.Meters` (→ Torque|Energy) — tokens compose via the [Product] relations.
public class MeasurementGenerator_TokenAlgebra {
    // Product-unit token algebra (input side). Each measurement type reachable as a running product
    // becomes a `ProductState_<Type>` struct carrying the running canonical value and implicitly
    // convertible to that type. A first token on Prefixed/double (the singular base unit of a
    // product-participating type, plus `Light` = the speed of light) enters the corresponding state;
    // each subsequent single-word unit token multiplies by one of that unit and transitions to the
    // product type via the [Product] relations, with the exact operator factor. Dimensionally
    // ambiguous products (Force × Length → Torque or Energy) resolve to a selector naming each result.
    // So `.Ampere.Hours`, `.Joule.Minutes`, `.Light.Seconds`, `.Newton.Meters.Torque` all compose.
    public static void EmitTokenAlgebra(SourceProductionContext ctx, ImmutableArray<MeasurementInfo> all) {
        string ns = ResolveNamespace(all);
        var removable = BuildRemovable(all);
        var meta = new Dictionary<string, (double DF, int G)>();
        foreach (var info in all) meta[info.Name] = (info.DisplayFactor, GramPower(info.Symbol));
        (int, double) Div(string t) => CoherentDivisor(meta[t].DF, meta[t].G);

        // Single-word unit tokens → (type, unit). Plural forms as declared; singular base unit per
        // type is the entry token. `Light` is a synthetic Speed token (the speed of light).
        var tokenType = new Dictionary<string, (string Type, string Unit)>(); // multiplying tokens
        var entryToken = new Dictionary<string, (string Type, string Unit)>(); // Prefixed entry tokens
        var baseUnit = new Dictionary<string, string>(); // type → its base unit
        var singleWords = new Dictionary<string, HashSet<string>>(); // type → its single-word units
        foreach (var info in all) {
            foreach (var u in EnumerateUnitNames(info.Units)) {
                if (SplitWords(u).Count != 1) continue;
                if (!tokenType.ContainsKey(u)) tokenType[u] = (info.Name, u);
                if (!baseUnit.ContainsKey(info.Name)) baseUnit[info.Name] = u; // first single-word unit
                if (!singleWords.TryGetValue(info.Name, out var set))
                    singleWords[info.Name] = set = new HashSet<string>();
                set.Add(u);
            }
        }

        // product pairs (unordered) → results, plus the Primary result of each pair (implicit target).
        var pair = new Dictionary<string, List<string>>();
        var primaryResult = new Dictionary<string, string>();

        void AddPair(string a, string b, string c, bool primary) {
            string k = string.CompareOrdinal(a, b) <= 0 ? a + "|" + b : b + "|" + a;
            if (!pair.TryGetValue(k, out var l)) pair[k] = l = new List<string>();
            if (!l.Contains(c)) l.Add(c);
            if (primary) primaryResult[k] = c;
        }

        foreach (var info in all)
            foreach (var product in info.Products)
                AddPair(product.LeftName, product.RightName, info.Name, product.IsPrimary);

        List<string> Results(string r, string t) {
            string k = string.CompareOrdinal(r, t) <= 0 ? r + "|" + t : t + "|" + r;
            return pair.TryGetValue(k, out var l) ? l : new List<string>();
        }

        string PrimaryOf(string r, string t) {
            string k = string.CompareOrdinal(r, t) <= 0 ? r + "|" + t : t + "|" + r;
            return primaryResult.TryGetValue(k, out var p) ? p : "";
        }

        // Entry tokens: the singular base unit of each product-participating type + Light. If a type's
        // base unit is itself an SI-prefixed name (Mass's coherent unit is the kilogram = Kilo+Grams),
        // decompose it so the entry token is the unprefixed singular (Gram), reached via the prefix
        // chain (`.Kilo.Gram`) — matching the rest of the fluent surface rather than a fused `.Kilogram`.
        var participating = new HashSet<string>();
        foreach (var k in pair.Keys) {
            var ab = k.Split('|');
            participating.Add(ab[0]);
            participating.Add(ab[1]);
        }

        foreach (var t in participating) {
            if (!baseUnit.TryGetValue(t, out var bu)) continue;
            singleWords.TryGetValue(t, out var sw);
            var dec = DecomposeDenom(bu, sw ?? new HashSet<string>());
            string factoryUnit = dec[dec.Count - 1]; // unprefixed base (Grams)
            string sing = Depluralize(factoryUnit);
            if (!tokenType.ContainsKey(sing) && !entryToken.ContainsKey(sing)) entryToken[sing] = (t, factoryUnit);
        }

        if (meta.ContainsKey("Speed")) entryToken["Light"] = ("Speed", "SpeedOfLight");

        // Reachable states via BFS over product transitions from the entry types.
        var states = new HashSet<string>();
        var queue = new Queue<string>();
        foreach (var e in entryToken.Values)
            if (states.Add(e.Type))
                queue.Enqueue(e.Type);
        while (queue.Count > 0) {
            string r = queue.Dequeue();
            foreach (var tt in tokenType.Values)
                foreach (var c in Results(r, tt.Type))
                    if (states.Add(c))
                        queue.Enqueue(c);
        }

        // Value of the running state R (canonical of R, via `field`) times one unit u of type T,
        // expressed in the canonical of result C (coherent product then back to C's stored scale).
        string Combine(string r, string t, string u, string c, string field) {
            string coR = ToCoherentExpr(field, Div(r));
            string coU = ToCoherentExpr($"({Cons(t, t, u, "1", removable)}.CanonicalValue)", Div(t));
            return FromCoherentExpr($"{coR} * {coU}", Div(c));
        }

        // Square/Cubic modifier tries over the Area/Volume units, so a running state R can be scaled
        // by an areal/cubic unit via R × Area / R × Volume (e.g., Mass.Square.Meters → MomentOfInertia).
        var modTrie =
            new Dictionary<string, (string Partner, Dictionary<string, string> Term,
                Dictionary<string, SortedSet<string>> Children, List<string> Nodes)>();
        var bases = SingleWordBases(all);
        foreach (var (mod, partner) in new[] { ("Square", "Area"), ("Cubic", "Volume") }) {
            if (!meta.ContainsKey(partner)) continue;
            var term = new Dictionary<string, string>();
            var childs = new Dictionary<string, SortedSet<string>>();
            var nds = new HashSet<string> { "" };
            var pinfo = all.First(a => a.Name == partner);
            foreach (var u in EnumerateUnitNames(pinfo.Units)) {
                var ws = SplitWords(u);
                if (ws.Count < 2 || ws[0] != mod) continue;
                var rest = new List<string>();
                for (int i = 1; i < ws.Count; i++) rest.AddRange(DecomposeDenom(ws[i], bases));
                string key = string.Join("_", rest);
                if (!term.ContainsKey(key)) term[key] = u;
                for (int k = 0; k <= rest.Count; k++) {
                    string p = string.Join("_", rest.GetRange(0, k));
                    nds.Add(p);
                    if (k < rest.Count) {
                        if (!childs.TryGetValue(p, out var set))
                            childs[p] = set = new SortedSet<string>(StringComparer.Ordinal);
                        set.Add(rest[k]);
                    }
                }
            }

            var ordered = new List<string>(nds);
            ordered.Sort(StringComparer.Ordinal);
            modTrie[mod] = (partner, term, childs, ordered);
        }

        var structs = new StringBuilder();
        var selectors = new StringBuilder();
        var seenSel = new HashSet<string>();
        var modStructs = new StringBuilder();
        foreach (var r in states.OrderBy(s => s, StringComparer.Ordinal)) {
            string nn = "ProductState_" + r;
            structs.Append($"    public readonly struct {nn} {{\n");
            structs.Append($"        internal readonly double c;\n");
            structs.Append($"        internal {nn}(double c) => this.c = c;\n");
            structs.Append($"        public static implicit operator {r}({nn} n) => {r}.FromCanonical(n.c);\n");
            // transitions, ordered by token
            foreach (var tok in tokenType.Keys.OrderBy(s => s, StringComparer.Ordinal)) {
                var (tt, tu) = tokenType[tok];
                var res = Results(r, tt);
                if (res.Count == 0) continue;
                if (res.Count == 1) {
                    string c = res[0];
                    structs.Append(
                        $"        public ProductState_{c} {tok} => new({Combine(r, tt, tu, c, "c")});\n");
                } else {
                    string sel = $"Sel_{r}_{tok}";
                    structs.Append($"        public {sel} {tok} => new(c);\n");
                    if (seenSel.Add(sel)) {
                        selectors.Append(
                            $"    public readonly struct {sel} {{\n        internal readonly double c;\n        internal {sel}(double c) => this.c = c;\n");
                        string prim = PrimaryOf(r, tt);
                        if (prim.Length != 0)
                            selectors.Append(
                                $"        public static implicit operator {prim}({sel} n) => global::{ns}.{prim}.FromCanonical({Combine(r, tt, tu, prim, "n.c")});\n");
                        foreach (var c in res)
                            selectors.Append(
                                $"        public ProductState_{c} {c} => new({Combine(r, tt, tu, c, "c")});\n");
                        selectors.Append("    }\n");
                    }
                }
            }

            // Square/Cubic modifiers: R × Area / R × Volume, when such a product exists.
            foreach (var mod in new[] { "Square", "Cubic" }) {
                if (!modTrie.TryGetValue(mod, out var mt) || Results(r, mt.Partner).Count == 0) continue;
                structs.Append($"        public {mod}Of_{r} {mod} => new(c);\n");
                foreach (var p in mt.Nodes) {
                    string mn = $"{mod}Of_{r}" + (p.Length == 0 ? "" : "_" + p);
                    modStructs.Append(
                        $"    public readonly struct {mn} {{\n        internal readonly double c;\n        internal {mn}(double c) => this.c = c;\n");
                    if (mt.Children.TryGetValue(p, out var set))
                        foreach (var w in set) {
                            string cp = p.Length == 0 ? w : p + "_" + w;
                            modStructs.Append($"        public {mod}Of_{r}_{cp} {w} => new(c);\n");
                        }

                    if (p.Length > 0 && mt.Term.TryGetValue(p, out var au)) {
                        var res = Results(r, mt.Partner);
                        if (res.Count == 1)
                            modStructs.Append(
                                $"        public static implicit operator {res[0]}({mn} n) => {res[0]}.FromCanonical({Combine(r, mt.Partner, au, res[0], "n.c")});\n");
                        else
                            foreach (var c in res)
                                modStructs.Append(
                                    $"        public ProductState_{c} {c} => new({Combine(r, mt.Partner, au, c, "c")});\n");
                    }

                    modStructs.Append("    }\n");
                }
            }

            structs.Append("    }\n");
        }

        structs.Append(modStructs);

        // Entry properties on Prefixed and double.
        var prefixed = new StringBuilder();
        var dbl = new StringBuilder();
        foreach (var tok in entryToken.Keys.OrderBy(s => s, StringComparer.Ordinal)) {
            var (ty, un) = entryToken[tok];
            prefixed.Append(
                $"        public ProductState_{ty} {tok} => new({Cons(ty, ty, un, "p.Value", removable)}.CanonicalValue);\n");
            dbl.Append(
                $"        public ProductState_{ty} {tok} => new({Cons(ty, ty, un, "value", removable)}.CanonicalValue);\n");
        }

        string src = $@"// <auto-generated/>
#nullable disable

namespace {ns} {{
    public static partial class Units {{
        extension(Prefixed p) {{
{prefixed}        }}
    }}
{structs}{selectors}}}

namespace {ns}.Fluent {{
    using {ns};
    public static partial class DoubleSugar {{
        extension(double value) {{
{dbl}        }}
    }}
}}
";
        ctx.AddSource("_Product.g.cs", SourceText.From(src, Encoding.UTF8));
    }
}
