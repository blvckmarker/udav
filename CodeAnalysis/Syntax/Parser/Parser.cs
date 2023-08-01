#region

using CodeAnalysis.Syntax.Parser.Expressions;
using CodeAnalysis.Syntax.Parser.Statements;
using CodeAnalysis.Syntax.Scanner;
using CodeAnalysis.Text;

#endregion

namespace CodeAnalysis.Syntax.Parser;

public class Parser
{
    private readonly DiagnosticsBase _diagnostics;
    private readonly IList<SyntaxToken> _tokens;
    private int _position;

    public Parser(Lexer lexer)
    {
        if (lexer.Diagnostics.Where(x => x.Kind == IssueKind.Problem).Any())
            throw new Exception("Unable to create syntax tree when lexer has problem issues");

        _tokens = lexer.LexAll()
            .Where(token => token.Kind is not SyntaxKind.WhitespaceToken)
            .ToList();
        _diagnostics = lexer.Diagnostics;
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

        _diagnostics.MakeIssue($"Unexpected token <{Current.Kind}> expected <{kind}>", Current.Text, Current.StartPosition);
        return new SyntaxToken(kind, Current.StartPosition, Current.EndPosition, Current.Text, null);
    }

    public SyntaxTree ParseTree()
    {
        var statement = ParseStatement();
        var eofToken = MatchToken(SyntaxKind.EofToken);
        return new SyntaxTree(_diagnostics, statement, eofToken);
    }

    /*
     * 
     * 
     * assignment_statement : LET_KEYWORD name EQUALTOKEN expression
     * 
     * 
     * expression : 
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

    private StatementSyntax ParseStatement()
    {
        var statement = ParseAssignmentStatement();
        return statement;
    }

    private StatementSyntax ParseAssignmentStatement()
    {
        var letToken = MatchToken(SyntaxKind.LetKeyword);
        var identifierName = MatchToken(SyntaxKind.NameExpression);
        var equalToken = MatchToken(SyntaxKind.EqualsToken);
        var expression = ParseExpression();
        return new AssignmentStatementSyntax(letToken, identifierName, equalToken, expression);
    }



    private ExpressionSyntax ParseExpression(int parentPrecedence = 0)
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

            case SyntaxKind.NumericExpression:
                return ParseNumericExpression();

            default:
                return ParseNameExpression();
        }
    }

    private LiteralExpressionSyntax ParseBooleanExpression()
    {
        var booleanValue = Current.Kind == SyntaxKind.TrueKeyword;
        var matchToken = booleanValue ? MatchToken(SyntaxKind.TrueKeyword) : MatchToken(SyntaxKind.FalseKeyword);
        return new LiteralExpressionSyntax(matchToken, booleanValue);
    }

    private LiteralExpressionSyntax ParseNumericExpression()
    {
        var numberToken = MatchToken(SyntaxKind.NumericExpression);
        return new LiteralExpressionSyntax(numberToken, (int)numberToken.Value!);
    }

    private LiteralExpressionSyntax ParseNameExpression()
    {
        var literalToken = MatchToken(SyntaxKind.NameExpression);
        return new LiteralExpressionSyntax(literalToken);
    }

    private ParenthesizedExpressionSyntax ParseParenthesizedExpression()
    {
        var left = TakeToken();
        var expression = ParseExpression();
        var right = MatchToken(SyntaxKind.CloseParenToken);
        return new ParenthesizedExpressionSyntax(left, expression, right);
    }

    private SyntaxToken Peek(int offset)
        => _position + offset >= _tokens.Count ? _tokens.Last() : _tokens[_position + offset];
}