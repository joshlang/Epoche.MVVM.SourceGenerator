using Epoche.MVVM.SourceGenerator.Models;
using Epoche.MVVM.SourceGenerator.Models.Attributes;

namespace Epoche.MVVM.SourceGenerator.Builders.Attributes;
static class ChangedByAttributeModelBuilder
{
    public static void Build(OutputModel outputModel, PropertyModel propertyModel, AttributeData attributeData)
    {
        var model = new ChangedByAttributeModel
        {
            AttributeData = attributeData
        };
        
        if (!attributeData.ConstructorArguments.IsDefaultOrEmpty)
        {
            model.Properties = attributeData.ConstructorArguments[0].Values.Select(x => x.Value as string).Where(x => x is not null).ToArray()!;
        }
        foreach (var named in attributeData.NamedArguments)
        {
            switch (named.Key)
            {
                case "Properties":
                    model.Properties = named.Value.Values.Select(x => x.Value as string).Where(x => x is not null).ToArray()!;
                    break;
            }
        }

        propertyModel.ChangedByAttribute = model;
    }
}
