using CodeAnalysis.Syntax.Parser.Expressions;

namespace CodeAnalysis.Syntax.Parser.Statements
{
    public sealed class AssignmentStatementSyntax : StatementSyntax
    {
        public AssignmentStatementSyntax(SyntaxToken letToken,
                                   ExpressionSyntax nameExpression,
                                   SyntaxToken equalToken,
                                   ExpressionSyntax expression)
        {
            LetKeyword = letToken;
            NameExpression = nameExpression;
            EqualToken = equalToken;
            Expression = expression;
        }

        public SyntaxToken LetKeyword { get; }
        public ExpressionSyntax NameExpression { get; }
        public SyntaxToken EqualToken { get; }
        public ExpressionSyntax Expression { get; }

        public override SyntaxKind Kind => SyntaxKind.AssignmentStatement;
        public override int StartPosition => LetKeyword.StartPosition;
        public override int EndPosition => Expression.EndPosition;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return LetKeyword;
            yield return NameExpression;
            yield return EqualToken;
            yield return Expression;
        }
    }
}
