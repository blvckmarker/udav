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
    EqualToken,
    EqualEqualToken,
    ExclamationEqualToken,
    TildeToken,
    LessEqualToken,
    GreaterToken,
    GreaterEqualToken,
    LessToken,

    TrueKeyword,
    FalseKeyword,
    LetKeyword,

    WhitespaceToken,
    BadToken,
    EofToken,

    NumericExpression,
    BinaryExpression,
    ParenthesizedExpression,
    UnaryExpression,
    LiteralExpression,
    NameExpression,

    AssignmentStatement,
    PercentToken,
}