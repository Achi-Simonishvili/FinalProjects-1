using ATM.Interfaces;

namespace ATM
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var menu = new ATMMenu();
            menu.Start();
        }
    }
}
