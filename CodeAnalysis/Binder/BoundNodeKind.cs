namespace CodeAnalysis.Binder;

public enum BoundNodeKind
{
    UnaryExpression,
    LiteralExpression,
    BinaryExpression,
    ParenthesizedExpression,
    AssignmentStatement,
    NameExpression,
    DeclaredVariableExpression,
    AssignmentExpression,
    AssignmentExpressionStatement,
    BlockStatement,
    CompilationUnit,
    IfStatement,
    ElseStatement
}