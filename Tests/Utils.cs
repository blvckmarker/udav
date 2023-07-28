using CodeAnalysis;
using CodeAnalysis.Binder;
using DynamicExpresso;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Data;
using System.Numerics;
using SyntaxKind = CodeAnalysis.Scanner.Syntax.SyntaxKind;
using SyntaxToken = CodeAnalysis.Scanner.Syntax.SyntaxToken;
using SyntaxTree = CodeAnalysis.Parser.Syntax.SyntaxTree;

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
            .Skip(3) /* To create a valid syntax tree using roslyn API, we should use source program such as `var _ = ...`,
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
        internal static IEnumerable<Microsoft.CodeAnalysis.SyntaxToken> GetDescendantTokens(string source)
            => SyntaxFactory.ParseSyntaxTree($"var _ = {source}")
                            .GetRoot()
                            .DescendantTokens();

        /// <summary>
        /// Provides syntax tokens using Udav API
        /// </summary>
        internal static IEnumerable<SyntaxToken> GetTokens(string source)
        {
            var lexer = new Lexer(source);
            return lexer.LexAll().Where(x => x.Kind is not SyntaxKind.BadToken and not SyntaxKind.WhitespaceToken);
        }

        /// <summary>
        /// Provides evaluating using Roslyn API
        /// </summary>
        /// <typeparam name="TResult">Output type</typeparam>
        internal static TResult EvaluateExternal<TResult>(string source)
        {
            var interpreter = new Interpreter();
            var result = interpreter.Eval<TResult>(source);

            return result;
        }

        /// <summary>
        /// Provides evaluating using Udav API
        /// </summary>
        /// <typeparam name="TResult">Output type</typeparam>
        internal static object EvaluateInternal<TResult>(string source)
        {
            var parser = SyntaxTree.Parse(source);
            var binder = new Binder();
            var boundTree = binder.BindExpression(parser.Root);
            var eval = new Evaluator(boundTree);

            return (TResult)eval.Evaluate();
        }

        /// <summary>
        /// Generate random numerical sequence of <typeparamref name="TNumber"/> using default list of operators
        /// </summary>
        /// <typeparam name="TNumber">Type</typeparam>
        internal static string GenerateRandomNumericalSequence<TNumber>() where TNumber : INumber<TNumber>
            => GenerateRandomNumericalSequence<TNumber>(new string[] { "+", "-", "*", "/", "^", "|", "&" });

        /// <summary>
        /// Generate random numerical sequence of <typeparamref name="TNumber"/> using custom list of operators
        /// </summary>
        /// <typeparam name="TNumber">Type which implement <typeparamref name="INumber"/></typeparam>
        /// <param name="operators">Binary operators</param>
        internal static string GenerateRandomNumericalSequence<TNumber>(string[] operators) where TNumber : INumber<TNumber>
        {
            string GenerateNumber() => new Random().Next().ToString();
            string GenerateBinaryOperator() => operators[new Random().Next(0, operators.Length)];
            string GenerateUnaryOperator() => new[] { "", "-" }[new Random().Next(0, 2)];
            var nodesCount = new Random().Next(1, 100);

            var source = "";
            for (int i = 0; i < nodesCount; i++)
            {
                var MakeParenthesizedExpression = new Random().Next(0, 2);
                if (MakeParenthesizedExpression == 1)
                {
                    source += "(";
                    source += $"{GenerateUnaryOperator()} {GenerateNumber()} ";
                    source += $"{GenerateBinaryOperator()} ";
                    source += $"{GenerateUnaryOperator()} {GenerateNumber()}";
                    source += ") ";

                    source += $"{GenerateBinaryOperator()} ";
                }
                else
                {
                    source += $"{GenerateUnaryOperator()} {GenerateNumber()} ";
                    source += $"{GenerateBinaryOperator()} ";
                }
            }

            source += GenerateNumber();

            return source;
        }

        /// <summary>
        /// Generate random boolean sequence using default list of operators
        /// </summary>
        internal static string GenerateRandomBooleanSequence()
            => GenerateRandomBooleanSequence(new string[] { "&&", "||" }, new string[] { "!", "" });

        /// <summary>
        /// Generate random boolean sequence using custom list of operators
        /// </summary>
        internal static string GenerateRandomBooleanSequence(string[] binaryOperators, string[] unaryOperators)
        {
            string MapBool(int i) => (i == 1).ToString().ToLower();
            string GenerateBinaryOperator() => binaryOperators[new Random().Next(0, binaryOperators.Length)];
            string GenerateUnaryOperator() => unaryOperators[new Random().Next(0, unaryOperators.Length)];
            string GenerateBool() => MapBool(new Random().Next(0, 2));

            var source = string.Empty;

            var nodesCount = new Random().Next(1, 100);
            for (int i = 0; i < nodesCount; ++i)
            {
                var MakeParenthesizedExpression = new Random().Next(0, 2) == 1;
                if (MakeParenthesizedExpression)
                {
                    source += "(";
                    source += $"{GenerateUnaryOperator() + GenerateBool()} ";
                    source += $"{GenerateBinaryOperator()} ";
                    source += $"{GenerateUnaryOperator() + GenerateBool()} ";
                    source += ") ";

                    source += $"{GenerateBinaryOperator()} ";
                }
                else
                {
                    source += $"{GenerateUnaryOperator() + GenerateBool()} ";
                    source += $"{GenerateBinaryOperator()} ";
                }
            }

            source += GenerateBool();
            return source;
        }
    }
    internal record BasicTokenModel(SyntaxKind Kind, string Text);
}
