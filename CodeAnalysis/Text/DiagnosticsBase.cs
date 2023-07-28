using System.Collections;

namespace CodeAnalysis.Text
{
    public abstract class DiagnosticsBase : IEnumerable<DiagnosticsBag>
    {
        protected readonly IEnumerable<DiagnosticsBag> _diagnostics;

        public DiagnosticsBase(IEnumerable<DiagnosticsBag> _diagnostics)
        {
            this._diagnostics = _diagnostics;
        }
        public DiagnosticsBase() : this(new List<DiagnosticsBag>()) { }

        public abstract void MakeIssue(string message, IssueKind issueKind = IssueKind.Problem);
        public abstract void MakeIssue(string message, string problemString, int startPosition, IssueKind issueKind = IssueKind.Problem);
        public abstract void MakeIssue(DiagnosticsBag issue);

        public abstract IEnumerator<DiagnosticsBag> GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
