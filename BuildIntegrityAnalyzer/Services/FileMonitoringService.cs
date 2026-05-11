using System;
using System.IO;

namespace BuildIntegrityAnalyzer.Services
{
    public class FileMonitoringService
    {
        private readonly FileSystemWatcher _watcher;

        public FileMonitoringService(string path)
        {
            _watcher = new FileSystemWatcher(path);

            _watcher.IncludeSubdirectories = true;
            _watcher.EnableRaisingEvents = true;

            _watcher.Created += OnCreated;
            _watcher.Deleted += OnDeleted;
            _watcher.Changed += OnChanged;
            _watcher.Renamed += OnRenamed;
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"[CREATED] {e.FullPath}");
        }

        private void OnDeleted(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"[DELETED] {e.FullPath}");
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"[MODIFIED] {e.FullPath}");
        }

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            Console.WriteLine($"[RENAMED]");
            Console.WriteLine($"Old: {e.OldFullPath}");
            Console.WriteLine($"New: {e.FullPath}");
        }
    }
}