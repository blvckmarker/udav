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
    private readonly List<SyntaxToken> _tokens = new();
    private int _position;

    public Parser(string text) : this(new Lexer(text)) { }
    public Parser(Lexer lexer)
    {
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
        return new SyntaxToken(kind, Current.StartPosition, Current.EndPosition, null, null);
    }

    public SyntaxTree ParseTree()
    {
        var statement = ParseAssignmentStatement();
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

    private StatementSyntax ParseAssignmentStatement()
    {
        var letToken = MatchToken(SyntaxKind.LetKeyword);
        var nameExpression = ParseNameExpression();
        var equalToken = MatchToken(SyntaxKind.EqualToken);
        var expression = ParseExpression();
        return new AssignmentStatementSyntax(letToken, nameExpression, equalToken, expression);
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

    private ExpressionSyntax ParseBooleanExpression()
    {
        var booleanValue = Current.Kind == SyntaxKind.TrueKeyword;
        var matchToken = booleanValue ? MatchToken(SyntaxKind.TrueKeyword) : MatchToken(SyntaxKind.FalseKeyword);
        return new LiteralExpressionSyntax(matchToken, booleanValue);
    }

    private ExpressionSyntax ParseNumericExpression()
    {
        var numberToken = MatchToken(SyntaxKind.NumericExpression);
        return new LiteralExpressionSyntax(numberToken, (int)numberToken.Value);
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