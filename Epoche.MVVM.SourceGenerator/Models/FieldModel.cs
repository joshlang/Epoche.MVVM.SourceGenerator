using Epoche.MVVM.SourceGenerator.Models.Attributes;

namespace Epoche.MVVM.SourceGenerator.Models;
class FieldModel
{
    public string FieldName = default!;
    public string FullTypeName = default!;
    public string TypeName = default!;
    public string? TypeNamespace;
    public bool IsReadOnly;

    public PropertyAttributeModel? PropertyAttribute;
    public FactoryInitializeAttributeModel? FactoryInitializeAttribute;
}
