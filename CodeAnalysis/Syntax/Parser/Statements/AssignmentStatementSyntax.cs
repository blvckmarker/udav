using CodeAnalysis.Syntax.Parser.Expressions;

namespace CodeAnalysis.Syntax.Parser.Statements
{
    public sealed class AssignmentStatementSyntax : StatementSyntax
    {
        public AssignmentStatementSyntax(SyntaxToken typeToken,
                                   DeclaredVariableExpressionSyntax identifierName,
                                   SyntaxToken equalToken,
                                   ExpressionSyntax expression)
        {
            TypeToken = typeToken;
            IdentifierName = identifierName;
            EqualToken = equalToken;
            Expression = expression;
        }

        public SyntaxToken TypeToken { get; }
        public DeclaredVariableExpressionSyntax IdentifierName { get; }
        public SyntaxToken EqualToken { get; }
        public ExpressionSyntax Expression { get; }

        public override SyntaxKind Kind => SyntaxKind.AssignmentStatement;
        public override int StartPosition => TypeToken.StartPosition;
        public override int EndPosition => Expression.EndPosition;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return TypeToken;
            yield return IdentifierName;
            yield return EqualToken;
            yield return Expression;
        }
    }
}
