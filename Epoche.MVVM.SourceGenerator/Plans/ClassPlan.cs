using Epoche.MVVM.SourceGenerator.Models;

namespace Epoche.MVVM.SourceGenerator.Plans;
class ClassPlan
{
    public string Namespace = default!;
    public string ClassName = default!;
    public string FullClassName = default!;
    public string ClassDefinition = default!;
    public string? BaseClassDefinition;
    public Dictionary<string, int> GenericTypeParameterIndexes = new();

    public class FactoryPlan
    {
        public string? FactoryName;
        public string? FactoryAccessModifier;
    }
    public FactoryPlan Factory = new();

    public List<string> BaseArguments = default!;
    public class ConstructorArg
    {
        public string FullTypeName = default!;
        public string ArgumentName = default!;
    }
    public List<ConstructorArg> ConstructorArguments = default!;

    public Dictionary<string, List<ConstructorArg>> ConstructorArgumentsByType = default!;

    public class InjectedPropertyPlan
    {
        public string FullTypeName = default!;
        public string PropertyName = default!;
        public string? PropertyAccessModifier;
        public ConstructorArg ConstructorArg = default!;
    }
    public List<InjectedPropertyPlan> InjectedPropertyPlans = default!;

    public class CommandPlan
    {
        public bool IsAsync;
        public bool AllowConcurrency;
        public bool HasParameter;
        public string? ParameterFullTypeName;
        public string MethodName = default!;
        public string? TaskFieldName;
        public string? TaskPropertyName;
        public string? AsyncReturnFullTypeName;
        public string? CanExecuteMethodName;
        public string PropertyName = default!;
    }
    public List<CommandPlan> CommandPlans = default!;

    public class FieldPropertyPlan
    {
        public string FieldName = default!;
        public string PropertyName = default!;
        public string FullTypeName = default!;
        public bool IsReadOnly;
        public string? SetterModifier;
        public string? OnChange;
        public string? EqualityComparer;
        public bool TrackChanges = true;
        public ConstructorArg? FactoryConstructorArg;
        public string? FactoryInitializerExpression;
        public HashSet<string> AffectedProperties = new();
    }
    public List<FieldPropertyPlan> FieldPropertiesPlans = default!;

    public static ClassPlan Create(OutputModel outputModel, ClassModel model, ClassPlan? baseClassPlan)
    {
        var plan = new ClassPlan
        {
            Namespace = model.Namespace,
            ClassName = model.ClassName,
            FullClassName = $"{model.Namespace}.{model.ClassName}",
            ClassDefinition = model.ClassName,
            BaseClassDefinition = model.FullBaseClassName.NullIfEmpty(),
            GenericTypeParameterIndexes = model.GenericTypeArguments.Select((x, i) => new { x, i }).ToDictionary(x => x.x, x => x.i)
        };
        if (model.GenericTypeArguments.Any())
        {
            plan.ClassDefinition += "<" + string.Join(", ", model.GenericTypeArguments) + ">";
        }

        SetupFactory(model, plan);
        SetupConstructorArgs(model, plan, baseClassPlan);
        SetupInjectedProperties(model, plan);
        SetupCommands(model, plan);
        SetupFieldProperties(model, plan);
        SetupChangeBy(model, plan);

        return plan;
    }

