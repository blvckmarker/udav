using CodeAnalysis.Syntax.Parser.Expressions;
using CodeAnalysis.Text;

namespace CodeAnalysis.Syntax.Parser.Statements;

public class ForStatementSyntax : StatementSyntax
{
    public ForStatementSyntax(SyntaxToken forKeyword, SyntaxToken leftParenthesis, StatementSyntax firstStatement, ExpressionSyntax expression, StatementSyntax secondStatement, SyntaxToken rightParenthesis, StatementSyntax statement)
    {
        ForKeyword = forKeyword;
        LeftParenthesis = leftParenthesis;
        DeclarationStatement = firstStatement;
        Expression = expression;
        AssignmentStatement = secondStatement;
        RightParenthesis = rightParenthesis;
        Statement = statement;
    }
    public SyntaxToken ForKeyword { get; }
    public SyntaxToken LeftParenthesis { get; }
    public StatementSyntax DeclarationStatement { get; }
    public ExpressionSyntax Expression { get; }
    public StatementSyntax AssignmentStatement { get; }
    public SyntaxToken RightParenthesis { get; }
    public StatementSyntax Statement { get; }

    public override SyntaxKind Kind => SyntaxKind.ForStatement;
    public override TextSpan Span => TextSpan.FromBounds(ForKeyword.Span.Start, Statement.Span.End);

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return ForKeyword;
        yield return LeftParenthesis;
        yield return DeclarationStatement;
        yield return Expression;
        yield return AssignmentStatement;
        yield return RightParenthesis;
        yield return Statement;
    }
}