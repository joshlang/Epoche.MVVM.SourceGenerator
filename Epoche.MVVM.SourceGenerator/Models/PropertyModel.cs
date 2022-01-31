using Epoche.MVVM.SourceGenerator.Models.Attributes;

namespace Epoche.MVVM.SourceGenerator.Models;
class PropertyModel
{
    public string PropertyName = default!;

    public ChangedByAttributeModel? ChangedByAttribute;
}
