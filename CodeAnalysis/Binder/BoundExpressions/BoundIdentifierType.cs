using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binder.BoundExpressions;

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
        switch (identifierType)
        {
            case SyntaxKind.IntKeyword:
                return new BoundIdentifierType(BoundTypeKind.DefinedType, typeof(int));
            case SyntaxKind.BoolKeyword:
                return new BoundIdentifierType(BoundTypeKind.DefinedType, typeof(bool));
            case SyntaxKind.LetKeyword:
                return new BoundIdentifierType(BoundTypeKind.UndefinedType, null);
            default:
                return new BoundIdentifierType(BoundTypeKind.DefinedType, null);
        }
    }
}
