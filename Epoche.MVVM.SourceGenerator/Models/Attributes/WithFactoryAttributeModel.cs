namespace Epoche.MVVM.SourceGenerator.Models.Attributes;
class WithFactoryAttributeModel
{
    public AttributeData AttributeData = default!;

    public string? FactoryName;
    public string? FactoryAccessModifier;
    public bool UseFactoryNameDefault = true;
}
