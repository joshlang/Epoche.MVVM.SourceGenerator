using Epoche.MVVM.SourceGenerator.Models;
using Epoche.MVVM.SourceGenerator.Models.Attributes;

namespace Epoche.MVVM.SourceGenerator.Builders.Attributes;
static class UseSourceGenAttributeModelBuilder
{
    public static void Build(OutputModel outputModel, ClassModel classModel, AttributeData attributeData) => classModel.UseSourceGenAttribute = new UseSourceGenAttributeModel
    {
        AttributeData = attributeData
    };
}
