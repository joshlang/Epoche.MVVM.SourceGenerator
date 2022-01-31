using Epoche.MVVM.SourceGenerator.Models;
using Epoche.MVVM.SourceGenerator.Plans;

namespace Epoche.MVVM.SourceGenerator.Writers;
static class OutputPlanWriter
{
    public static void Write(OutputPlan plan)
    {
        plan.CancellationToken.ThrowIfCancellationRequested();
        
        foreach (var classPlan in plan.ClassPlans)
        {
            ClassPlanWriter.Write(plan, classPlan);
        }

        plan.Context.AddSource("CachedPropertyChangeEventArgs.g.cs", CachedPropertyChangeEventArgs(plan));
    }

    static string CachedPropertyChangeEventArgs(OutputPlan plan) => $@"
#nullable enable
namespace Epoche.MVVM;
static class CachedPropertyChangeEventArgs
{{
    {string.Concat(plan.CachePropertyNames.Select(PropertyChangeEventArgs)).Up()}
}}
";

    static string PropertyChangeEventArgs(string propertyName) => $@"
    public static readonly System.ComponentModel.PropertyChangedEventArgs {propertyName} = new(""{propertyName}"");";
}
