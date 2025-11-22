using Calculator.Services;

namespace Calculator.UI
{
    public class CalculatorMenu
    {
        private readonly CalculatorService _service;

        public CalculatorMenu()
        {
            _service = new CalculatorService();
        }

        public void Start()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Calculator ===");
                Console.WriteLine("1. Add (+)");
                Console.WriteLine("2. Subtract (-)");
                Console.WriteLine("3. Multiply (*)");
                Console.WriteLine("4. Divide (/)");
                Console.WriteLine("5. Exit");
                Console.Write("Choose option: ");

                string option = Console.ReadLine();

                if (option == "5")
                    break;

                if (!IsValidOption(option))
                {
                    Console.WriteLine("Invalid option! Press any key...");
                    Console.ReadKey();
                    continue;
                }

                double num1 = ReadNumber("Enter first number: ");
                double num2 = ReadNumber("Enter second number: ");

                try
                {
                    double result = DoOperation(option, num1, num2);

                    if (result == 0)
                        result = 0;

                    Console.WriteLine($"\nResult: {result}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nError: {ex.Message}");
                }

                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
        }

        private bool IsValidOption(string? option)
        {
            return option == "1" || option == "2" || option == "3" || option == "4";
        }

        private double ReadNumber(string message)
        {
            double number;

            while (true)
            {
                Console.Write(message);
                string? input = Console.ReadLine();

                if (double.TryParse(input, out number))
                    return number;

                Console.WriteLine("Invalid number! Try again.");
            }
        }

        private double DoOperation(string option, double a, double b)
        {
            return option switch
            {
                "1" => _service.Add(a, b),
                "2" => _service.Subtract(a, b),
                "3" => _service.Multiply(a, b),
                "4" => _service.Divide(a, b),
                _ => throw new InvalidOperationException("Unknown operation")
            };
        }
    }
}
