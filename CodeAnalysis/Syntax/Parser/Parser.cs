using CodeAnalysis.Diagnostic;
using CodeAnalysis.Syntax.Parser.Expressions;
using CodeAnalysis.Syntax.Parser.Statements;
using CodeAnalysis.Syntax.Scanner;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace CodeAnalysis.Syntax.Parser;

public class Parser
{
    private readonly DiagnosticsBase _diagnostics;
    private readonly IImmutableList<SyntaxToken> _tokens;
    private int _position;

    private SyntaxToken Current => Peek(0);
    public DiagnosticsBase Diagnostics => _diagnostics;

    public Parser(Lexer lexer)
    {
        _tokens = lexer.LexAll()
                       .Where(token => token.Kind is not SyntaxKind.WhitespaceToken)
                       .ToImmutableArray();

        _diagnostics = lexer.Diagnostics;
    }
    public Parser(IEnumerable<SyntaxToken> tokens, DiagnosticsBase diagnostics)
    {
        _tokens = tokens
                  .Where(x => x.Kind != SyntaxKind.WhitespaceToken)
                  .ToImmutableArray();
        _diagnostics = diagnostics;
    }

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

        _diagnostics.MakeIssue($"Unexpected token <{Current.Kind}> expected <{kind}>", Current.Text, Current.Span);
        return new SyntaxToken(kind, Current.Span, Current.Text, null);
    }
    private SyntaxToken MatchTypeIdentifier()
    {
        switch (Current.Kind)
        {
            case SyntaxKind.LetKeyword:
            case SyntaxKind.IntKeyword:
            case SyntaxKind.BoolKeyword:
            case SyntaxKind.IdentifierToken:
                return TakeToken();
            default:
                _diagnostics.MakeIssue($"Unexpected token <{Current.Kind}> expected <IdentifierToken>", Current.Text, Current.Span);
                return new SyntaxToken(Current.Kind, Current.Span, Current.Text, null);
        }
    }

    public SyntaxTree ParseTree()
    {
        var compilationUnit = ParseCompilationUnit();
        var eofToken = MatchToken(SyntaxKind.EofToken);
        return new SyntaxTree(Diagnostics, compilationUnit, eofToken);
    }

    public CompilationUnit ParseCompilationUnit()
    {
        var statement = ParseStatement();
        return new CompilationUnit(statement);
    }


    private StatementSyntax ParseStatement()
    {
        switch (Peek().Kind)
        {
            case SyntaxKind.OpenBrace:
                return ParseBlockStatement();
            case SyntaxKind.WhileKeyword:
                return ParseWhileStatement();
            case SyntaxKind.ForKeyword:
                return ParseForStatement();
            case SyntaxKind.IfKeyword:
                return ParseIfStatement();
            case SyntaxKind.BoolKeyword:
            case SyntaxKind.IntKeyword:
            case SyntaxKind.LetKeyword:
            case SyntaxKind.IdentifierToken when Peek(1).Kind == SyntaxKind.IdentifierToken:
                return ParseAssignmentStatement();
            default:
                return ParseAssignmentExpressionStatement();
        }
    }

    private StatementSyntax ParseForStatement()
    {
        var forKeyword = MatchToken(SyntaxKind.ForKeyword);
        var leftParenthesis = MatchToken(SyntaxKind.OpenParenToken);
        var firstStatement = ParseStatement();
        MatchToken(SyntaxKind.SemicolonToken);
        var expression = ParseExpression();
        MatchToken(SyntaxKind.SemicolonToken);
        var secondStatement = ParseStatement();
        var rightParenthesis = MatchToken(SyntaxKind.CloseParenToken);
        var statement = ParseStatement();

        return new ForStatementSyntax(forKeyword, leftParenthesis, firstStatement, expression, secondStatement, rightParenthesis, statement);
    }

    private StatementSyntax ParseWhileStatement()
    {
        var whileKeyword = MatchToken(SyntaxKind.WhileKeyword);
        var leftParenthesis = MatchToken(SyntaxKind.OpenParenToken);
        var expression = ParseExpression();
        var rightParenthesis = MatchToken(SyntaxKind.CloseParenToken);
        var statement = ParseStatement();

        return new WhileStatementSyntax(whileKeyword, leftParenthesis, expression, rightParenthesis, statement);
    }

    private StatementSyntax ParseIfStatement()
    {
        var ifKeyword = MatchToken(SyntaxKind.IfKeyword);
        var leftParenthesis = MatchToken(SyntaxKind.OpenParenToken);
        var expression = ParseExpression();
        var rightParenthesis = MatchToken(SyntaxKind.CloseParenToken);
        var statement = ParseStatement();

        ElseStatementSyntax? elseStatement = null;
        if (Peek().Kind == SyntaxKind.ElseKeyword)
        {
            var elseKeyword = MatchToken(SyntaxKind.ElseKeyword);
            var elseBlockStatement = ParseStatement();

            elseStatement = new ElseStatementSyntax(elseKeyword, elseBlockStatement);
        }

        return new IfStatementSyntax(ifKeyword, leftParenthesis, expression, rightParenthesis, statement, elseStatement);
    }
    private StatementSyntax ParseBlockStatement()
    {
        if (Peek().Kind == SyntaxKind.OpenBrace)
        {
            var statements = new List<StatementSyntax>();
            var openBrace = MatchToken(SyntaxKind.OpenBrace);

            while (Peek().Kind != SyntaxKind.CloseBrace && Peek().Kind != SyntaxKind.EofToken)
                statements.Add(ParseStatement());

            var closeBrace = MatchToken(SyntaxKind.CloseBrace);
            return new BlockStatementSyntax(openBrace, statements, closeBrace);
        }
        return ParseStatement();
    }
    private StatementSyntax ParseAssignmentStatement()
    {
        var typeToken = MatchTypeIdentifier();
        var identifier = MatchToken(SyntaxKind.IdentifierToken);
        var equalToken = MatchToken(SyntaxKind.EqualsToken);
        var expression = ParseExpression();

        return new AssignmentStatementSyntax(typeToken, identifier, equalToken, expression);
    }

    private StatementSyntax ParseAssignmentExpressionStatement()
    {
        var assignmentExpression = (AssignmentExpressionSyntax)ParseAssignmentExpression();

        return new AssignmentExpressionStatementSyntax(
            assignmentExpression.Variable,
            assignmentExpression.EqualsToken,
            assignmentExpression.Expression);
    }

    private ExpressionSyntax ParseExpression()
    {
        if (Peek().Kind == SyntaxKind.IdentifierToken &&
            Peek(1).Kind == SyntaxKind.EqualsToken)
            return ParseAssignmentExpression();

        return ParseBinaryExpression();
    }

    private ExpressionSyntax ParseAssignmentExpression()
    {
        var name = ParseNameExpression();
        var equalsToken = MatchToken(SyntaxKind.EqualsToken);
        var expression = ParseExpression();
        return new AssignmentExpressionSyntax(name, equalsToken, expression);
    }

    private ExpressionSyntax ParseBinaryExpression(int parentPrecedence = 0)
    {
        ExpressionSyntax left;
        var unaryOperatorPrecedence = Current.Kind.GetUnaryOperatorPrecedence();

        if (unaryOperatorPrecedence != 0 && unaryOperatorPrecedence > parentPrecedence)
        {
            var operatorToken = TakeToken();
            var operand = ParseBinaryExpression(unaryOperatorPrecedence);

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
            var right = ParseBinaryExpression(precedence);

            left = new BinaryExpressionSyntax(left, operatorToken, right);
        }

        return left;
    }

    private ExpressionSyntax ParsePrimaryExpression()
    {
        switch (Current.Kind)
        {
            case SyntaxKind.OpenParenToken:
                return ParseParenthesizedExpression();

            case SyntaxKind.TrueKeyword:
            case SyntaxKind.FalseKeyword:
                return ParseBooleanExpression();

            case SyntaxKind.NumericToken:
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
        var numberToken = MatchToken(SyntaxKind.NumericToken);
        return new LiteralExpressionSyntax(numberToken, (int)numberToken.Value!);
    }

    private VariableExpressionSyntax ParseNameExpression()
    {
        var literalToken = MatchToken(SyntaxKind.IdentifierToken);
        return new VariableExpressionSyntax(literalToken);
    }

    private ParenthesizedExpressionSyntax ParseParenthesizedExpression()
    {
        var left = TakeToken();
        var expression = ParseBinaryExpression();
        var right = MatchToken(SyntaxKind.CloseParenToken);
        return new ParenthesizedExpressionSyntax(left, expression, right);
    }

    private SyntaxToken Peek(int offset = 0)
        => _position + offset >= _tokens.Count ? _tokens.Last() : _tokens[_position + offset];
}
