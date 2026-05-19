
using BuildIntegrityAnalyzer.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using System.IO;

namespace BuildIntegrityAnalyzer.Services
{
    public class RoslynAnalyzerService
    {
        public List<IntegrityIssue> AnalyzeCode(string filePath)
        {
            List<IntegrityIssue> issues = new List<IntegrityIssue>();
            try
            {

                if (!File.Exists(filePath))
                {
                    return issues;
                }
                else
                {
                    string code = File.ReadAllText(filePath);

                    SyntaxTree tree = CSharpSyntaxTree.ParseText(code);

                    var diagnostics = tree.GetDiagnostics();

                    foreach (var diagnostic in diagnostics)
                    {
                        if (diagnostic.Severity == DiagnosticSeverity.Error)
                        {
                            issues.Add(new IntegrityIssue("Syntax Error", filePath, diagnostic.ToString(), ""));
                        }
                    }
                }
                


            }
            catch (Exception ex)
            { }
            return issues;
        }
    }
}