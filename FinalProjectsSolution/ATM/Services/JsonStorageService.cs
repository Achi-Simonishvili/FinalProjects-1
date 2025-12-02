using System.Text.Json;
using ATM.Interfaces;

public class JsonStorageService : IJsonStorageService
{
    private readonly string _usersFile = "users.json";
    private readonly string _logFile = "transactions.json";

    public List<User> LoadUsers()
    {
        if (!File.Exists(_usersFile)) 
            return new List<User>();
        string json = File.ReadAllText(_usersFile);
        return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
    }

    public void SaveUsers(List<User> users)
    {
        string json = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_usersFile, json);
    }

    public List<TransactionLog> LoadLogs()
    {
        if (!File.Exists(_logFile)) 
            return new List<TransactionLog>();
        string json = File.ReadAllText(_logFile);
        return JsonSerializer.Deserialize<List<TransactionLog>>(json) ?? new List<TransactionLog>();
    }

    public void SaveLog(TransactionLog log)
    {
        var logs = LoadLogs();
        logs.Add(log);
        File.WriteAllText(_logFile, JsonSerializer.Serialize(logs, new JsonSerializerOptions { WriteIndented = true }));
    }
}
