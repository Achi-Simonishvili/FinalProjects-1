using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GuessNumber.Models;

namespace GuessNumber.Services
{
    public class ScoreService
    {
        private readonly string _filePath = "scores.csv";

        public ScoreService()
        {
            EnsureCsvFile();
        }

        private void EnsureCsvFile()
        {
            if (!File.Exists(_filePath))
            {
                File.WriteAllText(_filePath, "Name,BestScore,GamesPlayed\n");
            }
        }

        public void SaveScore(Player player, int newScore)
        {
            try
            {
                var lines = File.ReadAllLines(_filePath).ToList();
                bool found = false;

                for (int i = 1; i < lines.Count; i++)
                {
                    var parts = lines[i].Split(',');

                    if (parts[0] == player.Username)
                    {
                        int bestScore = int.Parse(parts[1]);
                        int games = int.Parse(parts[2]) + 1;

                        if (newScore > bestScore)
                            bestScore = newScore;

                        lines[i] = $"{player.Username},{bestScore},{games}";
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    lines.Add($"{player.Username},{newScore},1");
                }

                File.WriteAllLines(_filePath, lines);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving score: {ex.Message}");
            }
        }

        public List<Player> GetTopPlayers(int top = 10)
        {
            try
            {
                var lines = File.ReadAllLines(_filePath).Skip(1);
                return lines
                    .Select(l =>
                    {
                        var parts = l.Split(',');
                        return new Player(parts[0], parts[1])
                        {
                            BestScore = int.Parse(parts[1]),
                            GamesPlayed = int.Parse(parts[2])
                        };
                    })
                    .OrderByDescending(p => p.BestScore)
                    .Take(top)
                    .ToList();
            }
            catch
            {
                return new List<Player>();
            }
        }
    }
}
