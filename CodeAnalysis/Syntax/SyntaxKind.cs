namespace CodeAnalysis.Syntax;

public enum SyntaxKind
{
    PlusToken,
    MinusToken,
    AsteriskToken,
    SlashToken,
    OpenParenToken,
    CloseParenToken,
    ExclamationToken,
    AmpersandAmpersandToken,
    AmpersandToken,
    PipePipeToken,
    PipeToken,
    CaretToken,
    EqualsToken,
    EqualsEqualsToken,
    ExclamationEqualsToken,
    TildeToken,
    LessThanEqualToken,
    GreaterThanToken,
    GreaterThanEqualToken,
    PercentToken,
    LessThanToken,
    IdentifierToken,
    NumericToken,

    TrueKeyword,
    FalseKeyword,
    IntKeyword,
    LetKeyword,
    BoolKeyword,

    WhitespaceToken,
    BadToken,
    EofToken,

    BinaryExpression,
    ParenthesizedExpression,
    UnaryExpression,
    LiteralExpression,
    NameExpression,
    AssignmentExpression,

    AssignmentStatement,
    AssignmentExpressionStatement,
}