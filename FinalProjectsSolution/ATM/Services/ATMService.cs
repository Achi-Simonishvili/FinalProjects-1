using ATM.Interfaces;

public class ATMService
{
    private readonly IJsonStorageService _storage;

    public ATMService(IJsonStorageService storage)
    {
        _storage = storage;
    }

    public void Deposit(User user, decimal amount)
    {
        user.Balance += amount;
        Console.WriteLine($"Deposit successful. New balance: {user.Balance}GEL");
        SaveUser(user);
    }

    public void Withdraw(User user, decimal amount)
    {
        if (user.Balance < amount)
        {
            Console.WriteLine("Insufficient funds.");
            return;
        }

        user.Balance -= amount;
        Console.WriteLine($"Withdrawal successful. New balance: {user.Balance}GEL");
        SaveUser(user);
    }

    public void CheckBalance(User user)
    {
        Console.WriteLine($"Your balance: {user.Balance}GEL");
    }

    private void SaveUser(User user)
    {
        var users = _storage.LoadUsers();

        var existing = users.FirstOrDefault(u => u.PersonalNumber == user.PersonalNumber);

        if (existing != null)
        {
            existing.Balance = user.Balance; 
        }
        else
        {
            users.Add(user);
        }

        _storage.SaveUsers(users);
    }
}
