using CodeAnalysis.Binder.BoundExpressions;
using CodeAnalysis.Binder.BoundStatements;
using CodeAnalysis.Syntax.Parser;
using CodeAnalysis.Syntax.Parser.Expressions;
using CodeAnalysis.Syntax.Parser.Statements;
using CodeAnalysis.Text;

namespace CodeAnalysis.Binder;

public sealed partial class Binder
{
    private readonly DiagnosticsBase _diagnostics;
    private readonly IDictionary<string, object> _localVariables;
    private readonly StatementSyntax _syntaxRoot;
    public DiagnosticsBase Diagnostics => _diagnostics;

    public Binder(SyntaxTree syntaxTree, IDictionary<string, object> localVariables)
    {
        if (syntaxTree.Diagnostics.Where(x => x.Kind == IssueKind.Problem).Any())
            throw new Exception("Unable to bind syntax tree when tree has problem issues");

        _syntaxRoot = syntaxTree.Root;
        _diagnostics = syntaxTree.Diagnostics;
        _localVariables = localVariables;
    }

    public BoundStatement BindTree() => BindStatement(_syntaxRoot);

    private partial BoundExpression BindExpression(ExpressionSyntax syntax);
    private partial BoundExpression BindParenthesizedExpression(ParenthesizedExpressionSyntax syntax);
    private partial BoundExpression BindUnaryExpression(UnaryExpressionSyntax syntax);
    private partial BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax);
    private partial BoundExpression BindLiteralExpression(LiteralExpressionSyntax syntax);

    private partial BoundStatement BindStatement(StatementSyntax syntax);
    private partial BoundStatement BindAssignmentStatement(AssignmentStatementSyntax statement);
}
