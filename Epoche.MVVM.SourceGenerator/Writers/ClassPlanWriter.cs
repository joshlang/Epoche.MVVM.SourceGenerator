using Epoche.MVVM.SourceGenerator.Models;
using Epoche.MVVM.SourceGenerator.Plans;

namespace Epoche.MVVM.SourceGenerator.Writers;
static class ClassPlanWriter
{
    public static void Write(OutputPlan outputPlan, ClassPlan plan)
    {
        outputPlan.CancellationToken.ThrowIfCancellationRequested();

        outputPlan.Context.AddSource($"{plan.ClassName}.g.cs", Class(plan));
        FactoryInterfacePlanWriter.Write(outputPlan, plan);
        FactoryPlanWriter.Write(outputPlan, plan);
    }

    static string Class(ClassPlan plan) => $@"
#nullable enable
namespace {plan.Namespace};
partial class {plan.ClassName}
{{
    public {plan.ClassName}({string.Join(",", plan.ConstructorArguments.Select(ConstructorArg))}){BaseArgs(plan)}
    {{
        {FieldPropertyPlanWriter.FactoryInitialize(plan).Up()}
        {CommandPlanWriter.Create(plan)}
        {InjectedPropertyPlanWriter.Assign(plan)}
        this.Init();
    }}
    
    partial void Init();    
    {InjectedPropertyPlanWriter.Properties(plan)}
    {CommandPlanWriter.Properties(plan)}
    {FieldPropertyPlanWriter.Properties(plan)}
}}";

    static string ConstructorArg(ClassPlan.ConstructorArg arg) => $@"
        {arg.FullTypeName} {arg.ArgumentName}";

    static string BaseArgs(ClassPlan plan) => plan.BaseArguments.Count == 0 ? "" : $" : base({string.Join(", ", plan.BaseArguments)})";
}
