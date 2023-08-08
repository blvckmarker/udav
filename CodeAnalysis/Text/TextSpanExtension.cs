namespace CodeAnalysis.Text;

internal static class TextSpanExtension
{
    public static string Substring(this string source, TextSpan span)
        => source.Substring(span.Start, span.Length);
}