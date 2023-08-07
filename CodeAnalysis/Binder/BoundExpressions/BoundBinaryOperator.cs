using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binder.BoundExpressions;

public sealed class BoundBinaryOperator
{
    private static BoundBinaryOperator[] _operators = new[]
    {
        new BoundBinaryOperator(SyntaxKind.AsteriskToken, BoundBinaryOperatorKind.Multiplication,typeof(int), typeof(int)),
        new BoundBinaryOperator(SyntaxKind.SlashToken, BoundBinaryOperatorKind.Division, typeof(int), typeof(int)),
        new BoundBinaryOperator(SyntaxKind.MinusToken, BoundBinaryOperatorKind.Subtraction, typeof(int), typeof(int)),
        new BoundBinaryOperator(SyntaxKind.PlusToken, BoundBinaryOperatorKind.Addition, typeof(int), typeof(int)),
        new BoundBinaryOperator(SyntaxKind.PercentToken, BoundBinaryOperatorKind.DivisionRemainder, typeof(int), typeof(int)),

        new BoundBinaryOperator(SyntaxKind.PipePipeToken, BoundBinaryOperatorKind.LogicalOr, typeof(bool), typeof(bool)),
        new BoundBinaryOperator(SyntaxKind.AmpersandAmpersandToken, BoundBinaryOperatorKind.LogicalAnd, typeof(bool), typeof(bool)),
        new BoundBinaryOperator(SyntaxKind.EqualsEqualsToken, BoundBinaryOperatorKind.Equals, typeof(int), typeof(bool)),
        new BoundBinaryOperator(SyntaxKind.EqualsEqualsToken, BoundBinaryOperatorKind.Equals, typeof(bool), typeof(bool)),
        new BoundBinaryOperator(SyntaxKind.ExclamationEqualsToken, BoundBinaryOperatorKind.NotEqual, typeof(int), typeof(bool)),
        new BoundBinaryOperator(SyntaxKind.ExclamationEqualsToken, BoundBinaryOperatorKind.NotEqual, typeof(bool), typeof(bool)),
        new BoundBinaryOperator(SyntaxKind.LessThanEqualToken, BoundBinaryOperatorKind.LessOrEqual, typeof(int), typeof(bool)),
        new BoundBinaryOperator(SyntaxKind.LessThanToken, BoundBinaryOperatorKind.LessThan, typeof(int), typeof(bool)),
        new BoundBinaryOperator(SyntaxKind.GreaterThanEqualToken, BoundBinaryOperatorKind.GreaterOrEqual, typeof(int), typeof(bool)),
        new BoundBinaryOperator(SyntaxKind.GreaterThanToken, BoundBinaryOperatorKind.GreaterThan, typeof(int), typeof(bool)),

        new BoundBinaryOperator(SyntaxKind.PipeToken, BoundBinaryOperatorKind.BitwiseOr, typeof(int), typeof(int)),
        new BoundBinaryOperator(SyntaxKind.AmpersandToken, BoundBinaryOperatorKind.BitwiseAnd, typeof(int), typeof(int)),
        new BoundBinaryOperator(SyntaxKind.CaretToken, BoundBinaryOperatorKind.BitwiseXor, typeof(int), typeof(int)),
    };

    public SyntaxKind SyntaxKind { get; }
    public BoundBinaryOperatorKind BoundKind { get; }
    public Type LeftType { get; }
    public Type RightType { get; }
    public Type ResultType { get; }


    public BoundBinaryOperator(SyntaxKind syntaxKind, BoundBinaryOperatorKind kind, Type operandType, Type resultType)
        : this(syntaxKind, kind, operandType, operandType, resultType) { }
    public BoundBinaryOperator(SyntaxKind syntaxKind, BoundBinaryOperatorKind kind, Type leftType, Type rightType, Type resultType)
    {
        SyntaxKind = syntaxKind;
        BoundKind = kind;
        LeftType = leftType;
        RightType = rightType;
        ResultType = resultType;
    }
    internal static BoundBinaryOperator? Bind(SyntaxKind kind, Type leftType, Type rightType)
        => _operators.FirstOrDefault(op => op.SyntaxKind == kind
                                   && op.LeftType == leftType
                                   && op.RightType == rightType);
}


