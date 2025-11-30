namespace Hangman.Services
{
    public class HangmanGameService
    {
        private readonly List<string> _words = new List<string>
        {
            "apple", "banana", "orange", "grape", "kiwi",
            "strawberry", "pineapple", "blueberry", "peach", "watermelon"
        };

        private string _secretWord = string.Empty;
        private char[] _revealed = Array.Empty<char>();
        private HashSet<char> _guessedLetters = new HashSet<char>();
        private int _wrongLetterCount = 0;
        public const int MaxLetterGuesses = 6;

        public bool HadAnyCorrectLetter { get; private set; }

        public void StartNewGame()
        {
            var rnd = new Random();
            _secretWord = _words[rnd.Next(_words.Count)];
            _revealed = Enumerable.Repeat('_', _secretWord.Length).ToArray();
            _guessedLetters.Clear();
            _wrongLetterCount = 0;
            HadAnyCorrectLetter = false;
        }

        public int GuessLetter(char c)
        {
            c = char.ToLower(c);

            if (!char.IsLetter(c))
                throw new ArgumentException("Only letters are allowed.");

            if (_guessedLetters.Contains(c))
                return 0;

            _guessedLetters.Add(c);

            bool found = false;
            for (int i = 0; i < _secretWord.Length; i++)
            {
                if (_secretWord[i] == c)
                {
                    _revealed[i] = c;
                    found = true;
                }
            }

            if (found)
            {
                HadAnyCorrectLetter = true;
                return 1;
            }
            else
            {
                _wrongLetterCount++;
                return 2;
            }
        }

        public bool IsFullyRevealed() => !_revealed.Contains('_');

        public string GetRevealedWord() => new string(_revealed);

        public int WrongLetterCount => _wrongLetterCount;

        public string SecretWord => _secretWord;

        public IEnumerable<char> GuessedLetters => _guessedLetters;

        public int CalculateScore() => Math.Max(0, MaxLetterGuesses - _wrongLetterCount);
    }
}
