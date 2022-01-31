using Epoche.MVVM.SourceGenerator.Models;

namespace Epoche.MVVM.SourceGenerator.Plans;
class OutputPlan
{
    public CancellationToken CancellationToken;
    public SourceProductionContext Context;
    public List<ClassPlan> ClassPlans = default!;
    public HashSet<string> CachePropertyNames = default!;

    public static OutputPlan Create(OutputModel outputModel)
    {
        var plan = new OutputPlan
        {
            CancellationToken = outputModel.CancellationToken,
            Context = outputModel.Context
        };
        plan.ClassPlans = outputModel.Classes.Select(x => ClassPlan.Create(outputModel, x)).ToList();
        SetupCachePropertyNames(plan);
        return plan;
    }

    static void SetupCachePropertyNames(OutputPlan plan)
    {
        plan.CachePropertyNames = new();
        foreach (var classPlan in plan.ClassPlans)
        {
            foreach (var field in classPlan.FieldPropertiesPlans)
            {
                plan.CachePropertyNames.Add(field.PropertyName);
                foreach (var affected in field.AffectedProperties)
                {
                    plan.CachePropertyNames.Add(affected);
                }
            }
            foreach (var method in classPlan.CommandPlans.Where(x => x.TaskPropertyName is not null))
            {
                plan.CachePropertyNames.Add(method.TaskPropertyName!);
            }
        }
    }
}
