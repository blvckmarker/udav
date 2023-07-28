using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using SyntaxKind = CodeAnalysis.Scanner.Syntax.SyntaxKind;
using SyntaxToken = CodeAnalysis.Scanner.Syntax.SyntaxToken;

namespace Tests.Scanner
{
    internal static class Utils
    {
        internal static IEnumerable<BasicTokenModel> MapTokensToBasic(this IEnumerable<SyntaxToken> tokens)
        => tokens
            .SkipLast(1) // eof token is static 
            .Select(token => new BasicTokenModel(token.Kind, token.Text));


        internal static IEnumerable<BasicTokenModel> MapTokensToBasic(this IEnumerable<Microsoft.CodeAnalysis.SyntaxToken> tokens)
            => tokens
            .Skip(3) /* To create a valide syntax tree using roslyn API, we should use source program such as `var _ = ...`,
                      and skip three useless tokens: var, _, = */
            .SkipLast(2)
            .Select(token => new BasicTokenModel(MatchRoslynSyntaxKind(token.Kind()), token.ToString()));

        private static SyntaxKind MatchRoslynSyntaxKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind kind)
            => kind switch
            {
                Microsoft.CodeAnalysis.CSharp.SyntaxKind.PlusToken => SyntaxKind.PlusToken,
                Microsoft.CodeAnalysis.CSharp.SyntaxKind.MinusToken => SyntaxKind.MinusToken,
                Microsoft.CodeAnalysis.CSharp.SyntaxKind.AsteriskToken => SyntaxKind.AsteriskToken,
                Microsoft.CodeAnalysis.CSharp.SyntaxKind.SlashToken => SyntaxKind.SlashToken,
                Microsoft.CodeAnalysis.CSharp.SyntaxKind.EndOfFileToken => SyntaxKind.EofToken,
                Microsoft.CodeAnalysis.CSharp.SyntaxKind.OpenParenToken => SyntaxKind.OpenParenToken,
                Microsoft.CodeAnalysis.CSharp.SyntaxKind.CloseParenToken => SyntaxKind.CloseParenToken,
                Microsoft.CodeAnalysis.CSharp.SyntaxKind.NumericLiteralToken => SyntaxKind.NumberExpression,
                Microsoft.CodeAnalysis.CSharp.SyntaxKind.UnaryMinusExpression or Microsoft.CodeAnalysis.CSharp.SyntaxKind.UnaryPlusExpression => SyntaxKind.UnaryExpression,
                Microsoft.CodeAnalysis.CSharp.SyntaxKind.ParenthesizedExpression => SyntaxKind.ParenthesizedExpression,
                Microsoft.CodeAnalysis.CSharp.SyntaxKind.WhitespaceTrivia => SyntaxKind.WhitespaceToken,
                Microsoft.CodeAnalysis.CSharp.SyntaxKind.BadToken => SyntaxKind.BadToken,
                _ => throw new NotImplementedException(kind.ToString()) // ? BinaryExpression
            };
        /// <summary>
        /// Provides syntax tokens using Roslyn API
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        internal static IEnumerable<Microsoft.CodeAnalysis.SyntaxToken> GetDescendantTokens(string source)
            => SyntaxFactory.ParseSyntaxTree($"var _ = {source}")
                            .GetRoot()
                            .DescendantTokens();

        /// <summary>
        /// Provides syntax tokens using Udav API
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        internal static IEnumerable<SyntaxToken> GetTokens(string source)
        {
            var lexer = new Lexer(source);
            return lexer.LexAll().Where(x => x.Kind is not SyntaxKind.BadToken and not SyntaxKind.WhitespaceToken);
        }
    }
    internal record BasicTokenModel(SyntaxKind Kind, string Text);
}
