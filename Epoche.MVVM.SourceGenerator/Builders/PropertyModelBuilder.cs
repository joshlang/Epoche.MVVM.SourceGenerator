using Epoche.MVVM.SourceGenerator.Builders.Attributes;
using Epoche.MVVM.SourceGenerator.Models;

namespace Epoche.MVVM.SourceGenerator.Builders;
static class PropertyModelBuilder
{
    public static void Build(OutputModel outputModel, ClassModel classModel, IPropertySymbol symbol)
    {
        outputModel.CancellationToken.ThrowIfCancellationRequested();

        var model = new PropertyModel
        {
            PropertyName = symbol.Name
        };

        foreach (var attributeData in symbol.GetAttributes())
        {
            if (SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, outputModel.ChangedByAttributeSymbol))
            {
                ChangedByAttributeModelBuilder.Build(outputModel, model, attributeData);
            }
        }

        if (model.ChangedByAttribute is null)
        {
            return;
        }

        classModel.PropertyModels.Add(model);
    }
}
