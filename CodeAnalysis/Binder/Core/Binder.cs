using CodeAnalysis.Binder.BoundExpressions;
using CodeAnalysis.Binder.BoundStatements;
using CodeAnalysis.Binder.Scopes;
using CodeAnalysis.Diagnostic;
using CodeAnalysis.Syntax.Parser;
using CodeAnalysis.Syntax.Parser.Expressions;
using CodeAnalysis.Syntax.Parser.Statements;
using CodeAnalysis.Text;

namespace CodeAnalysis.Binder.Core;

public sealed partial class Binder
{
    private readonly DiagnosticsBase _diagnostics;
    private BoundScope _currScope;
    public DiagnosticsBase Diagnostics => _diagnostics;

    //public BoundGlobalScope BoundGlobalScope { get; }

    public Binder(BoundScope parent)
    {
        _diagnostics = new Diagnostics();
        _currScope = new BoundStatementScope(parent);
    }

    public BoundNode BindTree(SyntaxNode root) => BindCompilationUnit((CompilationUnit)root);
    private BoundCompilationUnit BindCompilationUnit(CompilationUnit syntax)
    {
        var boundStatement = BindStatement(syntax.Statement);
        return new BoundCompilationUnit(boundStatement);
    }

    private partial BoundExpression BindExpression(ExpressionSyntax syntax);
    private partial BoundParenthesizedExpression BindParenthesizedExpression(ParenthesizedExpressionSyntax syntax);
    private partial BoundAssignmentExpression BindAssignmentExpression(AssignmentExpressionSyntax expressionSyntax);
    private partial BoundUnaryExpression BindUnaryExpression(UnaryExpressionSyntax syntax);
    private partial BoundBinaryExpression BindBinaryExpression(BinaryExpressionSyntax syntax);
    private partial BoundLiteralExpression BindLiteralExpression(LiteralExpressionSyntax syntax);
    private partial BoundVariableExpression BindNameExpression(VariableExpressionSyntax syntax);

    private partial BoundStatement BindStatement(StatementSyntax syntax);
    private partial BoundWhileStatement BindWhileStatement(WhileStatementSyntax syntax);
    private partial BoundIfStatement BindIfStatement(IfStatementSyntax syntax);
    private partial BoundElseStatement BindElseStatement(ElseStatementSyntax syntax);
    private partial BoundBlockStatement BindBlockStatement(BlockStatementSyntax syntax);
    private partial BoundAssignmentExpressionStatement BindAssignmentExpressionStatement(AssignmentExpressionStatementSyntax syntax);
    private partial BoundAssignmentStatement BindAssignmentStatement(AssignmentStatementSyntax statement);

    public static BoundGlobalScope BindGlobalScope(BoundGlobalScope previous, SyntaxTree tree)
    {
        var binder = new Binder(previous);
        var boundTree = binder.BindTree(tree.Root);
        var diagnostics = binder.Diagnostics;
        var variables = binder._currScope.Variables;
        return new BoundGlobalScope(previous, boundTree, diagnostics, variables);
    }
}
