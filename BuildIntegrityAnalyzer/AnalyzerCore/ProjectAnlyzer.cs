using BuildIntegrityAnalyzer.Models;
using BuildIntegrityAnalyzer.Scanner;
using BuildIntegrityAnalyzer.Services;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
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
            try
            {


                if (!File.Exists(projectPath))
                {
                    issues.Add(new IntegrityIssue("Project Error", projectPath, "Project file not found", ""));

                    return issues;
                }
                else
                {

                    XDocument doc = XDocument.Load(projectPath);

                    var compileItems = doc.Descendants().Where(x => x.Name.LocalName == "Compile");

                    foreach (var item in compileItems)
                    {
                        string include = item.Attribute("Include")?.Value;

                        if (string.IsNullOrEmpty(include))
                            continue;

                        string fullPath = Path.Combine(Path.GetDirectoryName(projectPath), include);

                        if (!_fileScanner.FileExists(fullPath))
                        {
                            issues.Add(new IntegrityIssue("Missing File", include, "Source file is missing", "High"));
                        }
                        else
                        {
                            var roslynIssues =
                                _roslynService.AnalyzeCode(fullPath);

                            issues.AddRange(roslynIssues);
                        }
                    }

                    var references = doc.Descendants().Where(x => x.Name.LocalName == "ProjectReference");

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

                            issues.Add(new IntegrityIssue("Broken Reference", include, "Referenced project not found", ""));
                        }

                    }


                    string projectRoot = Path.GetDirectoryName(projectPath);

                    ValidateRequiredFiles(projectRoot, issues);
                    ValidatePackageReferences(projectPath, issues);

                    ValidateDllFiles(projectRoot, issues);

                    ValidateDuplicatePackages(projectPath, issues);
                }

            }
            catch(Exception ex)
            { 
            }
            return issues;
        }
        

        private void ValidateRequiredFiles(string projectRoot,List<IntegrityIssue> issues)
        {
            try
            {
                // --------------------------------
                // JSON FILES
                // --------------------------------

                string[] jsonFiles =
                {
        "appsettings.json"
    };

                // --------------------------------
                // CONFIG FILES
                // --------------------------------

                string[] configFiles =
                {
        "web.config"
    };

                // --------------------------------
                // DLL FILES
                // --------------------------------

                string[] dllFiles =
                {
        "System.Data.SQLite.dll"
    };

                // Validate JSON files
                ScanFiles(projectRoot, jsonFiles, "Missing JSON File", "Required JSON configuration file is missing", "Medium", issues);

                // Validate CONFIG files
                ScanFiles(projectRoot, configFiles, "Missing Config File", "Required configuration file is missing", "High", issues);

                // Validate DLL files
                ScanFiles(projectRoot, dllFiles, "Missing DLL", "Required DLL dependency is missing", "Critical", issues);
            }
            catch (Exception ex)
            { 
            }
        }


        // ==================================
        // COMMON SCANNING METHOD
        // ==================================

        private void ScanFiles(string projectRoot, string[] requiredFiles, string issueType, string message,string severity,List<IntegrityIssue> issues)
        {
            try
            {
                foreach (var requiredFile in requiredFiles)
                {
                    bool fileFound = Directory.GetFiles(projectRoot, requiredFile, SearchOption.AllDirectories).Any();

                    if (!fileFound)
                    {
                        issues.Add(new IntegrityIssue(issueType, requiredFile, message, severity));
                    }
                }
            }
            catch (Exception ex)
            { 
            }
        }
        private void ValidatePackageReferences(string projectFilePath, List<IntegrityIssue> issues)
        {
            // Load .csproj file

            XDocument projectFile = XDocument.Load(projectFilePath);

            // Get all PackageReference entries

            var packageReferences =projectFile.Descendants("PackageReference");

            foreach (var package in packageReferences)
            {
                string packageName =package.Attribute("Include")?.Value;

                string version = package.Attribute("Version")?.Value;

                // Check missing package name

                if (string.IsNullOrWhiteSpace(packageName))
                {
                    issues.Add(new IntegrityIssue( "Invalid Package Reference", "Unknown Package", "Package reference name is missing", "High" ));
                }

                // Check missing version

                if (string.IsNullOrWhiteSpace(version))
                {
                    issues.Add(new IntegrityIssue("Missing Package Version",packageName, "NuGet package version is missing","Medium"));
                }
            }
        }

        private void ValidateDllFiles( string projectRoot, List<IntegrityIssue> issues)
        {
            // Required DLLs

            string[] requiredDlls =
            {"System.Data.SQLite.dll"};

            foreach (var dll in requiredDlls)
            {
                bool dllFound =Directory.GetFiles(projectRoot, dll,SearchOption.AllDirectories) .Any();

                if (!dllFound)
                {
                    issues.Add(new IntegrityIssue( "Missing DLL",dll, "Required DLL dependency is missing","Critical"));
                }
            }
        }



        // ==========================================
        // VALIDATE DUPLICATE PACKAGE REFERENCES
        // ==========================================

        private void ValidateDuplicatePackages(string projectFilePath, List<IntegrityIssue> issues)
        {
            XDocument projectFile =XDocument.Load(projectFilePath);

            var packageNames = projectFile.Descendants("PackageReference").Select(p =>p.Attribute("Include")?.Value).ToList();

            var duplicates = packageNames.GroupBy(p => p).Where(g => g.Count() > 1);

            foreach (var duplicate in duplicates)
            {
                issues.Add(new IntegrityIssue("Duplicate Package Reference",duplicate.Key, "Package reference added multiple times","Medium" ));
            }
        }
    }
}