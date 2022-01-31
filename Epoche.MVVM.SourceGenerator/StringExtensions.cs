namespace Epoche.MVVM.SourceGenerator;
static class StringExtensions
{
    public static string AddSpace(this string? s) => string.IsNullOrEmpty(s) ? "" : s + " ";
    public static string? NullIfEmpty(this string? s) => string.IsNullOrEmpty(s) ? null : s;

    public static string NameWithoutInterface(this string s)
    {
        var lastDot = s.LastIndexOf('.') + 1;
        if (s.Length - lastDot > 2 && s[lastDot] == 'I' && char.IsUpper(s[lastDot + 1])) { return s.Substring(lastDot + 1); }
        return s.Substring(lastDot);
    }

    public static string NameWithoutGenerics(this string s) => s.Split('<')[0];

    public static string ToCamelCase(this string s) => string.IsNullOrEmpty(s) ? s : char.ToLower(s[0]) + s.Substring(1);
    public static string ToPascalCase(this string s) => string.IsNullOrEmpty(s) ? s : char.ToUpper(s[0]) + s.Substring(1);

    public static string ToFieldName(this string s) => string.IsNullOrEmpty(s) ? s : char.IsUpper(s[0]) ? ToCamelCase(s) : $"_{s}";
    public static string ToPropertyName(this string s) => string.IsNullOrEmpty(s) ? s : s[0] == '_' ? ToPascalCase(s.Substring(1)) : char.IsLower(s[0]) ? ToPascalCase(s) : $"{s}_";

    public static string Up(this string s) => string.IsNullOrEmpty(s) ? s : s.TrimStart('\n', '\r', ' ', '\t');
}
