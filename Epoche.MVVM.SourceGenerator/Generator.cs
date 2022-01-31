namespace Epoche.MVVM.SourceGenerator;

[Generator]
public class Generator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        AddGenerator(context);
        AddErrorChecks_Attributes(context);
    }

    void AddGenerator(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(OutputStaticFiles);

        var classDeclarations = context
            .SyntaxProvider
            .CreateSyntaxProvider(SyntaxProvider.Filter, SyntaxProvider.Transform)
            .Where(x => x is not null);

        var compilationAndClasses = context.CompilationProvider.Combine(classDeclarations.Collect());

        context.RegisterSourceOutput(compilationAndClasses, SourceOutput.Write);
    }

    void AddErrorChecks_Attributes(IncrementalGeneratorInitializationContext context)
    {
        var declarations = context
            .SyntaxProvider
            .CreateSyntaxProvider(ErrorSyntaxProvider.Filter, ErrorSyntaxProvider.Transform)
            .Where(x => x is not null);

        var compilationAndDeclarations = context.CompilationProvider.Combine(declarations.Collect());

        context.RegisterSourceOutput(compilationAndDeclarations, ErrorSourceOutput.Attributes);
    }

    static void OutputStaticFiles(IncrementalGeneratorPostInitializationContext context) => context.AddSource("SourceGeneratorAttributes.g.cs", Attributes.Text);
}
