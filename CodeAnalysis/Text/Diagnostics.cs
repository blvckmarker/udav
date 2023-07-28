using System.Collections;

namespace CodeAnalysis.Text
{
    public class Diagnostics : DiagnosticsBase, IEnumerable<DiagnosticsBag>
    {
        public Diagnostics(IEnumerable<DiagnosticsBag> diagnostics) : base(diagnostics) { }
        public Diagnostics() : base() { }


        public override void MakeIssue(string message, IssueKind issueKind = IssueKind.Problem)
            => MakeIssue(message, null, 0, issueKind);
        public override void MakeIssue(string message, string problemString, int startPosition, IssueKind issueKind = IssueKind.Problem)
            => _diagnostics.Append(new DiagnosticsBag(message, problemString, startPosition, issueKind));
        public override void MakeIssue(DiagnosticsBag issue)
            => _diagnostics.Append(issue);

        public override IEnumerator<DiagnosticsBag> GetEnumerator() => _diagnostics.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    }
}
