
#region

using CodeAnalysis.Lexer.Model;

#endregion

namespace CodeAnalysis.Lexer;


public class Lexer
{
    private static readonly List<string> _diagnostics = new();
    private readonly string _text;
    private int _position;

    private char CurrentChar => _position >= _text.Length ? '\0' : _text[_position];
    public IEnumerable<string> Diagnostics => _diagnostics;

    public Lexer(string text) => _text = text;
    private void Next() => _position++;

    public SyntaxToken Lex()
    {
        // numbers 
        // + - * / ()
        // <whitespace>
        if (CurrentChar is '\0')
            return new SyntaxToken(SyntaxKind.EofToken, _position, CurrentChar.ToString(), null);


        if (char.IsDigit(CurrentChar))
        {
            var start = _position;

            while (char.IsDigit(CurrentChar))
                Next();

            var length = _position - start;
            var text = _text[start.._position];

            if (!int.TryParse(text, out var value))
                _diagnostics.Add($"ERROR: The number {text} cannot be represented as int32");

            return new SyntaxToken(SyntaxKind.LiteralExpression, start, text, value);
        }

        if (char.IsWhiteSpace(CurrentChar))
        {
            var start = _position;

            while (char.IsWhiteSpace(CurrentChar))
                Next();

            var length = _position - start;
            var text = _text[start.._position];

            return new SyntaxToken(SyntaxKind.WhitespaceToken, start, text, null);
        }


        switch (CurrentChar)
        {
            case '+':
                return new SyntaxToken(SyntaxKind.PlusToken, _position++, "+", null);
            case '-':
                return new SyntaxToken(SyntaxKind.MinusToken, _position++, "-", null);
            case '*':
                return new SyntaxToken(SyntaxKind.AsteriskToken, _position++, "*", null);
            case '/':
                return new SyntaxToken(SyntaxKind.SlashToken, _position++, "/", null);
            case '(':
                return new SyntaxToken(SyntaxKind.OpenParenToken, _position++, "(", null);
            case ')':
                return new SyntaxToken(SyntaxKind.CloseParenToken, _position++, ")", null);
            default:
                _diagnostics.Add($"ERROR: Bad character input: {CurrentChar}");
                return new SyntaxToken(SyntaxKind.BadToken, _position++, _text.Substring(_position - 1, 1), null);
        }
    }

    private static char _CurrentChar(string source, int it) => it >= source.Length ? '\0' : source[it];
    private static int _MoveNext(ref int it, int len) => it + 1 <= len ? it++ : it;
    public static IEnumerable<SyntaxToken> LexTokens(string source)
    {
        for (int i = 0; i <= source.Length;)
        {
            if (_CurrentChar(source, i) is '\0')
            {
                yield return new SyntaxToken(SyntaxKind.EofToken, i, _CurrentChar(source, i).ToString(), null);
                break;
            }

            if (char.IsDigit(_CurrentChar(source, i)))
            {
                var start = i;

                while (char.IsDigit(_CurrentChar(source, i)))
                    _MoveNext(ref i, source.Length);

                var text = source[start..i];

                if (!int.TryParse(text, out var value))
                    _diagnostics.Add($"ERROR: The number {text} cannot be represented as int32");

                yield return new SyntaxToken(SyntaxKind.LiteralExpression, start, text, value);
                continue;
            }

            if (char.IsWhiteSpace(_CurrentChar(source, i)))
            {
                var start = i;

                while (char.IsWhiteSpace(_CurrentChar(source, i)))
                    _MoveNext(ref i, source.Length);

                var text = source[start..i];
                yield return new SyntaxToken(SyntaxKind.WhitespaceToken, start, text, null);
                continue;
            }

            switch (_CurrentChar(source, i))
            {
                case '+':
                    yield return new SyntaxToken(SyntaxKind.PlusToken, _MoveNext(ref i, source.Length), "+", null);
                    continue;
                case '-':
                    yield return new SyntaxToken(SyntaxKind.MinusToken, _MoveNext(ref i, source.Length), "-", null);
                    continue;
                case '*':
                    yield return new SyntaxToken(SyntaxKind.AsteriskToken, _MoveNext(ref i, source.Length), "*", null);
                    continue;
                case '/':
                    yield return new SyntaxToken(SyntaxKind.SlashToken, _MoveNext(ref i, source.Length), "/", null);
                    continue;
                case '(':
                    yield return new SyntaxToken(SyntaxKind.OpenParenToken, _MoveNext(ref i, source.Length), "(", null);
                    continue;
                case ')':
                    yield return new SyntaxToken(SyntaxKind.CloseParenToken, _MoveNext(ref i, source.Length), ")", null);
                    continue;
                default:
                    _diagnostics.Add($"ERROR: Bad character input: {_CurrentChar(source, i)}");
                    yield return new SyntaxToken(SyntaxKind.BadToken, _MoveNext(ref i, source.Length), source.Substring(i - 1, 1), null);
                    continue;

            }

        }
    }
}