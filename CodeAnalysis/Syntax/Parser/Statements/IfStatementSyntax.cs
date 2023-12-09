using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Parser;
using CodeAnalysis.Syntax.Parser.Expressions;
using CodeAnalysis.Syntax.Parser.Statements;
using CodeAnalysis.Text;

public class IfStatementSyntax : StatementSyntax
{
    public override SyntaxKind Kind => SyntaxKind.IfStatement;
    public SyntaxToken IfKeyword { get; }
    public SyntaxToken LeftParenthesis { get; }
    public ExpressionSyntax Expression { get; }
    public SyntaxToken RightParenthesis { get; }
    public StatementSyntax Statement { get; }
    public ElseStatementSyntax? ElseStatement { get; }

    public override TextSpan Span => TextSpan.FromBounds(IfKeyword.Span.Start,
                                                         ElseStatement is { } ? ElseStatement.Span.End : Statement.Span.End);

    public IfStatementSyntax(SyntaxToken ifKeyword,
                             SyntaxToken leftParenthesis,
                             ExpressionSyntax expression,
                             SyntaxToken rightParenthesis,
                             StatementSyntax statement,
                             ElseStatementSyntax? elseStatement)
    {
        IfKeyword = ifKeyword;
        LeftParenthesis = leftParenthesis;
        Expression = expression;
        RightParenthesis = rightParenthesis;
        Statement = statement;
        ElseStatement = elseStatement;
    }
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return IfKeyword;
        yield return LeftParenthesis;
        yield return Expression;
        yield return RightParenthesis;
        yield return Statement;
        if (ElseStatement is { })
            yield return ElseStatement;
    }
}
