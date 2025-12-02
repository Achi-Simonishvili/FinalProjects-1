using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GuessNumber.Models;
using GuessNumber.Services;

namespace GuessNumber.UI
{
    public class GameMenu
    {
        private readonly GameService _gameService;
        private readonly AuthService _authService;
        private readonly ScoreService _scoreService;
        private Player _player;

        public GameMenu()
        {
            _gameService = new GameService();
            _authService = new AuthService();
            _scoreService = new ScoreService();
        }

        public void Start()
        {
            Console.Clear();
            Console.WriteLine("==== Welcome to Guess Number ====\n");

            while (_player == null)
            {
                Console.WriteLine("1. Register");
                Console.WriteLine("2. Login");
                Console.WriteLine("3. Exit");
                Console.Write("\nChoose: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Register();
                        break;
                    case "2":
                        Login();
                        break;
                    case "3":
                        return;
                    default:
                        Console.WriteLine("Invalid choice!");
                        break;
                }
            }

            MainMenu();
        }

        private void Register()
        {
            Console.Write("Enter username: ");
            string username = Console.ReadLine();
            Console.Write("Enter password: ");
            string password = Console.ReadLine();

            if (_authService.Register(username, password, out string message))
            {
                Console.WriteLine(message);
            }
            else
            {
                Console.WriteLine(message);
            }
        }

        private void Login()
        {
            Console.Write("Enter username: ");
            string username = Console.ReadLine();
            Console.Write("Enter password: ");
            string password = Console.ReadLine();

            if (_authService.Login(username, password, out Player player, out string message))
            {
                Console.WriteLine(message);
                _player = player;
            }
            else
            {
                Console.WriteLine(message);
            }
        }

        private void MainMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"Hello, {_player.Username}! Your best score: {_player.BestScore}\n");
                Console.WriteLine("1. Play Game");
                Console.WriteLine("2. Show TOP 10 Players");
                Console.WriteLine("3. Logout");
                Console.WriteLine("4. Exit");
                Console.Write("\nChoose: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        PlayGame();
                        break;
                    case "2":
                        ShowTop10();
                        break;
                    case "3":
                        _player = null;
                        Start();
                        return;
                    case "4":
                        return;
                    default:
                        Console.WriteLine("Invalid choice!");
                        break;
                }

                Console.WriteLine("\nPress ENTER to continue...");
                Console.ReadLine();
            }
        }

        private void PlayGame()
        {
            Console.Clear();
            Console.WriteLine("Choose Difficulty:");
            Console.WriteLine("1. Easy (1–15)");
            Console.WriteLine("2. Medium (1–25)");
            Console.WriteLine("3. Hard (1–50)");
            Console.Write("\nChoose: ");
            string diff = Console.ReadLine();
            _gameService.SetDifficulty(diff);

            Console.WriteLine($"\nI have chosen a number between 1 and {_gameService.MaxRange}.");
            Console.WriteLine($"You have 10 attempts.\n");

            int attemptsUsed = 0;
            bool won = false;

            while (_gameService.RemainingAttempts > 0)
            {
                Console.Write($"Attempt {10 - _gameService.RemainingAttempts + 1}: Enter your guess: ");
                string input = Console.ReadLine();

                if (!int.TryParse(input, out int guess))
                {
                    Console.WriteLine("Invalid input! Must be a number.");
                    continue;
                }

                attemptsUsed++;
                if (_gameService.PlayAttempt(guess, out string hint, out int remaining))
                {
                    Console.WriteLine($"\nYOU WON! Guessed in {attemptsUsed} attempts!");
                    int baseScore = 11 - attemptsUsed;
                    int finalScore = (int)(baseScore * _gameService.DifficultyMultiplier);
                    _player.BestScore = Math.Max(_player.BestScore, finalScore);
                    _player.GamesPlayed++;
                    _authService.UpdatePlayer(_player);

                    _scoreService.SaveScore(_player, _player.BestScore);

                    won = true;
                    break;
                }
                else
                {
                    Console.WriteLine(hint);
                }
            }

            if (!won)
            {
                Console.WriteLine($"\nYou lost! The number was {_gameService.SecretNumber}");
                _player.GamesPlayed++;
                _authService.UpdatePlayer(_player);

                _scoreService.SaveScore(_player, _player.BestScore);
            }
        }

        private void ShowTop10()
        {
            Console.Clear();
            Console.WriteLine("==== TOP 10 PLAYERS ====\n");

            var top = _scoreService.GetTopPlayers(10);

            if (top == null || top.Count == 0)
            {
                Console.WriteLine("No scores yet.");
                return;
            }

            foreach (var p in top)
            {
                Console.WriteLine($"{p.Username} | Best Score: {p.BestScore} | Games: {p.GamesPlayed}");
            }
        }
    }
}
