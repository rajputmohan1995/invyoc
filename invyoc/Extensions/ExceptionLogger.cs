using invyoc.Models;
using System.Text.Json;

namespace invyoc.Extensions;

public class ExceptionLogger
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true
    };

    private static readonly object FileLock = new();

    // Save exception into JSON file
    public static void LogException(Exception ex, string logPath)
    {
        var entry = new ExceptionLog
        {
            Message = ex.Message,
            StackTrace = ex.StackTrace,
            Source = ex.Source,
            ExceptionType = ex.GetType().FullName
        };

        lock (FileLock)
        {
            List<ExceptionLog> logs = [];

            if (File.Exists(logPath))
            {
                var json = File.ReadAllText(logPath);
                if (!string.IsNullOrWhiteSpace(json))
                {
                    logs = JsonSerializer.Deserialize<List<ExceptionLog>>(json) ?? [];
                }
            }

            logs.Add(entry);

            File.WriteAllText(logPath, JsonSerializer.Serialize(logs, Options));
        }
    }

    // Read all logged exceptions
    public static List<ExceptionLog> GetAllExceptions(string logPath)
    {
        if (!File.Exists(logPath))
            return [];

        var json = File.ReadAllText(logPath);

        if (string.IsNullOrWhiteSpace(json))
            return [];

        return JsonSerializer.Deserialize<List<ExceptionLog>>(json) ?? [];
    }
}