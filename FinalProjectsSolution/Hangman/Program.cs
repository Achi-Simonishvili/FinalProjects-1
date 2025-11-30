using Hangman.UI;

namespace Hangman
{
    internal class Program
    {
        static void Main(string[] args)
        {
            HangmanMenu menu = new HangmanMenu();
            menu.Start();
        }
    }
}
