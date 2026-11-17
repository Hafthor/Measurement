using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using static Measurement.Generators.Utils;

namespace Measurement.Generators;

// Cross-type operators from [Product] relations: needs every type's symbol + DisplayFactor.
public class MeasurementGenerator_Products {
    // Emits every cross-type operator implied by the [Product] relations. For C = A × B it writes
    // A*B, B*A → C and C/A → B, C/B → A, each with an exact factor derived from the symbols'
    // gram-power and DisplayFactor, placed in a partial of one of its parameter types.
    public static void EmitProducts(SourceProductionContext ctx, ImmutableArray<MeasurementInfo> all) {
        var meta = new Dictionary<string, (double DF, int G)>();
        string ns = ResolveNamespace(all);
        foreach (var info in all) meta[info.Name] = (info.DisplayFactor, GramPower(info.Symbol));

        // container type → operator source lines.
        var byContainer = new Dictionary<string, StringBuilder>();

        StringBuilder Body(string container) {
            if (!byContainer.TryGetValue(container, out var sb)) byContainer[container] = sb = new StringBuilder();
            return sb;
        }

        // Each type's coherent-SI divisor. `Quantity` stores an Avogadro count, but in products it
        // acts as an amount of substance in moles, so normalize its dimension to moles (as the `.Per`
        // builders do) — otherwise molar results (Molality, Concentration, MolarMass, …) would be off
        // by Avogadro. All other types convert through the shared power-of-ten coherent machinery.
        (int Ten, double Res) Div(string t) => CoherentDivisor(meta[t].DF, meta[t].G);
        bool hasMole = meta.ContainsKey("Quantity");

        string CoherentVal(string t, string val) =>
            t == "Quantity" && hasMole
                ? $"({val} / (Quantity.FromMoles(1).CanonicalValue))"
                : ToCoherentExpr(val, Div(t));

        string FromCoherentVal(string t, string expr) =>
            t == "Quantity" && hasMole
                ? $"({expr} * (Quantity.FromMoles(1).CanonicalValue))"
                : FromCoherentExpr(expr, Div(t));

        // Gather every operator "intent" keyed by its C# signature; a signature that resolves to more
        // than one result type (a dimensionally ambiguous product like Force×Length → Energy or
        // Torque) is emitted as a selector instead of colliding.
        var intents =
            new Dictionary<string, (string Container, string Op, string P1, string P2, string Expr,
                List<(string Name, bool Primary)> Results)>();

        void Intent(string container, string op, string p1, string p2, string expr, string result, bool primary) {
            string sig = $"{container}|{op}|{p1}|{p2}";
            if (!intents.TryGetValue(sig, out var it))
                intents[sig] = it = (container, op, p1, p2, expr, new List<(string, bool)>());
            if (!it.Results.Exists(x => x.Name == result)) it.Results.Add((result, primary));
        }

        foreach (var info in all) {
            if (info.Products.Count == 0) continue;

            string c = info.Name;
            if (!meta.ContainsKey(c)) continue;

            foreach (var product in info.Products) {
                string aa = product.LeftName, bb = product.RightName;
                bool primary = product.IsPrimary;
                if (!meta.ContainsKey(aa) || !meta.ContainsKey(bb)) continue;

                // C = A × B and B × A — coherent product of the two operands.
                Intent(aa, "*", aa, bb,
                    $"{CoherentVal(aa, "a.CanonicalValue")} * {CoherentVal(bb, "b.CanonicalValue")}", c, primary);
                if (aa != bb)
                    Intent(bb, "*", bb, aa,
                        $"{CoherentVal(bb, "a.CanonicalValue")} * {CoherentVal(aa, "b.CanonicalValue")}", c,
                        primary);
                // A = C / B, and B = C / A — coherent quotient (a is C, b is the divisor).
                Intent(c, "/", c, aa,
                    $"{CoherentVal(c, "a.CanonicalValue")} / {CoherentVal(aa, "b.CanonicalValue")}", bb, false);
                if (aa != bb)
                    Intent(c, "/", c, bb,
                        $"{CoherentVal(c, "a.CanonicalValue")} / {CoherentVal(bb, "b.CanonicalValue")}", aa, false);
            }
        }

        // Emit each intent: a single result → a direct operator; multiple → a selector operator plus
        // a selector struct whose type-named properties pick the intended result (f×l).Energy / .Torque.
        var selectors = new Dictionary<string, List<(string Name, bool Primary)>>();

        string SelName(string op, string p1, string p2) =>
            op == "*"
                ? string.CompareOrdinal(p1, p2) <= 0 ? $"{p1}{p2}Product" : $"{p2}{p1}Product"
                : $"{p1}Over{p2}Quotient";

        foreach (var it in intents.Values) {
            if (it.Results.Count == 1) {
                string r = it.Results[0].Name;
                Body(it.Container)
                    .Append(
                        $"    public static {r} operator {it.Op}({it.P1} a, {it.P2} b) => {r}.FromCanonical({FromCoherentVal(r, it.Expr)});\n");
            } else {
                string sel = SelName(it.Op, it.P1, it.P2);
                if (!selectors.ContainsKey(sel)) selectors[sel] = it.Results;
                Body(it.Container)
                    .Append($"    public static {sel} operator {it.Op}({it.P1} a, {it.P2} b) => new({it.Expr});\n");
            }
        }

        if (byContainer.Count == 0) return;
        var src = new StringBuilder("// <auto-generated/>\n#nullable disable\n\nnamespace " + ns + " {\n");
        foreach (var kv in selectors) {
            var results = kv.Value;
            string primary = results.Find(x => x.Primary).Name; // null when none is marked
            var names = new List<string>();
            foreach (var r in results) names.Add(r.Name);
            src.Append(primary != null
                ? $"    // Ambiguous product — implicitly a {primary}; also (a * b).{string.Join(" / (a * b).", names)}\n"
                : $"    // Ambiguous product — pick the result: (a * b).{string.Join(" / (a * b).", names)}\n");
            src.Append($"    public readonly struct {kv.Key} {{\n");
            src.Append("        private readonly double v;\n");
            src.Append($"        internal {kv.Key}(double v) => this.v = v;\n");
            foreach (var r in names)
                src.Append($"        public {r} {r} => {r}.FromCanonical({FromCoherentVal(r, "v")});\n");
            if (primary != null)
                src.Append($"        public static implicit operator {primary}({kv.Key} s) => s.{primary};\n");
            src.Append("    }\n");
        }

        foreach (var kv in byContainer) {
            src.Append($"    public readonly partial struct {kv.Key} {{\n");
            src.Append(kv.Value);
            src.Append("    }\n");
        }

        src.Append("}\n");
        ctx.AddSource("_Operators.g.cs", SourceText.From(src.ToString(), Encoding.UTF8));
    }
}
