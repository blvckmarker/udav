
#region

using CodeAnalysis.Lexer.Model;

#endregion

namespace CodeAnalysis.Lexer;


public class Lexer
{
    private readonly List<string> _diagnostics = new();
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
            return new SyntaxToken(SyntaxKind.Eof, _position, CurrentChar.ToString(), null);


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

            return new SyntaxToken(SyntaxKind.Whitespace, start, text, null);
        }


        switch (CurrentChar)
        {
            case '+':
                return new SyntaxToken(SyntaxKind.Plus, _position++, "+", null);
            case '-':
                return new SyntaxToken(SyntaxKind.Minus, _position++, "-", null);
            case '*':
                return new SyntaxToken(SyntaxKind.StarToken, _position++, "*", null);
            case '/':
                return new SyntaxToken(SyntaxKind.SlashToken, _position++, "/", null);
            case '(':
                return new SyntaxToken(SyntaxKind.LeftBracket, _position++, "(", null);
            case ')':
                return new SyntaxToken(SyntaxKind.RightBracket, _position++, ")", null);
            default:
                _diagnostics.Add($"ERROR: Bad character input: {CurrentChar}");
                return new SyntaxToken(SyntaxKind.BadToken, _position++, _text.Substring(_position - 1, 1), null);
        }
    }
}