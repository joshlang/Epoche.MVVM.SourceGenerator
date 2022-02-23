using Epoche.MVVM.SourceGenerator.Models;

namespace Epoche.MVVM.SourceGenerator.Plans;
class OutputPlan
{
    public CancellationToken CancellationToken;
    public SourceProductionContext Context;
    public readonly List<ClassPlan> ClassPlans = new();
    public readonly HashSet<string> CachePropertyNames = new();

    public static OutputPlan Create(OutputModel outputModel)
    {
        var plan = new OutputPlan
        {
            CancellationToken = outputModel.CancellationToken,
            Context = outputModel.Context
        };
        var remainingClassModels = new HashSet<ClassModel>(outputModel.Classes);
        var classPlansByModel = new Dictionary<ClassModel, ClassPlan>();
        while (remainingClassModels.Count > 0)
        {
            foreach (var classModel in remainingClassModels.ToList())
            {
                if (classModel.SourceGeneratedBaseClass is not null &&
                    remainingClassModels.Contains(classModel.SourceGeneratedBaseClass))
                {
                    continue;
                }
                var baseClassPlan = classModel.SourceGeneratedBaseClass is null ? null : classPlansByModel[classModel.SourceGeneratedBaseClass];
                var classPlan = ClassPlan.Create(outputModel, classModel, baseClassPlan);
                plan.ClassPlans.Add(classPlan);
                classPlansByModel[classModel] = classPlan;
                remainingClassModels.Remove(classModel);
            }
        }
        SetupCachePropertyNames(plan);
        return plan;
    }

    static void SetupCachePropertyNames(OutputPlan plan)
    {
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
