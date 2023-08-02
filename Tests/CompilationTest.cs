using CodeAnalysis;
using CodeAnalysis.Compilation;

namespace Tests;

public class CompilationTest
{
    [Fact]
    public static void SyntaxErrorTestLexer()
    {
        var compiler = new Compiler(new Dictionary<VariableSymbol, object>());
        var compResult = compiler.Compile("leee baran", new());

        Assert.Equal(CompilationResultKind.SyntaxError, compResult.Kind);
    }

    [Fact]
    public static void SyntaxErrorTestTreeBasic()
    {
        var compiler = new Compiler(new Dictionary<VariableSymbol, object>());
        var compResult = compiler.Compile("let i != hello", new());
        Assert.Equal(CompilationResultKind.SyntaxError, compResult.Kind);
    }

    [Fact]
    public static void SyntaxErrorTestTree()
    {
        var compiler = new Compiler(new Dictionary<VariableSymbol, object>());
        var compResult = compiler.Compile("let i = ~3 - (false || true) % 3 @ 2", new());

        Assert.Equal(CompilationResultKind.SyntaxError, compResult.Kind);
    }

    [Fact]
    public static void SemanticErrorTestBasic()
    {
        var compiler = new Compiler(new Dictionary<VariableSymbol, object>());
        var compResult = compiler.Compile("let k = false | 1", new());

        Assert.Equal(CompilationResultKind.SemanticError, compResult.Kind);
    }

    [Fact]
    public static void SemanticErrorTestParenthesized()
    {
        var compiler = new Compiler(new Dictionary<VariableSymbol, object>());
        var compResult = compiler.Compile("let k = (false || true) - 2", new());

        Assert.Equal(CompilationResultKind.SemanticError, compResult.Kind);
    }

    [Fact]
    public static void SemanticErrorTestVariableReference()
    {
        var compiler = new Compiler(new Dictionary<VariableSymbol, object>());
        var _ = compiler.Compile("let k = false", new());
        var compResult = compiler.Compile("let l = k | 0", new());

        Assert.Equal(CompilationResultKind.SemanticError, compResult.Kind);
    }

    [Fact]
    public static void SemanticErrorTestWrongVariableReference()
    {
        var compiler = new Compiler(new Dictionary<VariableSymbol, object>());
        var compResult = compiler.Compile("let k = j", new());

        Assert.Equal(CompilationResultKind.SemanticError, compResult.Kind);
    }

    [Fact]
    public static void RuntimeErrorTestDefinedVariableReference()
    {
        var compiler = new Compiler(new Dictionary<VariableSymbol, object>());

        var _ = compiler.Compile("let k = 1", new());
        var compResult = compiler.Compile("let k = 1", new());

        Assert.Equal(CompilationResultKind.SemanticError, compResult.Kind);
    }

    [Fact]
    public static void RuntimeErrorTestDivideByZero()
    {
        var compiler = new Compiler(new Dictionary<VariableSymbol, object>());
        var compResult = compiler.Compile("let k = 0/0", new());

        Assert.Equal(CompilationResultKind.RuntimeError, compResult.Kind);
    }

    [Fact]
    public static void CorrectProgramTestNumerical()
    {
        var compiler = new Compiler(new Dictionary<VariableSymbol, object>());
        var source = Utils.GenerateRandomNumericalSequence<int>();
        var compResult = compiler.Compile("let k = " + source, new());

        var expected = Utils.EvaluateExpressionExternal<int>(source);
        Assert.Equal(CompilationResultKind.Success, compResult.Kind);
        Assert.Equal(expected, compResult.ReturnResult);
    }

    [Fact]
    public static void CorrectProgramTestBoolean()
    {
        var compiler = new Compiler(new Dictionary<VariableSymbol, object>());
        var source = Utils.GenerateRandomBooleanSequence();
        var compResult = compiler.Compile("let k = " + source, new());

        var expected = Utils.EvaluateExpressionExternal<bool>(source);
        Assert.Equal(CompilationResultKind.Success, compResult.Kind);
        Assert.Equal(expected, (bool)compResult.ReturnResult);
    }

    [Fact]
    public static void CorrectProgramTestWithVariableReference()
    {
        var compiler = new Compiler(new Dictionary<VariableSymbol, object>());
        var _ = compiler.Compile("let k = false", new());
        var compResult = compiler.Compile("let j = !(k || false) && true", new());

        Assert.Equal(CompilationResultKind.Success, compResult.Kind);
        Assert.True((bool)compResult.ReturnResult);
    }

    [Fact]
    public static void CorrectProgramTestWithVariableReference2()
    {
        var compiler = new Compiler(new Dictionary<VariableSymbol, object>());
        var _ = compiler.Compile("let k = 1", new());
        var compResult = compiler.Compile("let j = ~(k * k) - 0 + k / 1 / 1", new());

        Assert.Equal(CompilationResultKind.Success, compResult.Kind);
        Assert.Equal(-1, (int)compResult.ReturnResult);
    }

}
