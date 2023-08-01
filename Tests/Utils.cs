using CodeAnalysis;
using CodeAnalysis.Compilation;
using CodeAnalysis.Syntax.Parser;
using DynamicExpresso;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Data;
using System.Numerics;
using System.Reflection;
using Binder = CodeAnalysis.Binder.Binder;
using Diagnostic = CodeAnalysis.Text.Diagnostics;
using SyntaxKind = CodeAnalysis.Syntax.SyntaxKind;
using SyntaxToken = CodeAnalysis.Syntax.SyntaxToken;
using SyntaxTree = CodeAnalysis.Syntax.Parser.SyntaxTree;

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
                Microsoft.CodeAnalysis.CSharp.SyntaxKind.EqualsToken => SyntaxKind.EqualsToken,
                Microsoft.CodeAnalysis.CSharp.SyntaxKind.EqualsEqualsToken => SyntaxKind.EqualsEqualsToken,
                Microsoft.CodeAnalysis.CSharp.SyntaxKind.ExclamationEqualsToken => SyntaxKind.ExclamationEqualToken,
                Microsoft.CodeAnalysis.CSharp.SyntaxKind.TildeToken => SyntaxKind.TildeToken,
                Microsoft.CodeAnalysis.CSharp.SyntaxKind.LessThanToken => SyntaxKind.LessThanToken,
                Microsoft.CodeAnalysis.CSharp.SyntaxKind.LessThanEqualsToken => SyntaxKind.LessThanEqualToken,
                Microsoft.CodeAnalysis.CSharp.SyntaxKind.GreaterThanEqualsToken => SyntaxKind.GreaterThanEqualToken,
                Microsoft.CodeAnalysis.CSharp.SyntaxKind.GreaterThanToken => SyntaxKind.GreaterThanToken,
                Microsoft.CodeAnalysis.CSharp.SyntaxKind.PercentToken => SyntaxKind.PercentToken,
                Microsoft.CodeAnalysis.CSharp.SyntaxKind.ExclamationToken => SyntaxKind.ExclamationToken,
                Microsoft.CodeAnalysis.CSharp.SyntaxKind.SlashToken => SyntaxKind.SlashToken,
                Microsoft.CodeAnalysis.CSharp.SyntaxKind.EndOfFileToken => SyntaxKind.EofToken,
                Microsoft.CodeAnalysis.CSharp.SyntaxKind.OpenParenToken => SyntaxKind.OpenParenToken,
                Microsoft.CodeAnalysis.CSharp.SyntaxKind.CloseParenToken => SyntaxKind.CloseParenToken,
                Microsoft.CodeAnalysis.CSharp.SyntaxKind.NumericLiteralToken => SyntaxKind.NumericExpression,
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
        internal static TResult EvaluateExpressionExternal<TResult>(string source)
        {
            var interpreter = new Interpreter();
            var result = interpreter.Eval<TResult>(source);

            return result;
        }

        /// <summary>
        /// Provides compilation using Udav API
        /// </summary>
        /// <typeparam name="TResult">Output type</typeparam>
        internal static TResult CompileProgram<TResult>(string source)
        {
            var compiler = new Compiler(new Dictionary<string, object>());
            var compilation = compiler.Compile(source, new());

            if (compilation.Kind is not CompilationResultKind.Success)
                throw new Exception(compilation.Kind.ToString());

            return (TResult)compilation.ReturnResult!;
        }

        /// <summary>
        /// Provides evaluating using Udav API
        /// </summary>
        /// <typeparam name="TResult">Output type</typeparam>
        internal static TResult EvaluateExpressionInternal<TResult>(string source, IDictionary<string, object> variables = null) // Прости господи
        {
            if (variables == null)
                variables = new Dictionary<string, object>();

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);

            var expressionSyntax = InvokeMember(MemberTypes.Method, "ParseExpression", parser, 0);
            var binder = new Binder(new SyntaxTree(new Diagnostic(source), null, null), variables);
            var boundExpression = InvokeMember(MemberTypes.Method, "BindExpression", binder, expressionSyntax);
            var evaluator = new Evaluator(null, variables);
            var result = InvokeMember(MemberTypes.Method, "EvaluateExpression", evaluator, boundExpression);

            return (TResult)result;
        }

        internal static object? InvokeMember(MemberTypes type, string memberName, object obj, params object[]? param)
            => type switch
            {
                MemberTypes.Method => obj.GetType().GetMethod(memberName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static).Invoke(obj, param),
                MemberTypes.Property => obj.GetType().GetProperty(memberName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static).GetValue(obj),
                MemberTypes.Field => obj.GetType().GetField(memberName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static).GetValue(obj),
                _ => throw new NotSupportedException()
            };


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
