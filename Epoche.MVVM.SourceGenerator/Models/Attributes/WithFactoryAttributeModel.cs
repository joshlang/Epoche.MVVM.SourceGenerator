namespace Epoche.MVVM.SourceGenerator.Models.Attributes;
class WithFactoryAttributeModel
{
    public AttributeData AttributeData = default!;

    public string? InterfaceName;
    public string? FactoryName;
    public string? InterfaceAccessModifier;
    public string? FactoryAccessModifier;
    public bool UseInterfaceNameDefault = true;
    public bool UseFactoryNameDefault = true;
}
