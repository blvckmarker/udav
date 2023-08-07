namespace CodeAnalysis.Text;

public class Diagnostics : DiagnosticsBase, IEnumerable<DiagnosticsBag>
{
    public Diagnostics(string text) : base(text) { }

    public override void MakeIssue(DiagnosticsBag issue)
        => _diagnostics.Add(issue);
    public override void MakeIssue(string message, IssueKind issueKind = IssueKind.Problem)
        => MakeIssue(message, null, new TextSpan(0, 0), issueKind);
    public override void MakeIssue(string message, string? problemText, TextSpan textSpan, IssueKind issueKind = IssueKind.Problem)
        => _diagnostics.Add(new DiagnosticsBag(message, problemText, textSpan, issueKind));
}
