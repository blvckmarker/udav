using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binder.BoundExpressions
{
    public class BoundUnaryOperator
    {
        private static BoundUnaryOperator[] _operators =
        {
            new BoundUnaryOperator(SyntaxKind.MinusToken, BoundUnaryOperatorKind.Negation, typeof(int)),
            new BoundUnaryOperator(SyntaxKind.PlusToken, BoundUnaryOperatorKind.Identity, typeof(int)),
            new BoundUnaryOperator(SyntaxKind.ExclamationToken, BoundUnaryOperatorKind.LogicalNot, typeof(bool)),
            new BoundUnaryOperator(SyntaxKind.TildeToken, BoundUnaryOperatorKind.BitwiseNot, typeof(int)),
        };

        public BoundUnaryOperator(SyntaxKind kind, BoundUnaryOperatorKind boundKind, Type operand)
        {
            SyntaxKind = kind;
            BoundKind = boundKind;
            Operand = operand;
        }

        public SyntaxKind SyntaxKind { get; }
        public BoundUnaryOperatorKind BoundKind { get; }
        public Type Operand { get; }

        internal static BoundUnaryOperator? Bind(SyntaxKind kind, Type operand)
            => _operators.FirstOrDefault(op => op.SyntaxKind == kind
                                            && op.Operand == operand);
    }
}