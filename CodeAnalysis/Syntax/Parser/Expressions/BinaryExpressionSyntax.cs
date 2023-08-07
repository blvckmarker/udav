using CodeAnalysis.Text;

namespace CodeAnalysis.Syntax.Parser.Expressions;

public sealed class BinaryExpressionSyntax : ExpressionSyntax
{
    public BinaryExpressionSyntax(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
    {
        Left = left;
        OperatorToken = operatorToken;
        Right = right;
    }

    public ExpressionSyntax Left { get; }
    public SyntaxToken OperatorToken { get; }
    public ExpressionSyntax Right { get; }

    public override SyntaxKind Kind => SyntaxKind.BinaryExpression;

    public override TextSpan Span => TextSpan.FromBounds(Left.Span.Start, Right.Span.End);

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Left;
        yield return OperatorToken;
        yield return Right;
    }
}