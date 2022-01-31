using Epoche.MVVM.SourceGenerator.Plans;

namespace Epoche.MVVM.SourceGenerator.Writers;
class FactoryInterfacePlanWriter
{
    public static void Write(OutputPlan outputPlan, ClassPlan plan)
    {
        outputPlan.CancellationToken.ThrowIfCancellationRequested();

        if (plan.Factory.InterfaceName is null) { return; }

        outputPlan.Context.AddSource($"{plan.Factory.InterfaceName}.g.cs", FactoryInterface(plan));
    }

    static string FactoryInterface(ClassPlan plan) => $@"
#nullable enable
namespace {plan.Namespace};
{plan.Factory.InterfaceAccessModifier.AddSpace()}interface {plan.Factory.InterfaceName}
{{
    {plan.FullClassName} Create();
}}
";
}
