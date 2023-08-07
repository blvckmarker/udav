#region
using CodeAnalysis.Text;
#endregion

namespace CodeAnalysis.Syntax.Scanner;

public class Lexer
{
    private readonly DiagnosticsBase _diagnostics;
    private readonly string _text;
    private bool isAlreadyLexicalized = false;
    private int _position;

    private object? _value;
    private SyntaxKind _kind;
    private int _startPosition;

    private char CurrentChar => _position >= _text.Length ? '\0' : _text[_position];
    public DiagnosticsBase Diagnostics => _diagnostics;

    public Lexer(string text)
    {
        _text = text;
        _diagnostics = new Diagnostics(text);
    }
    private void Next() => _position++;
    private char Lookahead(int count) => _position + count >= _text.Length ? _text.Last() : _text[_position + count];

    public SyntaxToken Lex()
    {
        if (isAlreadyLexicalized)
            throw new Exception("The source program is already lexicalized");

        _value = null;
        _startPosition = _position;
        _kind = SyntaxKind.BadToken;

        switch (CurrentChar)
        {
            case '\0':
                _kind = SyntaxKind.EofToken;
                isAlreadyLexicalized = true;
                break;
            case '+':
                _position++;
                _kind = SyntaxKind.PlusToken;
                break;
            case '-':
                _position++;
                _kind = SyntaxKind.MinusToken;
                break;
            case '/':
                _position++;
                _kind = SyntaxKind.SlashToken;
                break;
            case '*':
                _position++;
                _kind = SyntaxKind.AsteriskToken;
                break;
            case '(':
                _position++;
                _kind = SyntaxKind.OpenParenToken;
                break;
            case ')':
                _position++;
                _kind = SyntaxKind.CloseParenToken;
                break;
            case '%':
                _position++;
                _kind = SyntaxKind.PercentToken;
                break;
            case '~':
                _position++;
                _kind = SyntaxKind.TildeToken;
                break;
            case '^':
                _position++;
                _kind = SyntaxKind.CaretToken;
                break;
            case '>':
                if (Lookahead(1) == '=')
                {
                    _position += 2;
                    _kind = SyntaxKind.GreaterThanEqualToken;
                    break;
                }
                _position++;
                _kind = SyntaxKind.GreaterThanToken;
                break;
            case '<':
                if (Lookahead(1) == '=')
                {
                    _position += 2;
                    _kind = SyntaxKind.LessThanEqualToken;
                    break;
                }
                _position++;
                _kind = SyntaxKind.LessThanToken;
                break;
            case '&':
                if (Lookahead(1) == '&')
                {
                    _position += 2;
                    _kind = SyntaxKind.AmpersandAmpersandToken;
                    break;
                }
                _position++;
                _kind = SyntaxKind.AmpersandToken;
                break;
            case '|':
                if (Lookahead(1) == '|')
                {
                    _position += 2;
                    _kind = SyntaxKind.PipePipeToken;
                    break;
                }
                _position++;
                _kind = SyntaxKind.PipeToken;
                break;
            case '!':
                if (Lookahead(1) == '=')
                {
                    _position += 2;
                    _kind = SyntaxKind.ExclamationEqualsToken;
                    break;
                }
                _position++;
                _kind = SyntaxKind.ExclamationToken;
                break;
            case '=':
                if (Lookahead(1) == '=')
                {
                    _position += 2;
                    _kind = SyntaxKind.EqualsEqualsToken;
                    break;
                }
                _position++;
                _kind = SyntaxKind.EqualsToken;
                break;

            case '0':
            case '1':
            case '2':
            case '3':
            case '4':
            case '5':
            case '6':
            case '7':
            case '8':
            case '9':
                LexNumericToken();
                break;

            case ' ':
            case '\n':
            case '\r':
            case '\t':
                LexWhitespaceToken();
                break;

            default:
                if (char.IsLetter(CurrentChar))
                    LexIdentifierOrKeywordToken();
                else if (char.IsWhiteSpace(CurrentChar))
                    LexWhitespaceToken();
                else
                {
                    _position++;
                    _diagnostics.MakeIssue($"Bad character input: {CurrentChar}", CurrentChar.ToString(), _startPosition);
                    _kind = SyntaxKind.BadToken;
                }
                break;
        }

        var span = _text[_startPosition.._position];
        return new SyntaxToken(_kind, _startPosition, span, _value);
    }

    private void LexIdentifierOrKeywordToken()
    {
        while (char.IsLetterOrDigit(CurrentChar))
            Next();

        var span = _text[_startPosition.._position];
        _kind = SyntaxFacts.GetKeywordKind(span);
    }

    private void LexWhitespaceToken()
    {
        while (char.IsWhiteSpace(CurrentChar))
            Next();

        _kind = SyntaxKind.WhitespaceToken;
    }

    private void LexNumericToken()
    {
        while (char.IsDigit(CurrentChar))
            Next();

        var span = _text[_startPosition.._position];
        if (!int.TryParse(span, out var value))
            _diagnostics.MakeIssue($"The number {span} cannot be represented as int32", span, _startPosition);

        _value = value;
        _kind = SyntaxKind.NumericToken;
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
