using System;
using System.Collections.Generic;
using System.Text;

namespace BuildIntegrityAnalyzer.Models
{
    public class IntegrityIssue
    {
        public string IssueType { get; set; }
        public string FileName { get; set; }
        public string Message { get; set; }

        public IntegrityIssue(string issueType, string fileName, string message)
        {
            IssueType = issueType;
            FileName = fileName;
            Message = message;
        }
    }
}
