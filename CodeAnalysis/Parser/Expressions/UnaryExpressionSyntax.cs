using CodeAnalysis.Parser.Expressions.AST;
using CodeAnalysis.Scanner.Model;

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

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return OperatorToken;
        yield return Operand;
    }

}