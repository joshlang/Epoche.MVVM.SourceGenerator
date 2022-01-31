namespace Epoche.MVVM.SourceGenerator;

static class Attributes
{
    public const string Text = @"
#nullable enable
using System;
namespace Epoche.MVVM.SourceGenerator;


[AttributeUsage(AttributeTargets.Class)]
sealed class UseSourceGenAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Field)]
sealed class PropertyAttribute : Attribute
{
    public string? Name { get; }
    public string? OnChange { get; set; }
    public string? EqualityComparer { get; set; }
    public bool TrackChanges { get; set; } = true;
    public string? SetterModifier { get; set; }
    public PropertyAttribute(string? name = null)
    {
        Name = name;
    }
}

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
sealed class ChangedByAttribute : Attribute
{
    public string[] Properties { get; } = Array.Empty<string>();
    public ChangedByAttribute(params string[] properties)
    {
        Properties = properties;
    }
}

[AttributeUsage(AttributeTargets.Field)]
sealed class FactoryInitializeAttribute : Attribute
{
    public Type? Type { get; set; }
    public FactoryInitializeAttribute(Type? type = null)
    {
        Type = type;
    }
}

[AttributeUsage(AttributeTargets.Class)]
sealed class WithFactoryAttribute : Attribute
{
    public string? InterfaceName { get; }
    public string? FactoryName { get; }
    public string? InterfaceAccessModifier { get; }
    public string? FactoryAccessModifier { get; }
    public WithFactoryAttribute()
    {
    }
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
sealed class InjectAttribute : Attribute
{
    public Type Type { get; }
    public string? Name { get; set; }
    public string? AccessModifier { get; set; }
    public InjectAttribute(Type type)
    {
        Type = type;
    }
}

[AttributeUsage(AttributeTargets.Method)]
sealed class CommandAttribute : Attribute
{
    public string? Name { get; }
    public bool AllowConcurrency { get; set; }
    public string? CanExecute { get; set; }
    public string? TaskName { get; set; }
    public CommandAttribute(string? name = null)
    {
        Name = name;
    }
}
";
}
