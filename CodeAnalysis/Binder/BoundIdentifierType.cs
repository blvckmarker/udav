using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binder;

public class BoundIdentifierType
{
    public BoundIdentifierType(BoundTypeKind typeKind, Type? type)
    {
        Type = type;
        TypeKind = typeKind;
    }

    public BoundTypeKind TypeKind { get; }
    public Type? Type { get; }

    public static BoundIdentifierType Bind(SyntaxKind identifierType)
    {
        return identifierType switch
        {
            SyntaxKind.IntKeyword => new BoundIdentifierType(BoundTypeKind.DefinedType, typeof(int)),
            SyntaxKind.BoolKeyword => new BoundIdentifierType(BoundTypeKind.DefinedType, typeof(bool)),
            SyntaxKind.LetKeyword => new BoundIdentifierType(BoundTypeKind.UndefinedType, null),
            _ => new BoundIdentifierType(BoundTypeKind.DefinedType, null),
        };
    }
}
