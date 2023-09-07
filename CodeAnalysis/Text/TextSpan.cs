namespace CodeAnalysis.Text;

public class TextSpan
{
    public TextSpan(int start, int length)
    {
        Start = start;
        Length = length;
    }

    public static TextSpan FromBounds(int start, int end) => new TextSpan(start, end - start);
    public override string ToString() => $"{Start}:{End}";
    public int Start { get; }
    public int Length { get; }
    public int End => Start + Length;
}
