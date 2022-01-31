namespace Epoche.MVVM.SourceGenerator;
static class INamedTypeSymbolExtensions
{
    public static bool IsClassDerivedFrom(this INamedTypeSymbol? symbol, INamedTypeSymbol baseSymbol) =>
        baseSymbol is null || symbol?.TypeKind != TypeKind.Class ? false :
        SymbolEqualityComparer.Default.Equals(symbol, baseSymbol) ? true :
        IsClassDerivedFrom(symbol.BaseType, baseSymbol);
}
