using Hangman.Services;

namespace Hangman.Tests
{
    public class HangmanGameServiceTests
    {
        [Fact]
        public void StartNewGame_ShouldPickAWordAndInitializeRevealed()
        {
            var game = new HangmanGameService();
            game.StartNewGame();

            Assert.False(string.IsNullOrEmpty(game.SecretWord));

            string revealed = game.GetRevealedWord();
            Assert.Equal(game.SecretWord.Length, revealed.Length);
            Assert.All(revealed, c => Assert.Equal('_', c));
        }

        [Fact]
        public void GuessLetter_CorrectLetter_ShouldRevealPositions()
        {
            var game = new HangmanGameService();

            typeof(HangmanGameService)
                .GetField("_secretWord", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(game, "apple");

            typeof(HangmanGameService)
                .GetField("_revealed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(game, new char[5] { '_', '_', '_', '_', '_' });

            int result = game.GuessLetter('a');

            Assert.Equal(1, result);
            Assert.Equal("a____", game.GetRevealedWord());
            Assert.True(game.HadAnyCorrectLetter);
        }

        [Fact]
        public void GuessLetter_IncorrectLetter_ShouldIncreaseWrongCount()
        {
            var game = new HangmanGameService();
            typeof(HangmanGameService)
                .GetField("_secretWord", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(game, "banana");

            int wrongCountBefore = game.WrongLetterCount;
            int result = game.GuessLetter('z');

            Assert.Equal(2, result);
            Assert.Equal(wrongCountBefore + 1, game.WrongLetterCount);
        }
        [Fact]
        public void GuessLetter_DuplicateLetter_ShouldReturnZero()
        {
            var game = new HangmanGameService();
            typeof(HangmanGameService)
                .GetField("_secretWord", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(game, "kiwi");

            typeof(HangmanGameService)
                .GetField("_revealed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(game, new char[4] { '_', '_', '_', '_' });

            game.GuessLetter('k');
            int result = game.GuessLetter('k'); 

            Assert.Equal(0, result);
        }


        [Fact]
        public void IsFullyRevealed_ShouldReturnTrue_WhenWordGuessed()
        {
            var game = new HangmanGameService();
            typeof(HangmanGameService)
                .GetField("_secretWord", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(game, "kiwi");

            typeof(HangmanGameService)
                .GetField("_revealed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(game, "kiwi".ToCharArray());

            Assert.True(game.IsFullyRevealed());
        }

        [Fact]
        public void CalculateScore_ShouldReturnCorrectValue()
        {
            var game = new HangmanGameService();
            typeof(HangmanGameService)
                .GetField("_secretWord", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(game, "peach");

            Assert.Equal(HangmanGameService.MaxLetterGuesses, game.CalculateScore());

            typeof(HangmanGameService)
                .GetField("_wrongLetterCount", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(game, 2);

            Assert.Equal(HangmanGameService.MaxLetterGuesses - 2, game.CalculateScore());
        }
    }
}
