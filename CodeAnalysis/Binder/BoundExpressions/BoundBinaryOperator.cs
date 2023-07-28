using CodeAnalysis.Scanner.Syntax;

namespace CodeAnalysis.Binder.BoundExpressions;

internal class BoundBinaryOperator
{
    private static BoundBinaryOperator[] _operators = new[]
    {
        new BoundBinaryOperator(SyntaxKind.AsteriskToken, BoundBinaryOperatorKind.Multiplication,typeof(int), typeof(int)),
        new BoundBinaryOperator(SyntaxKind.SlashToken, BoundBinaryOperatorKind.Division, typeof(int), typeof(int)),
        new BoundBinaryOperator(SyntaxKind.MinusToken, BoundBinaryOperatorKind.Subtraction, typeof(int), typeof(int)),
        new BoundBinaryOperator(SyntaxKind.PlusToken, BoundBinaryOperatorKind.Addition, typeof(int), typeof(int)),

        new BoundBinaryOperator(SyntaxKind.PipePipeToken, BoundBinaryOperatorKind.LogicalOr, typeof(bool), typeof(bool)),
        new BoundBinaryOperator(SyntaxKind.AmpersandAmpersandToken, BoundBinaryOperatorKind.LogicalAnd, typeof(bool), typeof(bool)),
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


