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
        {plan.SetterModifier.AddSpace()}set
        {{
            if (this.Set(
                ref this.{plan.FieldName}, 
                value,
                equalityComparer: {plan.EqualityComparer ?? "null"},
                onChange: {plan.OnChange ?? "null"},
                trackChanges: {(plan.TrackChanges ? "true" : "false")},
                cachedPropertyChangedEventArgs: Epoche.MVVM.CachedPropertyChangeEventArgs.{plan.PropertyName}))
            {{
                {string.Concat(plan.AffectedProperties.Where(x => x != plan.PropertyName).Select(AffectedProperty)).Up()}
                {string.Concat(plan.AffectedCommands.Select(AffectedCommand)).Up()}
            }}
        }}
    }}
";

    static string AffectedProperty(string propertyName) => $@"
                this.RaisePropertyChanged(Epoche.MVVM.CachedPropertyChangeEventArgs.{propertyName});";

    static string AffectedCommand(string commandName) => $@"
                this.{commandName}.RaiseCanExecuteChanged();";

    public static string FactoryInitialize(ClassPlan plan) => string.Concat(plan.FieldPropertiesPlans.Select(FactoryInitialize));

    static string FactoryInitialize(ClassPlan.FieldPropertyPlan plan) => plan.FactoryConstructorArg is null ? "" : $@"
        this.{plan.FieldName} = {plan.FactoryConstructorArg.ArgumentName}?.Create() ?? throw new System.ArgumentNullException(nameof({plan.FactoryConstructorArg.ArgumentName}));";
}
