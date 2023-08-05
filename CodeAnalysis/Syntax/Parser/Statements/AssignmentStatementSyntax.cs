using CodeAnalysis.Syntax.Parser.Expressions;

namespace CodeAnalysis.Syntax.Parser.Statements
{
    public sealed class AssignmentStatementSyntax : StatementSyntax
    {
        public AssignmentStatementSyntax(SyntaxToken typeToken,
                                   SyntaxToken identifier,
                                   SyntaxToken equalToken,
                                   ExpressionSyntax expression)
        {
            TypeToken = typeToken;
            IdentifierToken = identifier;
            EqualToken = equalToken;
            Expression = expression;
        }

        public SyntaxToken TypeToken { get; }
        public SyntaxToken IdentifierToken { get; }
        public SyntaxToken EqualToken { get; }
        public ExpressionSyntax Expression { get; }

        public override SyntaxKind Kind => SyntaxKind.AssignmentStatement;
        public override int StartPosition => TypeToken.StartPosition;
        public override int EndPosition => Expression.EndPosition;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return TypeToken;
            yield return IdentifierToken;
            yield return EqualToken;
            yield return Expression;
        }
    }
}
