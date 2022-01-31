namespace Epoche.MVVM.SourceGenerator;
static class SyntaxProvider
{
    public static bool Filter(SyntaxNode node, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (node is ClassDeclarationSyntax syntax)
        {
            foreach (var attributeListSyntax in syntax.AttributeLists)
            {
                foreach (var attributeSyntax in attributeListSyntax.Attributes)
                {
                    var name = attributeSyntax.Name.ToString();
                    if (name.Contains("UseSourceGen"))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public static ClassDeclarationSyntax? Transform(GeneratorSyntaxContext context, CancellationToken cancellationToken) => context.Node as ClassDeclarationSyntax;
}

