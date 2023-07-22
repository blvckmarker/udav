using SyntaxKind = CodeAnalysis.Lexer.Model.SyntaxKind;

namespace Tests.Lexer
{
    public class BasicLexicalAnalysisTest
    {
        [Fact]
        public static void CorrectTextTest()
        {
            var source = "111 +  -1 * (1 + 2) - 3 + 99999";

            var expected = Utils.GetDescendantTokens(source)
                                .MapTokensToBasic();
            var actual = Utils.GetTokens(source)
                              .MapTokensToBasic();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static void EmptyTextTest()
        {
            var expected = new List<BasicTokenModel>();
            var actual = Utils.MapTokensToBasic(Utils.GetTokens(" "));

            Assert.Equal(actual, expected);
        }

        [Fact]
        public static void WrongLiteralTest1()
        {
            var expected = new List<BasicTokenModel>
            {
                new(SyntaxKind.LiteralExpression, "111"),
                new(SyntaxKind.BadToken, "a"),
                new(SyntaxKind.MinusToken, "-"),
                new(SyntaxKind.PlusToken, "+"),
            };

            var actual = Utils.MapTokensToBasic(Utils.GetTokens("111a -+"));

            Assert.Equal(actual, expected);
        }

        [Fact]
        public static void WrongLiteralTest2()
        {
            var primaryExpected = new List<BasicTokenModel>
            {
                new(SyntaxKind.BadToken, "a"),
                new(SyntaxKind.BadToken, "b"),
                new(SyntaxKind.BadToken, "c"),
            };
            var actual = Utils.MapTokensToBasic(Utils.GetTokens("abc"));
            Assert.Equal(actual, primaryExpected);
        }
    }
}