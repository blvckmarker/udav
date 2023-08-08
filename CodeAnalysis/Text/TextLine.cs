namespace CodeAnalysis.Text;

public class TextLine
{
    public TextLine(SourceText text, int start, int length, int lengthWithLineBreak)
    {
        Text = text;
        Start = start;
        Length = length;
        LengthWithLineBreak = lengthWithLineBreak;
    }

    public SourceText Text { get; }
    public int Start { get; }
    public int Length { get; }
    public int End => Start + Length;
    public TextSpan Span => TextSpan.FromBounds(Start, End);
    public TextSpan SpanWithLineBreak => TextSpan.FromBounds(Start, Start + LengthWithLineBreak);
    public int LengthWithLineBreak { get; }
}