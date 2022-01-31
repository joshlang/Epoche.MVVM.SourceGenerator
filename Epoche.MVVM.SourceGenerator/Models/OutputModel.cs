namespace Epoche.MVVM.SourceGenerator.Models;
class OutputModel
{
    public CancellationToken CancellationToken;
    public SourceProductionContext Context;
    public Compilation Compilation = default!;

    public INamedTypeSymbol? ModelBaseSymbol;
    public INamedTypeSymbol? ViewModelBaseSymbol;

    public INamedTypeSymbol UseSourceGenAttributeSymbol = default!;
    public INamedTypeSymbol PropertyAttributeSymbol = default!;
    public INamedTypeSymbol ChangedByAttributeSymbol = default!;
    public INamedTypeSymbol FactoryInitializeAttributeSymbol = default!;
    public INamedTypeSymbol WithFactoryAttributeSymbol = default!;
    public INamedTypeSymbol InjectAttributeSymbol = default!;
    public INamedTypeSymbol CommandAttributeSymbol = default!;

    public List<ClassModel> Classes = new();
}
