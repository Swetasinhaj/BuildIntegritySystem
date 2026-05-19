
﻿using BuildIntegrityAnalyzer.AnalyzerCore;
using BuildIntegrityAnalyzer.Models;
using BuildIntegrityAnalyzer.Services;

Console.WriteLine("=== Intelligent Build Integrity Analyzer ===");
Console.WriteLine();

Console.Write("Enter project (.csproj) path: ");
//string projectPath =@"BuildIntegrityAnalyzer/BuildIntegrityAnalyzer.csproj";

//string? projectPath = Console.ReadLine();
string projectPath = @"..\TestProjects\DummyWebProject\DummyProject.csproj";

if (string.IsNullOrWhiteSpace(projectPath))
{
    Console.WriteLine("Invalid project path.");
    Environment.Exit(1);
}

var analyzer = new ProjectAnalyzer();

List<IntegrityIssue> issues =
    analyzer.AnalyzeProject(projectPath);

var databaseService =new DatabaseService();
databaseService.SaveIssues(issues);

//foreach (var issue in issues)
//{
  //  databaseService.SaveIssues(issue);
//}

Console.WriteLine();
Console.WriteLine($"Total Issues Found: {issues.Count}");

if (issues.Count > 0)
{
    Console.WriteLine("Integrity issues detected.");
    Console.WriteLine($"\nTotal Issues Found: {issues.Count}");

    foreach (var issue in issues)
    {
        Console.WriteLine("-----------------------------------");
        Console.WriteLine($"Severity   : {issue.Severity}");
        Console.WriteLine($"Issue Type : {issue.IssueType}");
        Console.WriteLine($"File Name  : {issue.FileName}");
        Console.WriteLine($"Description: {issue.Message}");
    }
   
}
else
{
    Console.WriteLine("Project integrity verified.");
    Environment.Exit(0);
}
