using ATM.Interfaces;

public class UserService
{
    private readonly JsonStorageService _storage;
    private List<User> _users;

    public UserService(JsonStorageService storage)
    {
        _storage = storage;
        _users = _storage.LoadUsers();
    }

    public User Register(string name, string surname, string personalNumber)
    {
        if (_users.Any(u => u.PersonalNumber == personalNumber))
        {
            Console.WriteLine("Personal number already exists.");
            return null;
        }

        string pin;
        do
        {
            pin = new Random().Next(1000, 10000).ToString();
        } while (_users.Any(u => u.Pin == pin));

        var user = new User
        {
            Id = _users.Count > 0 ? _users.Max(u => u.Id) + 1 : 1,
            Name = name,
            Surname = surname,
            PersonalNumber = personalNumber,
            Pin = pin,
            Balance = 0
        };

        _users.Add(user);
        _storage.SaveUsers(_users);
        return user;
    }

    public User Login(string personalNumber, string pin)
    {
        return _users.FirstOrDefault(u => u.PersonalNumber == personalNumber && u.Pin == pin);
    }
}
