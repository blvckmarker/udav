namespace CodeAnalysis.Text
{
    public class Diagnostics : DiagnosticsBase, IEnumerable<DiagnosticsBag>
    {
        public Diagnostics(IList<DiagnosticsBag> diagnostics) : base(diagnostics) { }
        public Diagnostics() : base() { }


        public override void MakeIssue(string message, IssueKind issueKind = IssueKind.Problem)
            => MakeIssue(message, null, 0, issueKind);
        public override void MakeIssue(string message, string? problemText, int startPosition, IssueKind issueKind = IssueKind.Problem)
            => _diagnostics.Add(new DiagnosticsBag(message, problemText, startPosition, issueKind));
        public override void MakeIssue(DiagnosticsBag issue)
            => _diagnostics.Add(issue);

    }
}
