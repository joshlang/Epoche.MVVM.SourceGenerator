using Epoche.MVVM.SourceGenerator.Plans;

namespace Epoche.MVVM.SourceGenerator.Writers;
static class InjectedPropertyPlanWriter
{
    public static string Properties(ClassPlan plan) => string.Concat(plan.InjectedPropertyPlans.Select(Property));

    static string Property(ClassPlan.InjectedPropertyPlan plan) => $@"
    {plan.PropertyAccessModifier.AddSpace()}{plan.FullTypeName} {plan.PropertyName} {{ get; }}";

    public static string Assign(ClassPlan plan) => string.Concat(plan.InjectedPropertyPlans.Select(Assign));

    public static string Assign(ClassPlan.InjectedPropertyPlan plan) => $@"
        this.{plan.PropertyName} = {plan.ConstructorArg.ArgumentName} ?? throw new System.ArgumentNullException(nameof({plan.ConstructorArg.ArgumentName}));";
}
