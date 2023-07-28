namespace CodeAnalysis.Text
{
    public class DiagnosticsBag
    {
        public DiagnosticsBag(string message, IssueKind issueKind) : this(message, null, 0, issueKind) { }
        public DiagnosticsBag(string message, string? problemString, int startPosition, IssueKind issueKind)
        {
            Message = message;
            ProblemMessage = problemString;
            Kind = issueKind;
        }

        public string Message { get; }
        public string? ProblemMessage { get; }
        public IssueKind Kind { get; }
    }
}
