namespace Tests
{
    public class LexicalAnalysisTest
    {
        [Fact]
        public static void CorrectTextTest()
        {
            var expected = new List<TokenModel>
            {
                new(SyntaxKind.LiteralExpression, "111"),
                new(SyntaxKind.Plus, "+"),
                new(SyntaxKind.Minus, "-"),
                new(SyntaxKind.LiteralExpression, "1"),
            };

            var actual = MapTokens(GetTokens("111   + -1"));

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static void EmptyTextTest()
        {
            var expected = new List<TokenModel>();
            var actual = MapTokens(GetTokens(" "));

            Assert.Equal(actual, expected);
        }

        [Fact]
        public static void WrongLiteralTest1()
        {
            var expected = new List<TokenModel>
        {
            new(SyntaxKind.LiteralExpression, "111"),
            new(SyntaxKind.BadToken, "a"),
            new(SyntaxKind.Minus, "-"),
            new(SyntaxKind.Plus, "+"),
        };
            var actual = MapTokens(GetTokens("111a -+"));

            Assert.Equal(actual, expected);
        }

        [Fact]
        public static void WrongLiteralTest2()
        {
            var expected = new List<TokenModel>
        {
            new(SyntaxKind.BadToken, "a"),
            new(SyntaxKind.BadToken, "b"),
            new(SyntaxKind.BadToken, "c"),
        };
            var actual = MapTokens(GetTokens("abc"));

            Assert.Equal(expected, actual);
        }

        private static IEnumerable<TokenModel> MapTokens(IEnumerable<SyntaxToken> tokens)
            => tokens
                .SkipLast(1) // eof token is static 
                .Select(token => new TokenModel(token.Kind, token.Text));

        private static IEnumerable<SyntaxToken> GetTokens(string source)
        {
            var lexer = new Lexer(source);
            var tokens = new List<SyntaxToken>();

            SyntaxToken currentToken;
            do
            {
                currentToken = lexer.Lex();

                if (currentToken.Kind is SyntaxKind.Whitespace)
                    continue;

                tokens.Add(currentToken);
            } while (currentToken.Kind is not SyntaxKind.Eof);

            return tokens;
        }


        private record TokenModel(SyntaxKind Kind, string Text);
    }
}