using CodeAnalysis.Syntax.Parser.Expressions;
using CodeAnalysis.Text;

namespace CodeAnalysis.Syntax.Parser.Statements;

public class WhileStatementSyntax : StatementSyntax
{
    public override SyntaxKind Kind => SyntaxKind.WhileStatement;
    public override TextSpan Span => TextSpan.FromBounds(WhileKeyword.Span.Start, Statement.Span.End);

    public SyntaxToken WhileKeyword { get; }
    public SyntaxToken LeftParenthesis { get; }
    public ExpressionSyntax Expression { get; }
    public SyntaxToken RightParenthesis { get; }
    public StatementSyntax Statement { get; }
    public WhileStatementSyntax(SyntaxToken whileKeyword, SyntaxToken leftParenthesis, ExpressionSyntax expression, SyntaxToken rightParenthesis, StatementSyntax statement)
    {
        WhileKeyword = whileKeyword;
        LeftParenthesis = leftParenthesis;
        Expression = expression;
        RightParenthesis = rightParenthesis;
        Statement = statement;
    }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return WhileKeyword;
        yield return LeftParenthesis;
        yield return Expression;
        yield return RightParenthesis;
        yield return Statement;
    }
}