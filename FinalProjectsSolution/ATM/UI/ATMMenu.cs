public class ATMMenu
{
    private readonly UserService _userService;
    private readonly ATMService _atmService;

    public ATMMenu()
    {
        var storage = new JsonStorageService();
        _userService = new UserService(storage);
        _atmService = new ATMService(storage);
    }

    public void Start()
    {
        Console.WriteLine("Welcome to ATM\n");

        User currentUser = null;

        while (currentUser == null)
        {
            Console.WriteLine("Choose an option:");
            Console.WriteLine("1 - Login");
            Console.WriteLine("2 - Register");
            Console.Write("Your choice: ");
            string choice = Console.ReadLine()?.Trim();

            if (choice == "1")
            {
                currentUser = Login();
            }
            else if (choice == "2")
            {
                currentUser = Register();
            }
            else
            {
                Console.WriteLine("Invalid option. Please try again.\n");
            }
        }

        Console.WriteLine($"\nWelcome {currentUser.Name} {currentUser.Surname}!");

        while (true)
        {
            Console.WriteLine("\nChoose an action:");
            Console.WriteLine("1 - Deposit");
            Console.WriteLine("2 - Withdraw");
            Console.WriteLine("3 - Check Balance");
            Console.WriteLine("4 - Exit");
            Console.Write("Your choice: ");

            string action = Console.ReadLine().Trim() ?? "";

            switch (action)
            {
                case "1":
                    Console.Write("Enter amount to deposit: ");
                    if (decimal.TryParse(Console.ReadLine(), out decimal depositAmt) && depositAmt > 0)
                        _atmService.Deposit(currentUser, depositAmt);
                    else
                        Console.WriteLine("Invalid amount.");
                    break;

                case "2":
                    Console.Write("Enter amount to withdraw: ");
                    if (decimal.TryParse(Console.ReadLine(), out decimal withdrawAmt) && withdrawAmt > 0)
                        _atmService.Withdraw(currentUser, withdrawAmt);
                    else
                        Console.WriteLine("Invalid amount.");
                    break;

                case "3":
                    _atmService.CheckBalance(currentUser);
                    break;

                case "4":
                    Console.WriteLine("Exiting... Goodbye!");
                    return;

                default:
                    Console.WriteLine("Unknown option. Please try again.");
                    break;
            }
        }

    }

    private User Login()
    {
        Console.Write("Enter personal number: ");
        string pn = Console.ReadLine()?.Trim() ?? "";
        Console.Write("Enter 4-digit PIN: ");
        string pin = Console.ReadLine()?.Trim() ?? "";

        var user = _userService.Login(pn, pin);
        if (user == null)
        {
            Console.WriteLine("Invalid personal number or PIN.\n");
        }
        return user;
    }

    private User Register()
    {
        Console.Write("Enter your name: ");
        string name = Console.ReadLine()?.Trim() ?? "";

        Console.Write("Enter your surname: ");
        string surname = Console.ReadLine()?.Trim() ?? "";

        Console.Write("Enter personal number: ");
        string personalNumber = Console.ReadLine()?.Trim() ?? "";

        var user = _userService.Register(name, surname, personalNumber);
        if (user != null)
        {
            Console.WriteLine($"\nRegistration successful! Your 4-digit PIN is: {user.Pin}\n");
        }
        return user;
    }
}
