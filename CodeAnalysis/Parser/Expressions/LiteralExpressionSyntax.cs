#region

using CodeAnalysis.Lexer.Model;
using CodeAnalysis.Parser.Expressions.AST;

#endregion

namespace CodeAnalysis.Parser.Expressions;

public sealed class LiteralExpressionSyntax : ExpressionSyntax
{
    public LiteralExpressionSyntax(SyntaxToken literalToken)
    {
        LiteralToken = literalToken;
    }

    public SyntaxToken LiteralToken { get; }
    public override SyntaxKind Kind => SyntaxKind.LiteralExpression;

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return LiteralToken;
    }
}