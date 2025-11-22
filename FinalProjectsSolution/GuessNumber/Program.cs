using GuessNumber.UI;

namespace GuessNumber
{
    internal class Program
    {
        static void Main(string[] args)
        {
            GameMenu menu = new GameMenu();
            menu.Start();
        }
    }
}