namespace CodeAnalysis.Syntax.Parser.Expressions
{
    internal class AssignmentExpressionSyntax : ExpressionSyntax
    {
        public AssignmentExpressionSyntax(VariableExpressionSyntax name, SyntaxToken equalsToken, ExpressionSyntax expression)
        {
            Name = name;
            EqualsToken = equalsToken;
            Expression = expression;
        }

        public override SyntaxKind Kind => SyntaxKind.AssignmentExpression;
        public override int StartPosition => Name.StartPosition;
        public override int EndPosition => Expression.EndPosition;

        public VariableExpressionSyntax Name { get; }
        public SyntaxToken EqualsToken { get; }
        public ExpressionSyntax Expression { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Name;
            yield return EqualsToken;
            yield return Expression;
        }
    }
}