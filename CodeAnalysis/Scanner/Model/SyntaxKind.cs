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

    LiteralExpression,
    BinaryExpression,
    ParenthesizedExpression,
    UnaryExpression
}