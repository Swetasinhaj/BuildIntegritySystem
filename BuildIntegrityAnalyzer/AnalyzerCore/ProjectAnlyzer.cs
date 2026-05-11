using BuildIntegrityAnalyzer.Models;
using BuildIntegrityAnalyzer.Scanner;
using BuildIntegrityAnalyzer.Services;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace BuildIntegrityAnalyzer.AnalyzerCore
{
    public class ProjectAnalyzer
    {
        private readonly FileScanner _fileScanner;
        private readonly RoslynAnalyzerService _roslynService;

        public ProjectAnalyzer()
        {
            _fileScanner = new FileScanner();
            _roslynService = new RoslynAnalyzerService();
        }

        public List<IntegrityIssue> AnalyzeProject(string projectPath)
        {
            List<IntegrityIssue> issues = new List<IntegrityIssue>();

            if (!File.Exists(projectPath))
            {
                issues.Add(new IntegrityIssue(
                    "Project Error",
                    projectPath,
                    "Project file not found"
                ));

                return issues;
            }

            XDocument doc = XDocument.Load(projectPath);

            var compileItems = doc.Descendants()
                .Where(x => x.Name.LocalName == "Compile");

            foreach (var item in compileItems)
            {
                string include = item.Attribute("Include")?.Value;

                if (string.IsNullOrEmpty(include))
                    continue;

                string fullPath = Path.Combine(
                    Path.GetDirectoryName(projectPath),
                    include
                );

                if (!_fileScanner.FileExists(fullPath))
                {
                    issues.Add(new IntegrityIssue(
                        "Missing File",
                        include,
                        "Source file is missing"
                    ));
                }
                else
                {
                    var roslynIssues =
                        _roslynService.AnalyzeCode(fullPath);

                    issues.AddRange(roslynIssues);
                }
            }

            var references = doc.Descendants()
                .Where(x => x.Name.LocalName == "ProjectReference");

            foreach (var reference in references)
            {
                string include =
                    reference.Attribute("Include")?.Value;

                if (string.IsNullOrEmpty(include))
                    continue;

                string refPath = Path.Combine(
                    Path.GetDirectoryName(projectPath),
                    include
                );

                if (!_fileScanner.FileExists(refPath))
                {
                    issues.Add(new IntegrityIssue(
                        "Broken Reference",
                        include,
                        "Referenced project not found"
                    ));
                }
            }

            return issues;
        }
    }
}