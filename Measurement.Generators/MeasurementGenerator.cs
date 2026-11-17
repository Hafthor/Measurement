using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Measurement.Generators.Utils;

namespace Measurement.Generators;

// Emits the identical-across-every-measurement boilerplate (operators, equality, comparison,
// formatting, math utilities, and the IMeasurement<T> / System.Numerics surface) into each
// [Measurement]-annotated readonly partial struct, so the handwritten source only needs the
// unit-specific From/To methods and cross-type operators.
[Generator]
public sealed class MeasurementGenerator : IIncrementalGenerator {
    public void Initialize(IncrementalGeneratorInitializationContext context) {
        var types = context.SyntaxProvider.ForAttributeWithMetadataName(
            AttrMetadataName,
            predicate: static (node, _) => node is StructDeclarationSyntax,
            transform: static (ctx, _) => {
                var type = (INamedTypeSymbol)ctx.TargetSymbol;
                var a0 = ctx.Attributes[0];
                var symbol = a0.ConstructorArguments.Length > 0
                    ? a0.ConstructorArguments[0].Value as string ?? ""
                    : "";

                double displayFactor = 1;
                string variableName = "value";
                foreach (var na in a0.NamedArguments)
                    if (na is { Key: "DisplayFactor", Value.Value: double d })
                        displayFactor = d;
                    else if (na is { Key: "VariableName", Value.Value: string s })
                        variableName = s;

                List<IUnitSpec> unitSpecs = type.GetAttributes()
                    .Select(FromAttribute)
                    .Where(spec => spec is not null)
                    .ToList();

                // [Product<TLeft, TRight>] relations: Name = Left × Right (Primary marks the result a
                // selector implicitly converts to when the product is shared by several types).
                List<ProductSpec> products = type.GetAttributes()
                    .Select(ProductSpec.FromAttribute)
                    .Where(spec => spec.HasValue)
                    .Select(spec => spec.Value)
                    .ToList();

                // [Reciprocal<TPartner>(Name=…)]: this = 1 / TPartner in coherent SI, encoded as
                // "partnerType|readName".
                List<ReciprocalSpec> reciprocals = type.GetAttributes()
                    .Select(ReciprocalSpec.FromAttribute)
                    .Where(spec => spec.HasValue)
                    .Select(spec => spec.Value)
                    .ToList();

                return new MeasurementInfo(type.Name, type.ContainingNamespace.ToDisplayString(), symbol,
                    displayFactor, variableName, unitSpecs, products, reciprocals);
            });

        // Struct body: identical-per-type boilerplate plus the unit From/To methods.
        context.RegisterSourceOutput(types, MeasurementGenerator_Base.EmitBase);

        var collected = types.Collect();
        Action<SourceProductionContext, ImmutableArray<MeasurementInfo>>[] emitters = [
            MeasurementGenerator_Fluent.EmitFluentSet,
            MeasurementGenerator_Products.EmitProducts,
            MeasurementGenerator_PerBuilders.EmitPerBuilders,
            MeasurementGenerator_PerReadBuilders.EmitPerReadBuilders,
            MeasurementGenerator_TokenAlgebra.EmitTokenAlgebra,
            MeasurementGenerator_SquareCubic.EmitSquareCubic,
            MeasurementGenerator_SquareCubicRead.EmitSquareCubicRead,
            MeasurementGenerator_TypedEntry.EmitTypedEntry,
            MeasurementGenerator_Reciprocals.EmitReciprocals
        ];
        foreach (var emitter in emitters)
            context.RegisterSourceOutput(collected, emitter);
    }
}
