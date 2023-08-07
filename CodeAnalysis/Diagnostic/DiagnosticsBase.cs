using System.Collections;

namespace CodeAnalysis.Text
{
    public abstract class DiagnosticsBase : IEnumerable<DiagnosticsBag>
    {
        protected readonly IList<DiagnosticsBag> _diagnostics;
        public string SourceText { get; }

        public DiagnosticsBase(string text)
        {
            _diagnostics = new List<DiagnosticsBag>();
            SourceText = text;
        }

        public DiagnosticsBase Extend(IEnumerable<DiagnosticsBag> diagnostics)
        {
            foreach (var diagnostic in diagnostics)
                _diagnostics.Add(diagnostic);
            return this;
        }
        public abstract void MakeIssue(string message, IssueKind issueKind = IssueKind.Problem);
        public abstract void MakeIssue(string message, string problemText, int startPosition, IssueKind issueKind = IssueKind.Problem);
        public abstract void MakeIssue(DiagnosticsBag issue);

        public IEnumerator<DiagnosticsBag> GetEnumerator() => _diagnostics.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
