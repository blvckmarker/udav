﻿using CodeAnalysis.Text;

namespace CodeAnalysis.Diagnostic
{
    public class DiagnosticsBag
    {
        public DiagnosticsBag(string message, IssueKind issueKind) : this(message, null, new TextSpan(0, 0), issueKind) { }
        public DiagnosticsBag(string message, string? problemText, TextSpan span, IssueKind issueKind)
        {
            Message = message;
            ProblemText = problemText;
            Span = span;
            Kind = issueKind;
        }

        public TextSpan Span { get; }
        public string Message { get; }
        public string? ProblemText { get; }
        public IssueKind Kind { get; }
    }
}