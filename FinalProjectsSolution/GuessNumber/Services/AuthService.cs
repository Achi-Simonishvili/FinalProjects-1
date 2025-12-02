using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GuessNumber.Models;

namespace GuessNumber.Services
{
    public class AuthService
    {
        private readonly string _filePath = "players.csv";

        public AuthService()
        {
            if (!File.Exists(_filePath))
                File.WriteAllText(_filePath, "Username,Password,BestScore,GamesPlayed\n");
        }

        public bool Register(string username, string password, out string message)
        {
            try
            {
                var lines = File.ReadAllLines(_filePath).ToList();
                if (lines.Skip(1).Any(l => l.Split(',')[0] == username))
                {
                    message = "Username already exists!";
                    return false;
                }

                lines.Add($"{username},{password},0,0");
                File.WriteAllLines(_filePath, lines);

                message = "Registration successful!";
                return true;
            }
            catch (Exception ex)
            {
                message = $"Error: {ex.Message}";
                return false;
            }
        }

        public bool Login(string username, string password, out Player player, out string message)
        {
            player = null;
            try
            {
                var lines = File.ReadAllLines(_filePath).Skip(1);

                foreach (var line in lines)
                {
                    var parts = line.Split(',');
                    if (parts[0] == username && parts[1] == password)
                    {
                        player = new Player(parts[0], parts[1])
                        {
                            BestScore = int.Parse(parts[2]),
                            GamesPlayed = int.Parse(parts[3])
                        };
                        message = "Login successful!";
                        return true;
                    }
                }

                message = "Invalid username or password!";
                return false;
            }
            catch (Exception ex)
            {
                message = $"Error: {ex.Message}";
                return false;
            }
        }

        public void UpdatePlayer(Player player)
        {
            try
            {
                var lines = File.ReadAllLines(_filePath).ToList();
                for (int i = 1; i < lines.Count; i++)
                {
                    var parts = lines[i].Split(',');
                    if (parts[0] == player.Username)
                    {
                        lines[i] = $"{player.Username},{player.Password},{player.BestScore},{player.GamesPlayed}";
                        break;
                    }
                }
                File.WriteAllLines(_filePath, lines);
            }
            catch {  }
        }
    }
}
