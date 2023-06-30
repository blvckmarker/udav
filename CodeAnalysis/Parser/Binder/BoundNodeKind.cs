namespace CodeAnalysis.Parser.Binder;

internal enum BoundNodeKind
{
    UnaryExpression,
    LiteralExpression,
    BinaryExpression,
    ParenthesizedExpression
}