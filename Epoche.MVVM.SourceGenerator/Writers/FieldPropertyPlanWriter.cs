using Epoche.MVVM.SourceGenerator.Plans;

namespace Epoche.MVVM.SourceGenerator.Writers;
static class FieldPropertyPlanWriter
{
    public static string Properties(ClassPlan plan) => string.Concat(plan.FieldPropertiesPlans.Select(Property));

    static string Property(ClassPlan.FieldPropertyPlan plan) => $@"
    public {plan.FullTypeName} {plan.PropertyName}{GetSet(plan)}
";

    static string GetSet(ClassPlan.FieldPropertyPlan plan) =>
        plan.IsReadOnly ? $" => this.{plan.FieldName};" : $@"
    {{
        get => this.{plan.FieldName};
        {plan.SetterModifier.AddSpace()}set{(plan.AffectedProperties.Where(x => x != plan.PropertyName).Any() ? ConditionalSetter(plan) : SetterExpression(plan))}
    }}
";

    static string ConditionalSetter(ClassPlan.FieldPropertyPlan plan) => $@"
        {{
            if ({Set(plan)})
            {{
                {string.Concat(plan.AffectedProperties.Where(x => x != plan.PropertyName).Select(AffectedProperty)).Up()}
            }}
        }}";

    static string SetterExpression(ClassPlan.FieldPropertyPlan plan) => $@" => {Set(plan)};";
    
    static string Set(ClassPlan.FieldPropertyPlan plan) => $@"this.Set(
                ref this.{plan.FieldName}, 
                value,
                equalityComparer: {plan.EqualityComparer ?? "null"},
                onChange: {plan.OnChange ?? "null"},
                trackChanges: {(plan.TrackChanges ? "true" : "false")},
                cachedPropertyChangedEventArgs: Epoche.MVVM.CachedPropertyChangeEventArgs.{plan.PropertyName})";

    static string AffectedProperty(string propertyName) => $@"
                this.RaisePropertyChanged(Epoche.MVVM.CachedPropertyChangeEventArgs.{propertyName});";

    public static string FactoryInitialize(ClassPlan plan) => string.Concat(plan.FieldPropertiesPlans.Select(FactoryInitialize));
    public static string InitializeExpressions(ClassPlan plan) => string.Concat(plan.FieldPropertiesPlans.Select(InitializeExpressions));

    static string FactoryInitialize(ClassPlan.FieldPropertyPlan plan) => plan.FactoryConstructorArg is null ? "" : $@"
        this.{plan.FieldName} = {plan.FactoryConstructorArg.ArgumentName}?.Create() ?? throw new System.ArgumentNullException(nameof({plan.FactoryConstructorArg.ArgumentName}));";
    static string InitializeExpressions(ClassPlan.FieldPropertyPlan plan) => plan.FactoryInitializerExpression is null ? "" : $@"
        this.{plan.FieldName}.Initialize({plan.FactoryInitializerExpression});";
}
