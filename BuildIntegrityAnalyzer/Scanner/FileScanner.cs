using System;
using System.Collections.Generic;
using System.Text;

namespace BuildIntegrityAnalyzer.Scanner
{
    public class FileScanner
    {
        public bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }
    }
}
