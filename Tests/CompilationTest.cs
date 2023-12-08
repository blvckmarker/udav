using System.Net.Http.Headers;
using CodeAnalysis;
using CodeAnalysis.Compilation;

namespace Tests;

public class CompilationTest
{
    [Fact]
    public static void SyntaxErrorTestTreeSimple()
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

    [Fact]
    public static void SemanticErrorTestUsingAssignmentExpressionWrapper()
    {
        var compiler = new Compiler(new Dictionary<VariableSymbol, object>());
        var compResult = compiler.Compile("let a = as", new());

        Assert.Equal(CompilationResultKind.SemanticError, compResult.Kind);
    }

    [Fact]
    public static void CorrectProgramTestUsingAssignmentExpressionWrapper()
    {
        var compiler = new Compiler(new Dictionary<VariableSymbol, object>());
        var _ = compiler.Compile("let a = true", new());
        var compResult = compiler.Compile("a = a || false && a", new());

        Assert.Equal(CompilationResultKind.Success, compResult.Kind);
        Assert.True((bool)compResult.ReturnResult);
    }


    [Fact]
    public static void CorrectProgramTestMultiAssignmentExpression()
    {
        var compiler = new Compiler(new Dictionary<VariableSymbol, object>());
        compiler.Compile("let a = 0", new());
        compiler.Compile("let b = 1", new());
        compiler.Compile("let c = 2", new());
        var compResult = compiler.Compile("a = b = c = 3", new());

        Assert.Equal(CompilationResultKind.Success, compResult.Kind);
        Assert.Equal(3, compResult.ReturnResult);

        foreach (var item in compiler.Variables)
            Assert.Equal(3, item.Value);
    }

    [Fact]
    public static void CorrectSimpleIfStatement()
    {
        var compiler = new Compiler(new Dictionary<VariableSymbol, object>());
        compiler.Compile("let a = 5", new());
        var compResult = compiler.Compile(
        """
        if (a > 5)
            a = 10
        else
            a = 0
        """, new());

        Assert.Equal(0, compiler.Variables.First().Value);
    }

    [Fact]
    public static void CorrectElseIfStatement()
    {
        var compiler = new Compiler(new Dictionary<VariableSymbol, object>());
        compiler.Compile("let a = 5", new());
        var compResult = compiler.Compile(
        """
        if (a > 5)
            a = 10
        else if (a < 0)
            a = -100
        else if (a == 5)
            a = 0
        else
            a = 666
        """, new());

        Assert.Equal(0, compiler.Variables.First().Value);
    }

    [Fact]
    public void CorrectBlockIfStatements()
    {
        var compiler = new Compiler(new Dictionary<VariableSymbol, object>());
        compiler.Compile("let a = 5", new());
        var compResult = compiler.Compile(
        """
        {
            if (a > 100)
            {
                a = 0
            }
            else if (a == 123123) {}
            else
                a = a * a
        }
        """, new());
        Assert.Equal(25, compiler.Variables.First().Value);
    }
}
