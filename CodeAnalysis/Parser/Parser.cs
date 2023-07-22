#region

using CodeAnalysis.Parser.Expressions;
using CodeAnalysis.Parser.Expressions.AST;
using CodeAnalysis.Scanner.Model;
using CodeAnalysis.Scanner.Shared;

#endregion

namespace CodeAnalysis.Parser;

public class Parser
{
    private readonly List<string> _diagnostics = new();
    private readonly List<SyntaxToken> _tokens = new();
    private int _position;

    public Parser(string text)
    {
        var lexer = new Lexer(text);

        var token = lexer.Lex();
        while (token.Kind is not SyntaxKind.EofToken)
        {
            if (token.Kind is not SyntaxKind.WhitespaceToken or SyntaxKind.BadToken)
                _tokens.Add(token);
            token = lexer.Lex();
        }

        _tokens.Add(token); //eof
        _diagnostics.AddRange(lexer.Diagnostics);
    }
    public Parser(Lexer lexer)
    {
        _tokens = lexer.LexAll()
            .Where(token => token.Kind is not SyntaxKind.WhitespaceToken and not SyntaxKind.BadToken)
            .ToList();
        _diagnostics.AddRange(lexer.Diagnostics);
    }


    private SyntaxToken Current => Peek(0);
    public IEnumerable<string> Diagnostics => _diagnostics;

    private SyntaxToken NextToken()
    {
        var current = Current;
        _position++;
        return current;
    }

    private SyntaxToken MatchToken(SyntaxKind kind)
    {
        if (Current.Kind == kind)
            return NextToken();
        _diagnostics.Add($"ERROR: Unexpected token <{Current.Kind}> expected <{kind}>");
        return new SyntaxToken(kind, Current.Position, null, null);
    }

    public SyntaxTree Parse()
    {
        var expression = ParseExpression();
        var eofToken = MatchToken(SyntaxKind.EofToken);
        return new SyntaxTree(_diagnostics, expression, eofToken);
    }

    private ExpressionSyntax ParseExpression(int parentPrecedence = 0)
    {
        ExpressionSyntax left;
        var operatorPrecedence = Current.Kind.GetUnaryOperatorPrecedence();

        if (operatorPrecedence != 0 && operatorPrecedence > parentPrecedence)
        {
            var operatorToken = NextToken();
            var operand = ParseExpression();

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

            var operatorToken = NextToken();
            var right = ParseExpression(precedence);

            left = new BinaryExpressionSyntax(left, operatorToken, right);
        }

        return left;
    }

    private ExpressionSyntax ParsePrimaryExpression()
    {
        if (Current.Kind is SyntaxKind.OpenParenToken)
        {
            var left = NextToken();
            var expression = ParseExpression();
            var right = MatchToken(SyntaxKind.CloseParenToken);

            return new ParenthesizedExpressionSyntax(left, expression, right);
        }

        var numberToken = MatchToken(SyntaxKind.LiteralExpression);
        return new LiteralExpressionSyntax(numberToken);
    }

    private SyntaxToken Peek(int offset)
        => _position + offset >= _tokens.Count ? _tokens.Last() : _tokens[_position + offset];
}