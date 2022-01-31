using System.Collections.Immutable;
using Epoche.MVVM.SourceGenerator.Builders;
using Epoche.MVVM.SourceGenerator.Models;
using Epoche.MVVM.SourceGenerator.Plans;
using Epoche.MVVM.SourceGenerator.Writers;

namespace Epoche.MVVM.SourceGenerator;
static class SourceOutput
{
    public static void Write(SourceProductionContext context, (Compilation Left, ImmutableArray<ClassDeclarationSyntax?> Right) action)
    {
        context.CancellationToken.ThrowIfCancellationRequested();
        if (action.Right.IsDefaultOrEmpty) { return; }

        var model = new OutputModel
        {
            Context = context,
            CancellationToken = context.CancellationToken,
            Compilation = action.Left,
            ModelBaseSymbol = action.Left.GetTypeByMetadataName("Epoche.MVVM.Models.ModelBase"),
            ViewModelBaseSymbol = action.Left.GetTypeByMetadataName("Epoche.MVVM.ViewModels.ViewModelBase"),
            UseSourceGenAttributeSymbol = action.Left.GetTypeByMetadataName("Epoche.MVVM.SourceGenerator.UseSourceGenAttribute")!,
            PropertyAttributeSymbol = action.Left.GetTypeByMetadataName("Epoche.MVVM.SourceGenerator.PropertyAttribute")!,
            ChangedByAttributeSymbol = action.Left.GetTypeByMetadataName("Epoche.MVVM.SourceGenerator.ChangedByAttribute")!,
            FactoryInitializeAttributeSymbol = action.Left.GetTypeByMetadataName("Epoche.MVVM.SourceGenerator.FactoryInitializeAttribute")!,
            WithFactoryAttributeSymbol = action.Left.GetTypeByMetadataName("Epoche.MVVM.SourceGenerator.WithFactoryAttribute")!,
            InjectAttributeSymbol = action.Left.GetTypeByMetadataName("Epoche.MVVM.SourceGenerator.InjectAttribute")!,
            CommandAttributeSymbol = action.Left.GetTypeByMetadataName("Epoche.MVVM.SourceGenerator.CommandAttribute")!,
        };
        if (model.UseSourceGenAttributeSymbol is null ||
            model.PropertyAttributeSymbol is null ||
            model.ChangedByAttributeSymbol is null ||
            model.FactoryInitializeAttributeSymbol is null ||
            model.WithFactoryAttributeSymbol is null ||
            model.InjectAttributeSymbol is null ||
            model.CommandAttributeSymbol is null)
        {
            return;
        }
        foreach (var syntax in action.Right.Where(x => x is not null))
        {
            ClassModelBuilder.Build(model, syntax!);
        }
        var plan = OutputPlan.Create(model);
        OutputPlanWriter.Write(plan);
    }
}
