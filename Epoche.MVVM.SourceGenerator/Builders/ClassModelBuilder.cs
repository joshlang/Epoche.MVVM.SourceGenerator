﻿using Epoche.MVVM.SourceGenerator.Builders.Attributes;
using Epoche.MVVM.SourceGenerator.Models;

namespace Epoche.MVVM.SourceGenerator.Builders;
static class ClassModelBuilder
{
    public static void Build(OutputModel outputModel, ClassDeclarationSyntax syntax)
    {
        outputModel.CancellationToken.ThrowIfCancellationRequested();

        if (syntax.Parent is TypeDeclarationSyntax)
        {
            outputModel.Context.Report(Diagnostics.Errors.SubClass, syntax);
            return;
        }

        var model = new ClassModel();
        var semanticModel = outputModel.Compilation.GetSemanticModel(syntax.SyntaxTree);
        if (semanticModel.GetDeclaredSymbol(syntax) is not INamedTypeSymbol classSymbol) { return; }

        model.GenericTypeArguments = classSymbol.TypeParameters.Select(x => x.Name).ToArray();
        model.Namespace = classSymbol.ContainingNamespace.ToDisplayString();
        model.ClassName = classSymbol.Name.ToString();
        model.IsMVVMModel = outputModel.ModelBaseSymbol is not null && classSymbol.IsClassDerivedFrom(outputModel.ModelBaseSymbol);
        model.IsMVVMViewModel = model.IsMVVMModel && outputModel.ViewModelBaseSymbol is not null && classSymbol.IsClassDerivedFrom(outputModel.ViewModelBaseSymbol);
        model.FullBaseClassName = classSymbol.BaseType?.ToDisplayString();
        model.FullBaseClassNameWithoutGenerics = classSymbol.BaseType?.ToDisplayString().Split('<')[0];
        model.BaseClassGenericTypeParameters = classSymbol.BaseType?.TypeParameters.Select(x => x.Name).ToArray() ?? Array.Empty<string>();
        model.BaseClassGenericTypeArguments = classSymbol.BaseType?.TypeArguments.Select(x => x.ToDisplayString()).ToArray() ?? Array.Empty<string>();

        foreach (var attributeData in classSymbol.GetAttributes())
        {
            if (SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, outputModel.UseSourceGenAttributeSymbol))
            {
                UseSourceGenAttributeModelBuilder.Build(outputModel, model, attributeData);
            }
            else if (SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, outputModel.InjectAttributeSymbol))
            {
                InjectAttributeModelBuilder.Build(outputModel, model, attributeData);
            }
            else if (SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, outputModel.WithFactoryAttributeSymbol))
            {
                WithFactoryAttributeModelBuilder.Build(outputModel, model, attributeData);
            }
        }
        if (model.UseSourceGenAttribute is null) { return; }
        if (!syntax.Modifiers.Any(x => x.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.PartialKeyword)))
        {
            outputModel.Context.Report(Diagnostics.Errors.NotPartial, syntax);
            return;
        }

        foreach (var member in classSymbol.GetMembers())
        {
            switch (member)
            {
                case IFieldSymbol symbol:
                    FieldModelBuilder.Build(outputModel, model, symbol);
                    break;
                case IPropertySymbol symbol:
                    PropertyModelBuilder.Build(outputModel, model, symbol);
                    break;
                case IMethodSymbol symbol:
                    MethodModelBuilder.Build(outputModel, model, symbol);
                    break;
            }
        }

        var constructor = classSymbol
            .BaseType?
            .Constructors
            .AsEnumerable()
            .Where(x => !x.IsStatic)
            .OrderByDescending(x => x.Parameters.Length)
            .FirstOrDefault();
        if (constructor is not null)
        {
            foreach (var p in constructor.Parameters)
            {
                model.BaseConstructorArgs.Add((p.Type.ToDisplayString(), p.Name));
            }
        }

        outputModel.Classes.Add(model);
    }

    public static void Finish(OutputModel outputModel)
    {
        var byType = outputModel.Classes.ToDictionary(x => $"{x.Namespace}.{x.ClassName}+{x.GenericTypeArguments.Length}");
        foreach (var classModel in outputModel.Classes.Where(x => x.BaseConstructorArgs.Count == 0))
        {
            classModel.SourceGeneratedBaseClass =
                byType.TryGetValue($"{classModel.FullBaseClassNameWithoutGenerics}+{classModel.BaseClassGenericTypeParameters.Length}", out var baseModel) ?
                baseModel :
                null;
        }
    }
}
