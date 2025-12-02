using Hangman.Models;
using Hangman.Services;

namespace Hangman.UI
{
    public class HangmanMenu
    {
        private readonly HangmanGameService _game;
        private readonly XmlStorageService _storage;
        private Player _currentPlayer;
        private List<Player> _players;

        public HangmanMenu()
        {
            _game = new HangmanGameService();
            _storage = new XmlStorageService();
            _players = _storage.LoadPlayers();
        }

        public void Start()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== HANGMAN ===");
                Console.WriteLine("1. Register");
                Console.WriteLine("2. Login");
                Console.WriteLine("3. Top 10 Players");
                Console.WriteLine("4. Exit");
                Console.Write("Choose option: ");

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
                        ShowTop10();
                        break;

                    case "4":
                        return;

                    default:
                        Console.WriteLine("Invalid choice. Press ENTER.");
                        Console.ReadLine();
                        break;
                }
            }
        }

        private void Register()
        {
            Console.Clear();
            Console.Write("Enter username: ");
            string username = Console.ReadLine()?.Trim();

            Console.Write("Enter password: ");
            string password = Console.ReadLine()?.Trim();

            if (_players.Any(p => p.Username == username))
            {
                Console.WriteLine("Username already exists. Press ENTER.");
                Console.ReadLine();
                return;
            }

            Player newPlayer = new Player(username, password);
            _players.Add(newPlayer);
            _storage.SavePlayers(_players);

            Console.WriteLine("Registration successful! Press ENTER.");
            Console.ReadLine();
        }

        private void Login()
        {
            Console.Clear();
            Console.Write("Username: ");
            string username = Console.ReadLine()?.Trim();

            Console.Write("Password: ");
            string password = Console.ReadLine()?.Trim();

            var player = _players.FirstOrDefault(p => p.Username == username && p.Password == password);

            if (player == null)
            {
                Console.WriteLine("Invalid credentials. Press ENTER.");
                Console.ReadLine();
                return;
            }

            _currentPlayer = player;

            PlayerMenu();
        }

        private void PlayerMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"Logged in as: {_currentPlayer.Username}");
                Console.WriteLine("1. Play Hangman");
                Console.WriteLine("2. Logout");
                Console.WriteLine("3. Top 10 Players");
                Console.Write("Choose option: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        PlayForCurrentPlayer(_currentPlayer);
                        break;

                    case "2":
                        _currentPlayer = null;
                        return;

                    case "3":
                        ShowTop10();
                        break;

                    default:
                        Console.WriteLine("Invalid choice. Press ENTER.");
                        Console.ReadLine();
                        break;
                }
            }
        }

        private void ShowTop10()
        {
            Console.Clear();
            Console.WriteLine("=== TOP 10 PLAYERS ===");

            var top = _players
                .OrderByDescending(p => p.BestScore)
                .Take(10)
                .ToList();

            if (top.Count == 0)
            {
                Console.WriteLine("No players yet.");
            }
            else
            {
                int rank = 1;
                foreach (var p in top)
                {
                    Console.WriteLine($"{rank}. {p.Username} - {p.BestScore}");
                    rank++;
                }
            }

            Console.WriteLine("\nPress ENTER.");
            Console.ReadLine();
        }

        public void PlayForCurrentPlayer(Player player)
        {
            if (player == null) throw new ArgumentNullException(nameof(player));
            _currentPlayer = player;

            Console.Clear();
            Console.WriteLine("Starting Hangman...\n");

            _game.StartNewGame();

            int letterGuesses = 0;

            while (letterGuesses < HangmanGameService.MaxLetterGuesses)
            {
                Console.Clear();
                Console.WriteLine($"Word: {_game.GetRevealedWord()}");
                Console.WriteLine($"Guessed letters: {string.Join(", ", _game.GuessedLetters)}");
                Console.WriteLine($"Wrong letters: {_game.WrongLetterCount}/{HangmanGameService.MaxLetterGuesses}");
                Console.Write($"\nLetter guess {letterGuesses + 1}/{HangmanGameService.MaxLetterGuesses}: Enter a letter: ");

                string input = Console.ReadLine()?.Trim().ToLower();

                if (string.IsNullOrEmpty(input))
                {
                    Console.WriteLine("Empty input. Press ENTER to try again.");
                    Console.ReadLine();
                    continue;
                }

                if (input.Length > 1)
                {
                    Console.WriteLine("Please enter only a single letter.");
                    Console.ReadLine();
                    continue;
                }

                char letter = input[0];
                try
                {
                    int res = _game.GuessLetter(letter);

                    if (res == 0)
                    {
                        Console.WriteLine("You already guessed that letter. This does not consume an attempt.");
                        Console.ReadLine();
                        continue; 
                    }
                    else if (res == 1)
                    {
                        Console.WriteLine("Good! Letter is present.");
                        letterGuesses++;
                    }
                    else 
                    {
                        Console.WriteLine("Letter not in the word.");
                        letterGuesses++; 
                    }
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Invalid input: {ex.Message}");
                    Console.ReadLine();
                    continue;
                }

                if (_game.IsFullyRevealed())
                {
                    int score = _game.CalculateScore();
                    Console.WriteLine($"\nAll letters revealed! You win. Score: {score}");
                    Console.WriteLine($"The word was: {_game.SecretWord}");
                    UpdatePlayerScoreIfBetter(score);
                    Console.WriteLine("Press ENTER to return to menu.");
                    Console.ReadLine();
                    return;
                }
            }

            Console.Clear();
            Console.WriteLine($"Word: {_game.GetRevealedWord()}");
            Console.WriteLine($"Wrong letters: {_game.WrongLetterCount}/{HangmanGameService.MaxLetterGuesses}");

            if (!_game.HadAnyCorrectLetter)
            {
                Console.WriteLine("\nAll six letters were incorrect. You lose.");
                Console.WriteLine($"The word was: {_game.SecretWord}");
                Console.WriteLine("Press ENTER to return to menu.");
                Console.ReadLine();
                return;
            }

            Console.Write("\nEnter your final guess for the full word: ");
            string finalGuess = Console.ReadLine().Trim().ToLower() ?? string.Empty;

            if (finalGuess == _game.SecretWord)
            {
                int score = _game.CalculateScore();
                Console.WriteLine($"\nCorrect! You win. Score: {score}");
                UpdatePlayerScoreIfBetter(score);
            }
            else
            {
                Console.WriteLine($"\nIncorrect. You lose. The word was: {_game.SecretWord}");
            }

            Console.WriteLine("\nPress ENTER to return to menu.");
            Console.ReadLine();
        }

        private void UpdatePlayerScoreIfBetter(int score)
        {
            if (_currentPlayer == null) return;

            if (score > _currentPlayer.BestScore)
            {
                _currentPlayer.BestScore = score;
            }

            var existing = _players.FirstOrDefault(p => p.Username == _currentPlayer.Username && p.Password == _currentPlayer.Password);
            if (existing != null)
            {
                existing.BestScore = _currentPlayer.BestScore;
            }
            else
            {
                _players.Add(_currentPlayer);
            }

            _storage.SavePlayers(_players);
        }
    }
}
