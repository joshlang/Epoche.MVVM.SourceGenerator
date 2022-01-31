namespace Epoche.MVVM.SourceGenerator;
static class SourceProductionContextExtensions
{
    public static void Report(this SourceProductionContext context, DiagnosticDescriptor diagnosticDescriptor, SyntaxNode? syntax) =>
        context.ReportDiagnostic(Diagnostic.Create(diagnosticDescriptor, syntax?.GetLocation()));

    public static void Report(this SourceProductionContext context, DiagnosticDescriptor diagnosticDescriptor, ISymbol? symbol) =>
        context.ReportDiagnostic(Diagnostic.Create(diagnosticDescriptor, symbol?.Locations.FirstOrDefault()));

}
