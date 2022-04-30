using Epoche.MVVM.SourceGenerator.Models;
using Epoche.MVVM.SourceGenerator.Models.Attributes;

namespace Epoche.MVVM.SourceGenerator.Builders.Attributes;
static class InjectAttributeModelBuilder
{
    public static void Build(OutputModel outputModel, ClassModel classModel, AttributeData attributeData)
    {
        var model = new InjectAttributeModel
        {
            AttributeData = attributeData
        };

        if (!attributeData.ConstructorArguments.IsDefaultOrEmpty)
        {
            var arg = attributeData.ConstructorArguments[0].Value;
            model.Type = (arg as ITypeSymbol)?.ToDisplayString() ?? (arg as string)!;
        }
        foreach (var named in attributeData.NamedArguments)
        {
            switch (named.Key)
            {
                case "Type":
                    model.Type = (named.Value.Value as ITypeSymbol)?.ToDisplayString()!;
                    break;
                case "TypeName":
                    model.Type = named.Value.Value as string ?? model.Type;
                    break;
                case "Name":
                    model.Name = named.Value.Value as string;
                    break;
                case "AccessModifier":
                    model.AccessModifier = named.Value.Value as string;
                    break;
            }
        }
        
        if (model.Type is null)
        {
            outputModel.Context.Report(Diagnostics.Errors.InjectMissingType, attributeData.AttributeConstructor);
            return;
        }

        classModel.InjectAttributes.Add(model);
    }
}
