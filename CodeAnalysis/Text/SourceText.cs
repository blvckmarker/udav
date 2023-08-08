using System.Collections;
using System.Collections.Immutable;

namespace CodeAnalysis.Text;
public class SourceText : IEnumerable<char>
{
    private readonly string _text;

    public ImmutableArray<TextLine> Lines { get; }
    private SourceText(string source)
    {
        _text = source;
        Lines = ParseLines(this, source);
    }

    public int Length => _text.Length;
    public char this[int index] => _text[index];
    public string this[int start, int end] => _text[start..end];
    public static SourceText From(string text)
    {
        return new SourceText(text);
    }
    public int GetLineIndex(int position)
    {
        var lower = 0;
        var upper = Lines.Length - 1;
        while (lower <= upper)
        {
            var index = lower + (upper - lower) / 2;
            var startLine = Lines[index].Start;
            var endLine = Lines[index].End;

            if (position >= startLine && position <= endLine)
                return index;
            else if (position < startLine)
                lower = index + 1;
            else
                upper = index - 1;
        }
        return -1;
    }
    private ImmutableArray<TextLine> ParseLines(SourceText sourceText, string source)
    {
        var resultLines = new List<TextLine>();

        var lineStart = 0;
        var currPosition = 0;

        for (; currPosition < source.Length;)
        {
            var lineBreakWidth = GetLineBreakWidth(source, currPosition);

            if (lineBreakWidth == 0)
                currPosition++;
            else
            {
                AddLine(resultLines, sourceText, lineStart, currPosition, lineBreakWidth);

                currPosition += lineBreakWidth;
                lineStart = currPosition;
            }
        }

        if (currPosition > lineStart)
            AddLine(resultLines, sourceText, lineStart, currPosition, 0);

        return resultLines.ToImmutableArray();
    }
    private static void AddLine(IList<TextLine> resultLines, SourceText text, int lineStart, int currPosition, int lineBreakWidth)
    {
        var lineLength = currPosition - lineStart;
        var lineLengthWithBreak = lineLength + lineBreakWidth;
        var textLine = new TextLine(text, lineStart, lineLength, lineLengthWithBreak);
        resultLines.Add(textLine);
    }
    private static int GetLineBreakWidth(string source, int i)
    {
        var curr = source[i];
        var next = i + 1 >= source.Length ? '\0' : source[i + 1];
        if (curr == '\r' && next == '\n')
            return 2;
        if (curr == '\n' || curr == '\r')
            return 1;
        return 0;
    }
    public override string ToString() => _text;
    public string ToString(int start, int length) => _text.Substring(start, length);
    public string ToString(TextSpan span) => ToString(span.Start, span.Length);

    public IEnumerator<char> GetEnumerator() => _text.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
