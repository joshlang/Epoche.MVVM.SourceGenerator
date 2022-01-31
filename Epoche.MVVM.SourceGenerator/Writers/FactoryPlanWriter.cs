using Epoche.MVVM.SourceGenerator.Plans;

namespace Epoche.MVVM.SourceGenerator.Writers;
class FactoryPlanWriter
{
    public static void Write(OutputPlan outputPlan, ClassPlan plan)
    {
        outputPlan.CancellationToken.ThrowIfCancellationRequested();

        if (plan.Factory.FactoryName is null) { return; }

        outputPlan.Context.AddSource($"{plan.Factory.FactoryName}.g.cs", Factory(plan));
    }

    static string Factory(ClassPlan plan) => $@"
#nullable enable
namespace {plan.Namespace};
{plan.Factory.FactoryAccessModifier.AddSpace()}class {plan.Factory.FactoryName}{(plan.Factory.InterfaceName is null ? "" : $" : {plan.Factory.FullInterfaceName}")}
{{
    public {plan.Factory.FactoryName}({string.Join(",", plan.ConstructorArguments.Select(ConstructorArg))})
    {{
        {string.Concat(plan.ConstructorArguments.Select(Assign)).Up()}
    }}
    {string.Concat(plan.ConstructorArguments.Select(Property))}

    public {plan.FullClassName} Create() => new({string.Join(", ", plan.ConstructorArguments.Select(x => x.ArgumentName))});
}}
";

    static string ConstructorArg(ClassPlan.ConstructorArg arg) => $@"
        {arg.FullTypeName} {arg.ArgumentName}";

    static string Property(ClassPlan.ConstructorArg arg) => $@"
    readonly {arg.FullTypeName} {arg.ArgumentName};";

    static string Assign(ClassPlan.ConstructorArg arg) => $@"
        this.{arg.ArgumentName} = {arg.ArgumentName} ?? throw new System.ArgumentNullException(nameof({arg.ArgumentName}));";
}
