using CodeAnalysis.Binder.BoundExpressions;
using CodeAnalysis.Binder.BoundStatements;
using CodeAnalysis.Syntax.Parser.Expressions;
using CodeAnalysis.Syntax.Parser.Statements;
using CodeAnalysis.Text;

namespace CodeAnalysis.Binder;

public sealed partial class Binder
{
    private readonly DiagnosticsBase _diagnostics;
    private readonly Dictionary<string, object> _localVariables;

    public DiagnosticsBase Diagnostics => _diagnostics;

    public Binder(DiagnosticsBase diagnostics, Dictionary<string, object> localVariables)
    {
        _diagnostics = diagnostics;
        _localVariables = localVariables;
    }

    public partial BoundExpression BindExpression(ExpressionSyntax syntax);
    private partial BoundExpression BindParenthesizedExpression(ParenthesizedExpressionSyntax syntax);
    private partial BoundExpression BindUnaryExpression(UnaryExpressionSyntax syntax);
    private partial BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax);
    private partial BoundExpression BindLiteralExpression(LiteralExpressionSyntax syntax);

    public partial BoundStatement BindStatement(StatementSyntax syntax);
    private partial BoundStatement BindAssignmentStatement(AssignmentStatementSyntax statement);
}
