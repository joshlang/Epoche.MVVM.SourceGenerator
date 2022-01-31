using System.Collections.Immutable;

namespace Epoche.MVVM.SourceGenerator;
static class ErrorSourceOutput
{
    public static void Attributes(SourceProductionContext context, (Compilation Left, ImmutableArray<MemberDeclarationSyntax?> Right) action)
    {
        context.CancellationToken.ThrowIfCancellationRequested();

        if (action.Right.IsDefaultOrEmpty) { return; }

        var useSourceGenAttributeSymbol = action.Left.GetTypeByMetadataName("Epoche.MVVM.SourceGenerator.UseSourceGenAttribute");
        var factoryInitializeAttributeSymbol = action.Left.GetTypeByMetadataName("Epoche.MVVM.SourceGenerator.FactoryInitializeAttribute");
        var propertyAttributeSymbol = action.Left.GetTypeByMetadataName("Epoche.MVVM.SourceGenerator.PropertyAttribute");
        var withFactoryAttributeSymbol = action.Left.GetTypeByMetadataName("Epoche.MVVM.SourceGenerator.WithFactoryAttribute");
        var injectAttributeSymbol = action.Left.GetTypeByMetadataName("Epoche.MVVM.SourceGenerator.InjectAttribute");
        var commandAttributeSymbol = action.Left.GetTypeByMetadataName("Epoche.MVVM.SourceGenerator.CommandAttribute");
        var changedByAttributeSymbol = action.Left.GetTypeByMetadataName("Epoche.MVVM.SourceGenerator.ChangedByAttribute");

        if (useSourceGenAttributeSymbol is null ||
            factoryInitializeAttributeSymbol is null ||
            propertyAttributeSymbol is null ||
            withFactoryAttributeSymbol is null ||
            injectAttributeSymbol is null ||
            commandAttributeSymbol is null ||
            changedByAttributeSymbol is null)
        {
            return;
        }

        foreach (var syntax in action.Right)
        {
            if (syntax is null) { continue; }

            var semanticModel = action.Left.GetSemanticModel(syntax.SyntaxTree);
            if (syntax is ClassDeclarationSyntax)
            {
                if (semanticModel.GetDeclaredSymbol(syntax) is not INamedTypeSymbol classSymbol) { continue; }
                var useSourceGen = false;
                var useInject = false;
                var useWithFactory = false;
                foreach (var attributeData in classSymbol.GetAttributes())
                {
                    if (SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, useSourceGenAttributeSymbol))
                    {
                        useSourceGen = true;
                        break;
                    }
                    else if (SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, injectAttributeSymbol))
                    {
                        useInject = true;
                    }
                    else if (SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, withFactoryAttributeSymbol))
                    {
                        useWithFactory = true;
                    }
                }
                if (useSourceGen) { continue; }
                if (useWithFactory)
                {
                    context.Report(Diagnostics.Errors.WithFactoryMissingUseSourceGen, syntax);
                }
                if (useInject)
                {
                    context.Report(Diagnostics.Errors.InjectMissingUseSourceGen, syntax);
                }
            }
            else
            {
                if (syntax.Parent is null) { continue; }

                ISymbol? symbol = syntax switch
                {
                    FieldDeclarationSyntax fieldSyntax when (fieldSyntax.Declaration.Variables.Count > 0) => semanticModel.GetDeclaredSymbol(fieldSyntax.Declaration.Variables[0]),
                    PropertyDeclarationSyntax => semanticModel.GetDeclaredSymbol(syntax),
                    MethodDeclarationSyntax => semanticModel.GetDeclaredSymbol(syntax),
                    _ => null
                };
                if (symbol is null) { continue; }
                var useProperty = false;
                var useFactoryInitialize = false;
                var useChangedBy = false;
                var useCommand = false;
                foreach (var attributeData in symbol.GetAttributes())
                {
                    if (SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, propertyAttributeSymbol))
                    {
                        useProperty = true;
                    }
                    else if (SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, factoryInitializeAttributeSymbol))
                    {
                        useFactoryInitialize = true;
                    }
                    else if (SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, changedByAttributeSymbol))
                    {
                        useChangedBy = true;
                    }
                    else if (SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, commandAttributeSymbol))
                    {
                        useCommand = true;
                    }
                }
                if (!useProperty && !useFactoryInitialize && !useChangedBy && !useCommand) { continue; }
                if (syntax.Parent is ClassDeclarationSyntax classSyntax) 
                {
                    semanticModel = action.Left.GetSemanticModel(syntax.SyntaxTree);
                    if (semanticModel.GetDeclaredSymbol(classSyntax) is not INamedTypeSymbol classSymbol) { continue; }
                    var useSourceGen = false; 
                    foreach (var attributeData in classSymbol.GetAttributes())
                    {
                        if (SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, useSourceGenAttributeSymbol))
                        {
                            useSourceGen = true;
                            break;
                        }
                    }
                    if (useSourceGen) { continue; }
                }
                if (useFactoryInitialize)
                {
                    context.Report(Diagnostics.Errors.FactoryInitializeMissingUseSourceGen, syntax);
                }
                if (useProperty)
                {
                    context.Report(Diagnostics.Errors.PropertyMissingUseSourceGen, syntax);
                }
                if (useChangedBy)
                {
                    context.Report(Diagnostics.Errors.ChangedByMissingUseSourceGen, syntax);
                }
                if (useCommand)
                {
                    context.Report(Diagnostics.Errors.CommandMissingUseSourceGen, syntax);
                }
            }
        }
    }
}
