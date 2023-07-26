namespace CodeAnalysis.Scanner.Model;

public enum SyntaxKind
{
    PlusToken,
    MinusToken,
    AsteriskToken,
    SlashToken,
    OpenParenToken,
    CloseParenToken,

    WhitespaceToken,
    BadToken,
    EofToken,

    NumberExpression,
    BinaryExpression,
    ParenthesizedExpression,
    UnaryExpression,
    LiteralExpression,
    TrueKeyword,
    FalseKeyword,
    NameExpression,
}