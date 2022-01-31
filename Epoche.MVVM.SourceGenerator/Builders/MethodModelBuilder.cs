using Epoche.MVVM.SourceGenerator.Builders.Attributes;
using Epoche.MVVM.SourceGenerator.Models;

namespace Epoche.MVVM.SourceGenerator.Builders;
static class MethodModelBuilder
{
    public static void Build(OutputModel outputModel, ClassModel classModel, IMethodSymbol symbol)
    {
        outputModel.CancellationToken.ThrowIfCancellationRequested();

        var model = new MethodModel
        {
            MethodName = symbol.Name
        };

        foreach (var attributeData in symbol.GetAttributes())
        {
            if (SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, outputModel.CommandAttributeSymbol))
            {
                CommandAttributeModelBuilder.Build(outputModel, classModel, model, attributeData);
            }
        }

        if (model.CommandAttribute is null)
        {
            return;
        }
        if (!symbol.Parameters.IsDefaultOrEmpty && symbol.Parameters.Length > 1)
        {
            outputModel.Context.Report(Diagnostics.Errors.MultipleCommandParameters, symbol);
            return;
        }

        model.IsAsync = symbol.ReturnType.ToDisplayString().StartsWith("System.Threading.Tasks.Task");
        if (model.IsAsync && symbol.ReturnType is INamedTypeSymbol { TypeArguments.IsDefaultOrEmpty: false } returnSymbol)
        {
            model.AsyncReturnType = returnSymbol.TypeArguments[0].ToDisplayString();
        }
        if (!symbol.Parameters.IsDefaultOrEmpty)
        {
            model.CommandParameterType = symbol.Parameters[0].Type.ToDisplayString();
        }
        
        classModel.MethodModels.Add(model);
    }
}
