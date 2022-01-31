using Epoche.MVVM.SourceGenerator.Plans;

namespace Epoche.MVVM.SourceGenerator.Writers;
static class CommandPlanWriter
{
    public static string Properties(ClassPlan plan) => string.Concat(
        string.Concat(plan.CommandPlans.Select(Property)),
        string.Concat(plan.CommandPlans.Select(CommandTaskProperty))
        );

    static string Property(ClassPlan.CommandPlan plan) => $@"
    public Epoche.MVVM.ViewModels.DelegateCommand{(plan.HasParameter ? $"<{plan.ParameterFullTypeName}>" : "")} {plan.PropertyName} {{ get; }}";

    static string CommandTaskProperty(ClassPlan.CommandPlan plan) => plan.TaskPropertyName is null ? "" : $@"
    Epoche.MVVM.Models.NotifyTask{(plan.AsyncReturnFullTypeName is null ? "" : $"<{plan.AsyncReturnFullTypeName}>")}? {plan.TaskFieldName};
    public Epoche.MVVM.Models.NotifyTask{(plan.AsyncReturnFullTypeName is null ? "" : $"<{plan.AsyncReturnFullTypeName}>")}? {plan.TaskPropertyName}
    {{
        get => this.{plan.TaskFieldName};
        set => this.Set(ref {plan.TaskFieldName}, value, trackChanges: false, cachedPropertyChangedEventArgs: Epoche.MVVM.CachedPropertyChangeEventArgs.{plan.TaskPropertyName});
    }}
";

    public static string Create(ClassPlan plan) => string.Concat(plan.CommandPlans.Select(Create));

    static string Create(ClassPlan.CommandPlan plan) => $@"
        this.{plan.PropertyName} = new({CreateCommandExecute(plan)},{CreateCanCommandExecute(plan)});
";

    static string CreateCommandExecute(ClassPlan.CommandPlan plan) =>
        !plan.IsAsync ? $"this.{plan.MethodName}" :
        plan.AllowConcurrency ? (plan.HasParameter ? $"x => this.{plan.MethodName}(x)" : $"() => this.{plan.MethodName}()") :
        $@"
            async {(plan.HasParameter ? "x" : "()")} => 
            {{
                if (this.{plan.TaskPropertyName}?.IsCompleted == false) {{ return; }}
                var task = this.{plan.MethodName}({(plan.HasParameter ? "x" : "")});
                var notify = new Epoche.MVVM.Models.NotifyTask{(plan.AsyncReturnFullTypeName is null ? "" : $"<{plan.AsyncReturnFullTypeName}>")}(task);
                this.{plan.TaskPropertyName} = notify;
                this.{plan.PropertyName}!.RaiseCanExecuteChanged();
                await notify.TaskCompletion;
                this.{plan.PropertyName}!.RaiseCanExecuteChanged();
            }}";

    static string CreateCanCommandExecute(ClassPlan.CommandPlan plan) =>
        !plan.IsAsync || plan.AllowConcurrency || plan.TaskPropertyName is null ?
        (string.IsNullOrEmpty(plan.CanExecuteMethodName) ? "null" : $"this.{plan.CanExecuteMethodName}"!) :
        $@"
            {(plan.HasParameter ? "x" : "()")} =>
            {{
                if (this.{plan.TaskPropertyName}?.IsCompleted == false) {{ return false; }}
                return {(string.IsNullOrEmpty(plan.CanExecuteMethodName) ? "true" : $"this.{plan.CanExecuteMethodName}({(plan.HasParameter ? "x" : "")})")};
            }}";

}
