namespace CodeAnalysis.Text
{
    public class DiagnosticsBag
    {
        public DiagnosticsBag(string message, IssueKind issueKind) : this(message, null, 0, issueKind) { }
        public DiagnosticsBag(string message, string? problemText, int startPosition, IssueKind issueKind)
        {
            Message = message;
            ProblemText = problemText;
            StartPosition = startPosition;
            Kind = issueKind;
        }

        public int StartPosition { get; }
        public string Message { get; }
        public string? ProblemText { get; }
        public IssueKind Kind { get; }
    }
}