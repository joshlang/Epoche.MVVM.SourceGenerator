namespace Epoche.MVVM.SourceGenerator;
static class Diagnostics
{
    public static class Errors
    {
        static DiagnosticDescriptor Create(string id, string text) => new DiagnosticDescriptor(id, text, text, "SourceGeneration", DiagnosticSeverity.Error, true);

        public static DiagnosticDescriptor NotPartial = Create("GEN001", "Classes decorated with [UseSourceGen] must be declared with the 'partial' modifier");
        public static DiagnosticDescriptor SubClass = Create("GEN002", "Nested types are not supported");
        public static DiagnosticDescriptor InjectMissingType = Create("GEN003", "[Inject] is missing a Type");
        public static DiagnosticDescriptor NotViewModel = Create("GEN004", "Class must be derived from ViewModelBase to use this attribute");
        public static DiagnosticDescriptor MultipleCommandParameters = Create("GEN005", "A [Command] method cannot have multiple parameters");
        public static DiagnosticDescriptor InjectMissingUseSourceGen = Create("GEN006", "[Inject] requires the class to be decorated with [UseSourceGen]");
        public static DiagnosticDescriptor WithFactoryMissingUseSourceGen = Create("GEN007", "[WithFactory] requires the class to be decorated with [UseSourceGen]");
        public static DiagnosticDescriptor PropertyMissingUseSourceGen = Create("GEN008", "[Property] requires the class to be decorated with [UseSourceGen]");
        public static DiagnosticDescriptor ChangedByMissingUseSourceGen = Create("GEN009", "[ChangedBy] requires the class to be decorated with [UseSourceGen]");
        public static DiagnosticDescriptor CommandMissingUseSourceGen = Create("GEN010", "[Command] requires the class to be decorated with [UseSourceGen]");
        public static DiagnosticDescriptor FactoryInitializeMissingUseSourceGen = Create("GEN011", "[FactoryInitialize] requires the class to be decorated with [UseSourceGen]");
    }
    public static class Warnings
    {
        static DiagnosticDescriptor Create(string id, string text) => new DiagnosticDescriptor(id, text, text, "SourceGeneration", DiagnosticSeverity.Warning, true);

        public static DiagnosticDescriptor NoFactoryInfo = Create("WGEN001", "[WithFactory] has no effect when both interface and factory names are missing");
    }
}
