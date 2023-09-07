namespace CodeAnalysis.Text;

public class TextLine
{
    public TextLine(string text, int start, int length, int lengthWithLineBreak)
    {
        Text = text;
        Length = length;
        Span = TextSpan.FromBounds(start, length + start);
        LengthWithLineBreak = lengthWithLineBreak;
    }

    public string Text { get; }
    public int Length { get; }
    public TextSpan Span { get; }
    public TextSpan SpanWithLineBreak => TextSpan.FromBounds(Span.Start, Span.Start + LengthWithLineBreak);
    public int LengthWithLineBreak { get; }
}