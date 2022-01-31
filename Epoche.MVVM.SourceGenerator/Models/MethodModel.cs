using Epoche.MVVM.SourceGenerator.Models.Attributes;

namespace Epoche.MVVM.SourceGenerator.Models;
class MethodModel
{
    public string MethodName = default!;
    public bool IsAsync;
    public string? AsyncReturnType;
    public string? CommandParameterType;

    public CommandAttributeModel? CommandAttribute;
}
