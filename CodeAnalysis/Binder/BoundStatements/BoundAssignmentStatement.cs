using CodeAnalysis.Binder.BoundExpressions;
using CodeAnalysis.Syntax.Parser.Expressions;

namespace CodeAnalysis.Binder.BoundStatements
{
    public sealed class BoundAssignmentStatement : BoundStatement
    {
        public BoundAssignmentStatement(LiteralExpressionSyntax variableName, BoundExpression boundExpression)
        {
            VariableName = variableName;
            BoundExpression = boundExpression;
        }

        public LiteralExpressionSyntax VariableName { get; }
        public BoundExpression BoundExpression { get; }
        public Type Type => BoundExpression.Type;

        public override BoundNodeKind Kind => BoundNodeKind.AssignmentStatement;
    }

}

