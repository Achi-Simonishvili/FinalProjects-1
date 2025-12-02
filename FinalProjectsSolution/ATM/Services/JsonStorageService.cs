using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using ATM.Interfaces;

public class JsonStorageService : IJsonStorageService
{
    private readonly string _usersFileName = "users.json";
    private readonly string _logFileName = "transactions.json";
    private readonly string _basePath;

    public JsonStorageService(string? basePath = null)
    {
        _basePath = string.IsNullOrWhiteSpace(basePath) ? AppContext.BaseDirectory : basePath;
    }

    private string UsersFile => Path.Combine(_basePath, _usersFileName);
    private string LogFile => Path.Combine(_basePath, _logFileName);

    public List<User> LoadUsers()
    {
        if (!File.Exists(UsersFile)) 
            return new List<User>();
        string json = File.ReadAllText(UsersFile);
        return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
    }

    public void SaveUsers(List<User> users)
    {
        Directory.CreateDirectory(_basePath);
        string json = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(UsersFile, json);
    }

    public List<TransactionLog> LoadLogs()
    {
        if (!File.Exists(LogFile)) 
            return new List<TransactionLog>();
        string json = File.ReadAllText(LogFile);
        return JsonSerializer.Deserialize<List<TransactionLog>>(json) ?? new List<TransactionLog>();
    }

    public void SaveLog(TransactionLog log)
    {
        var logs = LoadLogs();
        logs.Add(log);
        Directory.CreateDirectory(_basePath);
        File.WriteAllText(LogFile, JsonSerializer.Serialize(logs, new JsonSerializerOptions { WriteIndented = true }));
    }
}
