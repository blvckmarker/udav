using CodeAnalysis.Diagnostic;
using CodeAnalysis.Text;
using System.Collections.Immutable;

namespace CodeAnalysis.Syntax.Scanner;

public class Lexer
{
    private readonly DiagnosticsBase _diagnostics;
    private readonly SourceText _text;
    private bool isAlreadyLexicalized = false;
    private int _position;

    private object? _value;
    private SyntaxKind _kind;
    private int _startPosition;

    private char CurrentChar => _position >= _text.Length ? '\0' : _text[_position];
    public DiagnosticsBase Diagnostics => _diagnostics;

    public Lexer(string text)
    {
        _text = SourceText.From(text);
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
                _kind = SyntaxKind.PlusToken;
                _position++;
                break;
            case '-':
                _kind = SyntaxKind.MinusToken;
                _position++;
                break;
            case '/':
                _kind = SyntaxKind.SlashToken;
                _position++;
                break;
            case '*':
                _kind = SyntaxKind.AsteriskToken;
                _position++;
                break;
            case '(':
                _kind = SyntaxKind.OpenParenToken;
                _position++;
                break;
            case ')':
                _kind = SyntaxKind.CloseParenToken;
                _position++;
                break;
            case '%':
                _kind = SyntaxKind.PercentToken;
                _position++;
                break;
            case '~':
                _kind = SyntaxKind.TildeToken;
                _position++;
                break;
            case '^':
                _kind = SyntaxKind.CaretToken;
                _position++;
                break;
            case '>':
                if (Lookahead(1) == '=')
                {
                    _kind = SyntaxKind.GreaterThanEqualToken;
                    _position += 2;
                    break;
                }
                _kind = SyntaxKind.GreaterThanToken;
                _position++;
                break;
            case '<':
                if (Lookahead(1) == '=')
                {
                    _kind = SyntaxKind.LessThanEqualToken;
                    _position += 2;
                    break;
                }
                _kind = SyntaxKind.LessThanToken;
                _position++;
                break;
            case '&':
                if (Lookahead(1) == '&')
                {
                    _kind = SyntaxKind.AmpersandAmpersandToken;
                    _position += 2;
                    break;
                }
                _kind = SyntaxKind.AmpersandToken;
                _position++;
                break;
            case '|':
                if (Lookahead(1) == '|')
                {
                    _position += 2;
                    _kind = SyntaxKind.PipePipeToken;
                    break;
                }
                _kind = SyntaxKind.PipeToken;
                _position++;
                break;
            case '!':
                if (Lookahead(1) == '=')
                {
                    _position += 2;
                    _kind = SyntaxKind.ExclamationEqualsToken;
                    break;
                }
                _kind = SyntaxKind.ExclamationToken;
                _position++;
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
                    _diagnostics.MakeIssue($"Bad character input: {CurrentChar}", CurrentChar.ToString(), TextSpan.FromBounds(_startPosition, _position));
                    _kind = SyntaxKind.BadToken;

                    _position++;
                }
                break;
        }

        var span = _text[_startPosition, _position];
        return new SyntaxToken(_kind, _startPosition, span, _value);
    }

    private void LexIdentifierOrKeywordToken()
    {
        while (char.IsLetterOrDigit(CurrentChar))
            Next();

        var span = _text[_startPosition, _position];
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

        var span = _text[_startPosition, _position];
        if (!int.TryParse(span, out var value))
            _diagnostics.MakeIssue($"The number {span} cannot be represented as int32", span, TextSpan.FromBounds(_startPosition, _position));

        _value = value;
        _kind = SyntaxKind.NumericToken;
    }

    public IImmutableList<SyntaxToken> LexAll()
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
        return tokens.ToImmutableArray();
    }
}
