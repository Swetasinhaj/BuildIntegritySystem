using BuildIntegrityAnalyzer.AnalyzerCore;
using BuildIntegrityAnalyzer.Models;
using BuildIntegrityAnalyzer.Services;

Console.WriteLine("=== Intelligent Build Integrity Analyzer ===");
Console.WriteLine();

Console.Write("Enter project (.csproj) path: ");
string projectPath =@"BuildIntegrityAnalyzer/BuildIntegrityAnalyzer.csproj";

//string? projectPath = Console.ReadLine();
//string projectPath = @"E:\TestGitProject\TestGitProject\TestGitProject.csproj";

if (string.IsNullOrWhiteSpace(projectPath))
{
    Console.WriteLine("Invalid project path.");
    Environment.Exit(1);
}

var analyzer = new ProjectAnalyzer();

List<IntegrityIssue> issues =
    analyzer.AnalyzeProject(projectPath);

var databaseService =
    new DatabaseService();
databaseService.SaveIssues(issues);

//foreach (var issue in issues)
//{
  //  databaseService.SaveIssues(issue);
//}
int x=
Console.WriteLine();
Console.WriteLine($"Total Issues Found: {issues.Count}");

if (issues.Count > 0)
{
    Console.WriteLine("Integrity issues detected.");
    Environment.Exit(1);
}
else
{
    Console.WriteLine("Project integrity verified.");
    Environment.Exit(0);
}
