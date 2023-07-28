using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using SyntaxKind = CodeAnalysis.Scanner.Syntax.SyntaxKind;
using SyntaxToken = CodeAnalysis.Scanner.Syntax.SyntaxToken;

namespace Tests
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
                Microsoft.CodeAnalysis.CSharp.SyntaxKind.AmpersandAmpersandToken => SyntaxKind.AmpersandAmpersandToken,
                Microsoft.CodeAnalysis.CSharp.SyntaxKind.AmpersandToken => SyntaxKind.AmpersandToken,
                Microsoft.CodeAnalysis.CSharp.SyntaxKind.BarBarToken => SyntaxKind.PipePipeToken,
                Microsoft.CodeAnalysis.CSharp.SyntaxKind.BarToken => SyntaxKind.PipeToken,
                Microsoft.CodeAnalysis.CSharp.SyntaxKind.TrueKeyword => SyntaxKind.TrueKeyword,
                Microsoft.CodeAnalysis.CSharp.SyntaxKind.FalseKeyword => SyntaxKind.FalseKeyword,
                Microsoft.CodeAnalysis.CSharp.SyntaxKind.CaretToken => SyntaxKind.CaretToken,
                Microsoft.CodeAnalysis.CSharp.SyntaxKind.ExclamationToken => SyntaxKind.ExclamationToken,
                Microsoft.CodeAnalysis.CSharp.SyntaxKind.SlashToken => SyntaxKind.SlashToken,
                Microsoft.CodeAnalysis.CSharp.SyntaxKind.EndOfFileToken => SyntaxKind.EofToken,
                Microsoft.CodeAnalysis.CSharp.SyntaxKind.OpenParenToken => SyntaxKind.OpenParenToken,
                Microsoft.CodeAnalysis.CSharp.SyntaxKind.CloseParenToken => SyntaxKind.CloseParenToken,
                Microsoft.CodeAnalysis.CSharp.SyntaxKind.NumericLiteralToken => SyntaxKind.NumberExpression,
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

        internal static string GenerateRandomNumericalSequence()
            => GenerateRandomNumericSequence(new string[] { "+", "-", "*", "/", "^", "|", "&" });
        internal static string GenerateRandomNumericSequence(string[] operations)
        {
            var nodesCount = new Random().Next(0, 100);

            var source = "";
            for (int i = 0; i < nodesCount; i++)
            {
                var MakeParenthesizedExpression = new Random().Next(0, 2);
                if (MakeParenthesizedExpression == 1)
                {
                    if (source.Length != 0 && source[^1] == ')')
                        source += operations[new Random().Next(0, operations.Length)];

                    source += '(';
                    source += new Random().Next();
                    source += operations[new Random().Next(0, operations.Length)];
                    source += new Random().Next();
                    source += ')';
                }
                else
                {
                    if (source.Length != 0 && source[^1] == ')')
                    {
                        source += operations[new Random().Next(0, operations.Length)];
                        source += new Random().Next();
                    }
                    else
                    {
                        if (source.Length != 0 && char.IsDigit(source[^1]))
                            source += operations[new Random().Next(0, operations.Length)];

                        source += new Random().Next();
                        source += operations[new Random().Next(0, operations.Length)];
                    }

                }
            }

            if (operations.Contains(source[^1].ToString()))
                source += new Random().Next();

            return source;
        }

        internal static string GenerateRandomBooleanSequence()
            => GenerateRandomBooleanSequence(new string[] { "&&", "||" }, new string[] { "!" });

        internal static string GenerateRandomBooleanSequence(string[] binaryOperations, string[] unaryOperations)
        {
            string MapBool(int i) => (i == 1).ToString().ToLower();

            var source = string.Empty;

            var nodesCount = new Random().Next(0, 100);
            for (int i = 0; i < nodesCount; ++i)
            {
                var MakeParenthesizedExpression = new Random().Next(0, 2) == 1;
                var UseUnaryOperator = new Random().Next(0, 2) == 1;
                if (MakeParenthesizedExpression)
                {
                    if (source.Length != 0 && source[^1] == ')')
                        source += binaryOperations[new Random().Next(0, binaryOperations.Length)];

                    source += '(';
                    source += UseUnaryOperator ? unaryOperations[new Random().Next(0, unaryOperations.Length)]
                        + MapBool(new Random().Next(0, 2)) : MapBool(new Random().Next(0, 2));

                    source += binaryOperations[new Random().Next(0, binaryOperations.Length)];

                    source += UseUnaryOperator ? unaryOperations[new Random().Next(0, unaryOperations.Length)]
                        + MapBool(new Random().Next(0, 2)) : MapBool(new Random().Next(0, 2));
                    source += ')';
                }
                else
                {
                    if (source.Length != 0 && source[^1] == ')')
                    {
                        source += binaryOperations[new Random().Next(0, binaryOperations.Length)];
                        source += MapBool(new Random().Next(0, 2));
                    }
                    else
                    {
                        if (source.Length != 0 && char.IsLetter(source[^1]))
                            source += binaryOperations[new Random().Next(0, binaryOperations.Length)];

                        source += MapBool(new Random().Next(0, 2));
                        source += binaryOperations[new Random().Next(0, binaryOperations.Length)];
                    }
                }
            }

            if (binaryOperations.Contains(source[^1].ToString()))
                source += MapBool(new Random().Next());

            return source;
        }
    }
    internal record BasicTokenModel(SyntaxKind Kind, string Text);
}
