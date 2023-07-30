using CodeAnalysis.Parser.Syntax;
using CodeAnalysis.Scanner.Syntax;

namespace CodeAnalysis.Parser.Expressions;

public sealed class UnaryExpressionSyntax : ExpressionSyntax
{
    public UnaryExpressionSyntax(SyntaxToken operatorToken, ExpressionSyntax operand)
    {
        OperatorToken = operatorToken;
        Operand = operand;
    }

    public ExpressionSyntax Operand { get; }
    public SyntaxToken OperatorToken { get; }

    public override SyntaxKind Kind => SyntaxKind.UnaryExpression;

    public override int StartPosition => OperatorToken.StartPosition;
    public override int EndPosition => Operand.EndPosition;

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return OperatorToken;
        yield return Operand;
    }

}