    static void SetupFactory(ClassModel model, ClassPlan plan)
    {
        if (model.WithFactoryAttribute is not null)
        {
            plan.Factory.FactoryAccessModifier = model.WithFactoryAttribute.FactoryAccessModifier.NullIfEmpty();
            plan.Factory.FactoryName = model.WithFactoryAttribute.FactoryName.NullIfEmpty();
            if (model.WithFactoryAttribute.UseFactoryNameDefault)
            {
                plan.Factory.FactoryName = $"{model.ClassName}Factory";
            }
        }
    }
    static void SetupConstructorArgs(ClassModel model, ClassPlan plan, ClassPlan? baseClassPlan)
    {
        plan.BaseArguments = model.BaseConstructorArgs.Select(x => x.Name).ToList();
        var baseArgTypes = new HashSet<string>(model.BaseConstructorArgs.Select(x => x.FullTypeName));

        plan.ConstructorArguments = new();
        if (baseClassPlan is not null)
        {
            plan.ConstructorArguments.AddRange(baseClassPlan.ConstructorArguments.Select(x =>
            {
                var arg = new ConstructorArg
                {
                    ArgumentName = x.ArgumentName,
                    FullTypeName = x.FullTypeName
                };
                foreach (var kvp in baseClassPlan.GenericTypeParameterIndexes)
                {
                    var actual = model.BaseClassGenericTypeArguments[kvp.Value];
                    if (arg.FullTypeName == kvp.Key)
                        arg.FullTypeName = actual;
                    else if (arg.FullTypeName.Contains(kvp.Key))
                    {
                        arg.FullTypeName = arg.FullTypeName
                            .Replace($"<{kvp.Key}>", $"<{actual}>") // super ghetto
                            .Replace($"<{kvp.Key},", $"<{actual},") // if it's a problem, write an actual
                            .Replace($",{kvp.Key},", $",{actual},") // replacer that looks at all the
                            .Replace($",{kvp.Key}>", $",{actual}>") // types and generic arguments
                            .Replace($" {kvp.Key},", $" {actual},") // and generic generic arguments
                            .Replace($" {kvp.Key}>", $" {actual}>"); // and generic generic generic arguments ... etc
                    }
                }
                return arg;
            }));
            plan.BaseArguments.AddRange(baseClassPlan.ConstructorArguments.Select(x => x.ArgumentName));
        }
        plan.ConstructorArguments.AddRange(model.BaseConstructorArgs.Select(x => new ConstructorArg
        {
            FullTypeName = x.FullTypeName,
            ArgumentName = x.Name
        }));
        plan.ConstructorArguments.AddRange(model.InjectAttributes.Where(x => !baseArgTypes.Contains(x.Type)).Select(x => new ConstructorArg
        {
            FullTypeName = x.Type,
            ArgumentName = x.Name.NullIfEmpty() ?? x.Type.NameWithoutInterface().NameWithoutGenerics().ToCamelCase()
        }));

        var constructorArgTypes = new HashSet<string>(plan.ConstructorArguments.Select(x => x.FullTypeName));
        foreach (var fieldModel in model.FieldModels.Where(x => x.FactoryInitializeAttribute is not null))
        {
            var type = GetFactoryInitializeType(fieldModel);
            if (constructorArgTypes.Contains(type)) { continue; }
            plan.ConstructorArguments.Add(new ConstructorArg
            {
                FullTypeName = type,
                ArgumentName = $"{fieldModel.TypeName.ToCamelCase()}Factory"
            });
        }
        plan.ConstructorArgumentsByType = plan.ConstructorArguments.GroupBy(x => x.FullTypeName).ToDictionary(x => x.Key, x => x.ToList());
    }
    static void SetupInjectedProperties(ClassModel model, ClassPlan plan)
    {
        plan.InjectedPropertyPlans = model.InjectAttributes.Select(x => new InjectedPropertyPlan
        {
            FullTypeName = x.Type,
            PropertyAccessModifier = x.AccessModifier.NullIfEmpty(),
            PropertyName = x.Name.NullIfEmpty() ?? x.Type.NameWithoutInterface().NameWithoutGenerics(),
            ConstructorArg = plan.ConstructorArgumentsByType[x.Type][0]
        }).ToList();
        var types = new HashSet<string>(plan.InjectedPropertyPlans.Select(x => x.FullTypeName));
        foreach (var field in model.FieldModels.Where(x => x.FactoryInitializeAttribute is not null))
        {
            var factoryType = GetFactoryInitializeType(field);
            if (types.Add(factoryType))
            {
                plan.InjectedPropertyPlans.Add(new InjectedPropertyPlan
                {
                    FullTypeName = factoryType,
                    PropertyName = $"{field.TypeName}Factory",
                    ConstructorArg = plan.ConstructorArgumentsByType[factoryType][0]
                });
            }
        }
    }
    static string GetFactoryInitializeType(FieldModel fieldModel) => $"Epoche.MVVM.Models.IModelFactory<{fieldModel.FullTypeName}>";
    static void SetupCommands(ClassModel model, ClassPlan plan) => plan.CommandPlans = model.MethodModels.Where(x => x.CommandAttribute is not null).Select(x =>
    {
        var commandPlan = new CommandPlan
        {
            AllowConcurrency = x.CommandAttribute!.AllowConcurrency,
            IsAsync = x.IsAsync,
            AsyncReturnFullTypeName = x.AsyncReturnType.NullIfEmpty(),
            MethodName = x.MethodName,
            CanExecuteMethodName = x.CommandAttribute.CanExecute,
            TaskPropertyName = x.CommandAttribute.TaskName.NullIfEmpty(),
            ParameterFullTypeName = x.CommandParameterType.NullIfEmpty(),
            HasParameter = x.CommandParameterType.NullIfEmpty() is not null,
            PropertyName = x.CommandAttribute.Name.NullIfEmpty() ?? $"{(x.MethodName.EndsWith("Async") ? x.MethodName.Substring(0, x.MethodName.Length - 5) : x.MethodName)}Command"
        };
        if (commandPlan.TaskPropertyName is null && x.CommandAttribute.UseDefaultTaskName)
        {
            commandPlan.TaskPropertyName = $"{commandPlan.PropertyName}Task";
        }
        if (!commandPlan.IsAsync)
        {
            commandPlan.TaskPropertyName = null;
        }
        commandPlan.TaskFieldName = commandPlan.TaskPropertyName?.ToFieldName();
        return commandPlan;
    }).ToList();

