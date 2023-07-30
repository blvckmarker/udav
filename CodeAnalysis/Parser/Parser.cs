#region

using CodeAnalysis.Parser.Expressions;
using CodeAnalysis.Parser.Syntax;
using CodeAnalysis.Scanner.Shared;
using CodeAnalysis.Scanner.Syntax;
using CodeAnalysis.Text;

#endregion

namespace CodeAnalysis.Parser;

public class Parser
{
    private readonly DiagnosticsBase _diagnostics;
    private readonly List<SyntaxToken> _tokens = new();
    private readonly string _text;
    private int _position;

    public Parser(string text) : this(new Lexer(text)) { }
    public Parser(Lexer lexer)
    {
        _tokens = lexer.LexAll()
            .Where(token => token.Kind is not SyntaxKind.WhitespaceToken and not SyntaxKind.BadToken)
            .ToList();
        _diagnostics = lexer.Diagnostics;
        _text = lexer.SourceText;
    }

    private SyntaxToken Current => Peek(0);
    public DiagnosticsBase Diagnostics => _diagnostics;

    private SyntaxToken TakeToken()
    {
        var current = Current;
        _position++;
        return current;
    }

    private SyntaxToken MatchToken(SyntaxKind kind)
    {
        if (Current.Kind == kind)
            return TakeToken();

        var start = Current.Position;
        var end = Current.Position + Current.Text.Length;
        var problemText = _text[start..end];
        _diagnostics.MakeIssue($"Unexpected token <{Current.Kind}> expected <{kind}>", problemText, start);
        return new SyntaxToken(kind, Current.Position, null, null);
    }

    public SyntaxTree Parse()
    {
        var expression = ParseExpression();
        var eofToken = MatchToken(SyntaxKind.EofToken);
        return new SyntaxTree(_diagnostics, expression, eofToken);
    }

    /* expression : 
     *            | ('-' | '+') expression
     *            | expression op=('*' | '+') expression
     *            | expression op=('+' | '-') expression
     *            | '!' expression
     *            | expression op=('&&'| '||') expression
     *            | primary
     * 
     * 
     * 
     * primary : '(' expression ')'
     *         | name
     *         | number
     *         | boolean
     */
    private ExpressionSyntax ParseExpression(int parentPrecedence = 0) // false || !false && false -> false || true && false -> false || false -> false
    {
        ExpressionSyntax left;
        var unaryOperatorPrecedence = Current.Kind.GetUnaryOperatorPrecedence();

        if (unaryOperatorPrecedence != 0 && unaryOperatorPrecedence > parentPrecedence)
        {
            var operatorToken = TakeToken();
            var operand = ParseExpression(unaryOperatorPrecedence);

            left = new UnaryExpressionSyntax(operatorToken, operand);
        }
        else
        {
            left = ParsePrimaryExpression();
        }

        while (true)
        {
            var precedence = Current.Kind.GetBinaryOperatorPrecedence();

            if (precedence == 0 || precedence <= parentPrecedence)
                break;

            var operatorToken = TakeToken();
            var right = ParseExpression(precedence);

            left = new BinaryExpressionSyntax(left, operatorToken, right);
        }

        return left;
    }

    private ExpressionSyntax ParsePrimaryExpression() // literal is atom
    {
        switch (Current.Kind)
        {
            case SyntaxKind.OpenParenToken:
                return ParseParenthesizedExpression();

            case SyntaxKind.TrueKeyword:
            case SyntaxKind.FalseKeyword:
                return ParseBooleanExpression();

            case SyntaxKind.NumberExpression:
                return ParseNumberExpression();

            default:
                return ParseNameExpression();
        }
    }

    private ExpressionSyntax ParseBooleanExpression()
    {
        var booleanValue = Current.Kind == SyntaxKind.TrueKeyword;
        var matchToken = booleanValue ? MatchToken(SyntaxKind.TrueKeyword) : MatchToken(SyntaxKind.FalseKeyword);
        return new LiteralExpressionSyntax(matchToken, booleanValue);
    }

    private ExpressionSyntax ParseNumberExpression()
    {
        var numberToken = MatchToken(SyntaxKind.NumberExpression);
        return new LiteralExpressionSyntax(numberToken);
    }

    private ExpressionSyntax ParseNameExpression()
    {
        var literalToken = MatchToken(SyntaxKind.NameExpression);
        return new LiteralExpressionSyntax(literalToken);
    }

    private ExpressionSyntax ParseParenthesizedExpression()
    {
        var left = TakeToken();
        var expression = ParseExpression();
        var right = MatchToken(SyntaxKind.CloseParenToken);
        return new ParenthesizedExpressionSyntax(left, expression, right);
    }

    private SyntaxToken Peek(int offset)
        => _position + offset >= _tokens.Count ? _tokens.Last() : _tokens[_position + offset];
}