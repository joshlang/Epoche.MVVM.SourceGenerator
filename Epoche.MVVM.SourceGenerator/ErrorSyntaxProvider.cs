namespace Epoche.MVVM.SourceGenerator;
static class ErrorSyntaxProvider
{
    public static bool Filter(SyntaxNode node, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (node is not MemberDeclarationSyntax syntax) { return false; }
        if (syntax is not FieldDeclarationSyntax &&
            syntax is not PropertyDeclarationSyntax &&
            syntax is not MethodDeclarationSyntax &&
            syntax is not ClassDeclarationSyntax)
        {
            return false;
        }
        var maybeEpoche = false;
        foreach (var attributeListSyntax in syntax.AttributeLists)
        {
            foreach (var attributeSyntax in attributeListSyntax.Attributes)
            {
                var name = attributeSyntax.Name.ToString();
                if (name.Contains("UseSourceGen"))
                {
                    return false;
                }
                maybeEpoche |=
                    name.Contains("Inject") ||
                    name.Contains("WithFactory") ||
                    name.Contains("FactoryInitialize") ||
                    name.Contains("Property") ||
                    name.Contains("Command") ||
                    name.Contains("ChangedBy");
            }
        }
        if (!maybeEpoche) { return false; }
        if (node is ClassDeclarationSyntax) { return true; }
        if (node.Parent is ClassDeclarationSyntax classSyntax)
        {
            foreach (var attributeListSyntax in syntax.AttributeLists)
            {
                foreach (var attributeSyntax in attributeListSyntax.Attributes)
                {
                    var name = attributeSyntax.Name.ToString();
                    if (name.Contains("UseSourceGen"))
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }

    public static MemberDeclarationSyntax? Transform(GeneratorSyntaxContext context, CancellationToken cancellationToken) => context.Node as MemberDeclarationSyntax;
}

