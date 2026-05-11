using BuildIntegrityAnalyzer.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

namespace BuildIntegrityAnalyzer.Services
{
    public class DatabaseService
    {
        private readonly string connectionString =
            "Data Source=IntegrityLogs.db";

        public DatabaseService()
        {
            CreateTable();
        }

        private void CreateTable()
        {
            using var connection =
                new SqliteConnection(connectionString);

            connection.Open();

            string query =
            @"
            CREATE TABLE IF NOT EXISTS IntegrityLogs
            (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                IssueType TEXT,
                FileName TEXT,
                Message TEXT,
                Timestamp TEXT
            );
            ";

            using var command =
                new SqliteCommand(query, connection);

            command.ExecuteNonQuery();
        }

        public void SaveIssues(List<IntegrityIssue> issues)
        {
            using var connection =
                new SqliteConnection(connectionString);

            connection.Open();

            foreach (var issue in issues)
            {
                string query =
                @"
                INSERT INTO IntegrityLogs
                (
                    IssueType,
                    FileName,
                    Message,
                    Timestamp
                )
                VALUES
                (
                    @IssueType,
                    @FileName,
                    @Message,
                    @Timestamp
                );
                ";

                using var command =
                    new SqliteCommand(query, connection);

                command.Parameters.AddWithValue(
                    "@IssueType",
                    issue.IssueType
                );

                command.Parameters.AddWithValue(
                    "@FileName",
                    issue.FileName
                );

                command.Parameters.AddWithValue(
                    "@Message",
                    issue.Message
                );

                command.Parameters.AddWithValue(
                    "@Timestamp",
                    DateTime.Now.ToString()
                );

                command.ExecuteNonQuery();
            }
        }

        public List<IntegrityIssue> GetIssues()
        {
            List<IntegrityIssue> issues =
                new List<IntegrityIssue>();

            using var connection =
                new SqliteConnection(connectionString);

            connection.Open();

            string query =
            @"
    SELECT IssueType, FileName, Message
    FROM IntegrityLogs
    ORDER BY Id DESC
    ";

            using var command =
                new SqliteCommand(query, connection);

            using var reader =
                command.ExecuteReader();

            while (reader.Read())
            {
                issues.Add(new IntegrityIssue(
                    reader.GetString(0),
                    reader.GetString(1),
                    reader.GetString(2)
                ));
            }

            return issues;
        }
    }
}