namespace Epoche.MVVM.SourceGenerator.Models.Attributes;
class CommandAttributeModel
{
    public AttributeData AttributeData = default!;

    public string? Name;
    public bool AllowConcurrency;
    public string? CanExecute;
    public string? TaskName;
    public bool UseDefaultTaskName = true;
}