    static void SetupFieldProperties(ClassModel model, ClassPlan plan) => plan.FieldPropertiesPlans = model.FieldModels.Where(x => x.PropertyAttribute is not null).Select(x => new FieldPropertyPlan
    {
        FieldName = x.FieldName,
        PropertyName = x.PropertyAttribute!.Name.NullIfEmpty() ?? x.FieldName.ToPropertyName(),
        FullTypeName = x.FullTypeName,
        IsReadOnly = x.IsReadOnly,
        OnChange = x.PropertyAttribute.OnChange.NullIfEmpty(),
        EqualityComparer = x.PropertyAttribute.EqualityComparer.NullIfEmpty(),
        SetterModifier = x.PropertyAttribute.SetterModifier.NullIfEmpty(),
        TrackChanges = x.PropertyAttribute.TrackChanges,
        FactoryConstructorArg = x.FactoryInitializeAttribute is null ? null : plan.ConstructorArgumentsByType[GetFactoryInitializeType(x)][0],
        FactoryInitializerExpression = x.FactoryInitializeAttribute?.InitializeExpression.NullIfEmpty()
    }).ToList();

    static void SetupChangeBy(ClassModel model, ClassPlan plan)
    {
        var fieldsByName = plan.FieldPropertiesPlans.ToDictionary(x => x.FieldName);
        var fieldsByProp = plan.FieldPropertiesPlans.ToDictionary(x => x.PropertyName);
        foreach (var prop in model.PropertyModels.Where(x => x.ChangedByAttribute is not null))
        {
            foreach (var propName in prop.ChangedByAttribute!.Properties)
            {
                if (fieldsByName.TryGetValue(propName, out var fieldPlan) ||
                    fieldsByProp.TryGetValue(propName, out fieldPlan))
                {
                    fieldPlan.AffectedProperties.Add(prop.PropertyName);
                }
            }
        }
    }
}