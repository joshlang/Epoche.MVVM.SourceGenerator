using Epoche.MVVM.SourceGenerator.Models;
using Epoche.MVVM.SourceGenerator.Models.Attributes;

namespace Epoche.MVVM.SourceGenerator.Builders.Attributes;
static class PropertyAttributeModelBuilder
{
    public static void Build(OutputModel outputModel, FieldModel fieldModel, AttributeData attributeData)
    {
        var model = new PropertyAttributeModel
        {
            AttributeData = attributeData
        };

        if (!attributeData.ConstructorArguments.IsDefaultOrEmpty)
        {
            model.Name = attributeData.ConstructorArguments[0].Value as string;
        }
        foreach (var named in attributeData.NamedArguments)
        {
            switch (named.Key)
            {
                case "Name":
                    model.Name = named.Value.Value as string;
                    break;
                case "TrackChanges":
                    model.TrackChanges = (bool?)named.Value.Value ?? model.TrackChanges;
                    break;
                case "SetterModifier":
                    model.SetterModifier = named.Value.Value as string;
                    break;
                case "OnChange":
                    model.OnChange = named.Value.Value as string;
                    break;
                case "EqualityComparer":
                    model.EqualityComparer = named.Value.Value as string;
                    break;
            }
        }

        fieldModel.PropertyAttribute = model;
    }
}
