namespace CodeAnalysis.Lexer.Model;

public enum SyntaxKind
{
    Plus,
    Minus,
    StarToken,
    SlashToken,
    LeftBracket,
    RightBracket,
    
    Whitespace,
    BadToken,
    Eof,
    
    LiteralExpression,
    BinaryExpression,
    ParenthesizedExpression,
    UnaryExpression
}