#region
using CodeAnalysis.Parser.Syntax;
using CodeAnalysis.Scanner.Syntax;
using CodeAnalysis.Text;
#endregion

namespace CodeAnalysis.Scanner.Shared;

public class Lexer
{
    private readonly DiagnosticsBase _diagnostics = new Diagnostics();
    private readonly string _text;
    private bool isAlreadyLexicalized = false;
    private int _position;

    private char CurrentChar => _position >= _text.Length ? '\0' : _text[_position];
    public DiagnosticsBase Diagnostics => _diagnostics;

    public Lexer(string text)
    {
        _text = text;
    }
    private void Next() => _position++;
    private char Lookahead(int count) => _position + count >= _text.Length ? _text.Last() : _text[_position + count];

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
                _diagnostics.MakeIssue($"ERROR: The number {text} cannot be represented as int32", text, start);

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

        switch (CurrentChar) // &&  || !
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
            case '!':
                return new SyntaxToken(SyntaxKind.ExclamationToken, _position++, "!", null);
            case '^':
                return new SyntaxToken(SyntaxKind.CaretToken, _position++, "^", null);
            case '&':
                if (Lookahead(1) == '&')
                {
                    _position += 2;
                    return new SyntaxToken(SyntaxKind.AmpersandAmpersandToken, _position, "&&", null);
                }
                return new SyntaxToken(SyntaxKind.AmpersandToken, _position++, "&", null);

            case '|':
                if (Lookahead(1) == '|')
                {
                    _position += 2;
                    return new SyntaxToken(SyntaxKind.PipePipeToken, _position, "||", null);
                }
                return new SyntaxToken(SyntaxKind.PipeToken, _position++, "|", null);

            default:
                _diagnostics.MakeIssue($"Bad character input: {CurrentChar}", CurrentChar.ToString(), _position);
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
