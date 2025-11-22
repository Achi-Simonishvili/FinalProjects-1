using System;

namespace GuessNumber.Services
{
    public class GameService
    {
        private int _secretNumber;
        private int _maxAttempts;
        private int _rangeMax;

        public double DifficultyMultiplier { get; private set; }


        public void SetDifficulty(string difficulty)
        {
            switch (difficulty)
            {
                case "1": _rangeMax = 15; DifficultyMultiplier = 1; break;
                case "2": _rangeMax = 25; DifficultyMultiplier = 1.5; break;
                case "3": _rangeMax = 50; DifficultyMultiplier = 2; break;
                default: _rangeMax = 15; DifficultyMultiplier = 1; break;
            }

            _maxAttempts = 10;
            Random rnd = new Random();
            _secretNumber = rnd.Next(1, _rangeMax + 1);
        }

        public bool PlayAttempt(int guess, out string hint, out int remainingAttempts)
        {
            remainingAttempts = --_maxAttempts;
            hint = string.Empty;

            if (guess == _secretNumber)
            {
                return true;
            }
            else if (guess < _secretNumber)
            {
                hint = "Too LOW!";
            }
            else
            {
                hint = "Too HIGH!";
            }

            return false;
        }

        public int RemainingAttempts => _maxAttempts;
        public int MaxRange => _rangeMax;
        public int SecretNumber => _secretNumber;
    }
}