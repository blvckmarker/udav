using SyntaxKind = CodeAnalysis.Scanner.Syntax.SyntaxKind;

namespace Tests.Scanner
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
        public static void AllTokensTest()
        {
            var source = "+ - * / ( ) ! && & || | ^";

            var expected = Utils.GetTokens(source).MapTokensToBasic().ToList();

            var actual = typeof(SyntaxKind).GetEnumNames()
                                           .Where(x => x.EndsWith("Token"))
                                           .SkipLast(3) //eof bad whitespace
                                           .ToList();

            Assert.True(actual.Count() == expected.Count(), "Size of `expected` not equal to `actual`");

            for (int i = 0; i < actual.Count(); i++)
                Assert.Equal(expected[i].Kind.ToString(), actual[i]);
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
                new(SyntaxKind.NumberExpression, "111"),
                new(SyntaxKind.LiteralExpression, "a"),
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
                new(SyntaxKind.LiteralExpression, "abc"),
            };
            var actual = Utils.MapTokensToBasic(Utils.GetTokens("abc"));
            Assert.Equal(actual, primaryExpected);
        }

        [Fact]
        public static void LexicalException1()
        {
            var lexer = new Lexer("Hello world!");
            lexer.LexAll();

            Assert.Throws<Exception>(lexer.Lex);
        }

        [Fact]
        public static void LexicalException2()
        {
            var lexer = new Lexer("Hello world!");
            lexer.LexAll();

            Assert.Throws<Exception>(lexer.LexAll);
        }

        [Fact]
        public static void RandomNumericalTest()
        {
            var source = Utils.GenerateRandomNumericalSequence();

            var actual = Utils.GetTokens(source)
                              .MapTokensToBasic();
            var expected = Utils.GetDescendantTokens(source)
                                .MapTokensToBasic();

            Assert.Equal(actual, expected);
        }

        [Fact]
        public static void RandomBooleanTest()
        {
            var source = Utils.GenerateRandomBooleanSequence();

            var actual = Utils.GetTokens(source)
                              .MapTokensToBasic();
            var expected = Utils.GetDescendantTokens(source)
                                .MapTokensToBasic();

            Assert.Equal(actual, expected);
        }

    }
}