#region
using CodeAnalysis.Parser.Expressions.AST;
using CodeAnalysis.Scanner.Model;
#endregion

namespace CodeAnalysis.Scanner.Shared;

public class Lexer
{
    private readonly List<string> _diagnostics = new();
    private readonly string _text;
    private bool isAlreadyLexicalized = false;
    private int _position;

    private char CurrentChar => _position >= _text.Length ? '\0' : _text[_position];
    public IEnumerable<string> Diagnostics => _diagnostics;

    public Lexer(string text) => _text = text;
    private void Next() => _position++;
    public SyntaxToken Lex()
    {
        if (isAlreadyLexicalized)
            throw new Exception("The source program is already lexicalized");

        // numbers 
        // + - * / ()
        // <whitespace>
        if (CurrentChar is '\0')
        {
            isAlreadyLexicalized = true;
            return new SyntaxToken(SyntaxKind.EofToken, _position, CurrentChar.ToString(), null);
        }

        if (char.IsDigit(CurrentChar))
        {
            var start = _position;

            while (char.IsDigit(CurrentChar))
                Next();

            var length = _position - start;
            var text = _text[start.._position];

            if (!int.TryParse(text, out var value))
                _diagnostics.Add($"ERROR: The number {text} cannot be represented as int32");

            return new SyntaxToken(SyntaxKind.NumberExpression, start, text, value);
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

        if (char.IsLetter(CurrentChar))
        {
            var start = _position;
            while (char.IsLetterOrDigit(CurrentChar))
                Next();

            var length = _position - start;
            var text = _text[start.._position];

            if (SyntaxFacts.GetKeywordKind(text) is { } keywordKind && keywordKind != SyntaxKind.LiteralExpression)
                return new SyntaxToken(keywordKind, start, text, text);

            return new SyntaxToken(SyntaxKind.LiteralExpression, start, text, text);
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
    public IEnumerable<SyntaxToken> LexAll()
    {
        if (isAlreadyLexicalized)
            throw new Exception("The source program is already lexicalized");

        var tokens = new List<SyntaxToken>();
        var currToken = Lex();
        while (currToken.Kind != SyntaxKind.EofToken)
        {
            tokens.Add(currToken);
            currToken = Lex();
        }
        tokens.Add(currToken);
        return tokens;
    }
}
