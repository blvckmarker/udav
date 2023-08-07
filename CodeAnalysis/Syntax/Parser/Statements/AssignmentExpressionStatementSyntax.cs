using CodeAnalysis.Syntax.Parser.Expressions;

namespace CodeAnalysis.Syntax.Parser.Statements
{
    internal class AssignmentExpressionStatementSyntax : StatementSyntax
    {
        public AssignmentExpressionStatementSyntax(VariableExpressionSyntax identifier, SyntaxToken equalsToken, ExpressionSyntax expression)
        {
            Identifier = identifier;
            EqualsToken = equalsToken;
            Expression = expression;
        }

        public override SyntaxKind Kind => SyntaxKind.AssignmentExpressionStatement;
        public override int StartPosition => Identifier.StartPosition;
        public override int EndPosition => Expression.EndPosition;

        public VariableExpressionSyntax Identifier { get; }
        public SyntaxToken EqualsToken { get; }
        public ExpressionSyntax Expression { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Identifier;
            yield return EqualsToken;
            yield return Expression;
        }
    }
}