namespace Epoche.MVVM.SourceGenerator.Models.Attributes;
class PropertyAttributeModel
{
    public AttributeData AttributeData = default!;

    public bool TrackChanges = true;
    public string? SetterModifier;
    public string? Name;
    public string? OnChange;
    public string? EqualityComparer;
}
