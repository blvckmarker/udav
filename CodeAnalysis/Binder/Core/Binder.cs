using CodeAnalysis.Binder.BoundExpressions;
using CodeAnalysis.Binder.BoundStatements;
using CodeAnalysis.Syntax.Parser;
using CodeAnalysis.Syntax.Parser.Expressions;
using CodeAnalysis.Syntax.Parser.Statements;
using CodeAnalysis.Text;

namespace CodeAnalysis.Binder.Core;

public sealed partial class Binder
{
    private readonly string _sourceProgram;
    private readonly StatementSyntax _syntaxRoot;
    private readonly DiagnosticsBase _diagnostics;
    private readonly IDictionary<VariableSymbol, object> _sessionVariables;

    public DiagnosticsBase Diagnostics => _diagnostics;
    public IDictionary<VariableSymbol, object> SessionVariables => _sessionVariables;

    public Binder(SyntaxTree syntaxTree, IDictionary<VariableSymbol, object> sessionVariables)
    {
        if (syntaxTree.Diagnostics.Where(x => x.Kind == IssueKind.Problem).Any())
            throw new Exception("Unable to bind syntax tree when tree has problem issues");

        _syntaxRoot = syntaxTree.Root;
        _diagnostics = syntaxTree.Diagnostics;
        _sessionVariables = sessionVariables;
        _sourceProgram = _diagnostics.SourceText;
    }

    public BoundStatement BindTree() => BindStatement(_syntaxRoot);

    private partial BoundExpression BindExpression(ExpressionSyntax syntax);
    private partial BoundExpression BindParenthesizedExpression(ParenthesizedExpressionSyntax syntax);
    private partial BoundExpression BindAssignmentExpression(AssignmentExpressionSyntax expressionSyntax);
    private partial BoundExpression BindUnaryExpression(UnaryExpressionSyntax syntax);
    private partial BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax);
    private partial BoundExpression BindLiteralExpression(LiteralExpressionSyntax syntax);
    private partial BoundExpression BindNameExpression(NameExpressionSyntax syntax);

    private partial BoundStatement BindStatement(StatementSyntax syntax);
    private partial BoundStatement BindAssignmentExpressionStatement(AssignmentExpressionStatementSyntax syntax);
    private partial BoundStatement BindAssignmentStatement(AssignmentStatementSyntax statement);
}
