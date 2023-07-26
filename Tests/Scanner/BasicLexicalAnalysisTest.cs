using SyntaxKind = CodeAnalysis.Scanner.Model.SyntaxKind;

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
            var source = "+ - * / ( ) true false";

            var expected = Utils.GetTokens(source).MapTokensToBasic().ToList();

            var actual = new SyntaxKind[]
            {
                SyntaxKind.PlusToken,
                SyntaxKind.MinusToken,
                SyntaxKind.AsteriskToken,
                SyntaxKind.SlashToken,
                SyntaxKind.OpenParenToken,
                SyntaxKind.CloseParenToken,
                SyntaxKind.TrueKeyword,
                SyntaxKind.FalseKeyword,
            };

            Assert.True(actual.Length == expected.Count(), "Size of `expected` not equal to `actual`");

            for (int i = 0; i < actual.Length; i++)
                Assert.Equal(expected[i].Kind, actual[i]);
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
        public static void RandomTest()
        {
            var operations = new[] { '+', '-', '/', '*' };
            var nodesCount = new Random().Next(0, 100);

            var source = "";
            for (int i = 0; i < nodesCount; i++)
            {
                var MakeParenthesizedExpression = new Random().Next(0, 2);
                if (MakeParenthesizedExpression == 1)
                {
                    if (source.Length != 0 && source[^1] == ')')
                        source += operations[new Random().Next(0, 4)];

                    source += '(';
                    source += new Random().Next();
                    source += operations[new Random().Next(0, 4)];
                    source += new Random().Next();
                    source += ')';
                }
                else
                {
                    if (source.Length != 0 && source[^1] == ')')
                    {
                        source += operations[new Random().Next(0, 4)];
                        source += new Random().Next();
                    }
                    else
                    {
                        if (source.Length != 0 && char.IsDigit(source[^1]))
                            source += operations[new Random().Next(0, 4)];

                        source += new Random().Next();
                        source += operations[new Random().Next(0, 4)];
                    }

                }
            }

            if (operations.Contains(source[^1]))
                source += new Random().Next();

            var actual = Utils.GetTokens(source)
                              .MapTokensToBasic();
            var expected = Utils.GetDescendantTokens(source)
                                .MapTokensToBasic();

            Assert.Equal(actual, expected);
        }

    }
}