using System.Collections;

namespace CodeAnalysis.Text
{
    public abstract class DiagnosticsBase : IEnumerable<DiagnosticsBag>
    {
        protected readonly IList<DiagnosticsBag> _diagnostics;

        public DiagnosticsBase(IList<DiagnosticsBag> _diagnostics)
        {
            this._diagnostics = _diagnostics;
        }
        public DiagnosticsBase() : this(new List<DiagnosticsBag>()) { }

        public abstract void MakeIssue(string message, IssueKind issueKind = IssueKind.Problem);
        public abstract void MakeIssue(string message, string problemText, int startPosition, IssueKind issueKind = IssueKind.Problem);
        public abstract void MakeIssue(DiagnosticsBag issue);

        public IEnumerator<DiagnosticsBag> GetEnumerator() => _diagnostics.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
