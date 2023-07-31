namespace CodeAnalysis.Binder.BoundExpressions;

public enum BoundBinaryOperatorKind
{
    Addition,
    Subtraction,
    Division,
    Multiplication,
    LogicalOr,
    LogicalAnd,
    BitwiseAnd,
    BitwiseXor,
    BitwiseOr,
    Equals,
    NotEqual,
    LessOrEqual,
    GreaterThan,
    GreaterOrEqual,
    LessThan,
    DivisionRemainder,
}