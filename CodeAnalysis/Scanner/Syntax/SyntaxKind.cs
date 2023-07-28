namespace CodeAnalysis.Scanner.Syntax;

public enum SyntaxKind
{
    PlusToken,
    MinusToken,
    AsteriskToken,
    SlashToken,
    OpenParenToken,
    CloseParenToken,
    TrueKeyword,
    FalseKeyword,

    WhitespaceToken,
    BadToken,
    EofToken,

    NumberExpression,
    BinaryExpression,
    ParenthesizedExpression,
    UnaryExpression,
    LiteralExpression,
    NameExpression,
    ExclamationToken,
    AmpersandAmpersandToken,
    AmpersandToken,
    PipePipeToken,
    PipeToken,
}