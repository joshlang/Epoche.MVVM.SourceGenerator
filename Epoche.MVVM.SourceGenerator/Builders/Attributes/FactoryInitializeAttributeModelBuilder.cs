using Epoche.MVVM.SourceGenerator.Models;
using Epoche.MVVM.SourceGenerator.Models.Attributes;

namespace Epoche.MVVM.SourceGenerator.Builders.Attributes;
static class FactoryInitializeAttributeModelBuilder
{
    public static void Build(OutputModel outputModel, FieldModel fieldModel, AttributeData attributeData)
    {
        var model = new FactoryInitializeAttributeModel
        {
            AttributeData = attributeData
        };

        if (!attributeData.ConstructorArguments.IsDefaultOrEmpty)
        {
            model.Type = (attributeData.ConstructorArguments[0].Value as ITypeSymbol)?.ToDisplayString()!;
        }
        foreach (var named in attributeData.NamedArguments)
        {
            switch (named.Key)
            {
                case "Type":
                    model.Type = (named.Value.Value as ITypeSymbol)?.ToDisplayString()!;
                    break;
            }
        }

        fieldModel.FactoryInitializeAttribute = model;
    }
}
