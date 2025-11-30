using System.Text.Json;
using ATM.Interfaces;

namespace ATM.Tests
{
    public class ATMServiceTests
    {
        private class InMemoryStorage : IJsonStorageService
        {
            public List<User> Users { get; set; } = new List<User>();
            public List<User> LastSavedUsers { get; private set; }
            public int SaveCalls { get; private set; }

            public List<User> LoadUsers() => new List<User>(Users);

            public void SaveUsers(List<User> users)
            {
                SaveCalls++;
                LastSavedUsers = new List<User>(users);
                Users = new List<User>(users);
            }
        }

        [Fact]
        public void Deposit_IncreasesBalance_And_SavesUpdatedUser()
        {
            var storage = new InMemoryStorage();
            var user = new User
            {
                Id = 1,
                Name = "Test",
                Surname = "User",
                PersonalNumber = "PN001",
                Pin = "0000",
                Balance = 100m
            };
            storage.Users.Add(new User
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                PersonalNumber = user.PersonalNumber,
                Pin = user.Pin,
                Balance = user.Balance
            });

            var svc = new ATMService(storage);
            svc.Deposit(user, 50m);

            Assert.Equal(150m, user.Balance);
            Assert.Equal(1, storage.SaveCalls);

            var saved = storage.LastSavedUsers.Find(u => u.PersonalNumber == user.PersonalNumber);
            Assert.NotNull(saved);
            Assert.Equal(150m, saved.Balance);
        }

        [Fact]
        public void Withdraw_DecreasesBalance_WhenEnoughFunds_And_SavesUpdatedUser()
        {
            var storage = new InMemoryStorage();
            var user = new User
            {
                Id = 2,
                Name = "Withdraw",
                Surname = "Able",
                PersonalNumber = "PN002",
                Pin = "1111",
                Balance = 200m
            };
            storage.Users.Add(new User
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                PersonalNumber = user.PersonalNumber,
                Pin = user.Pin,
                Balance = user.Balance
            });

            var svc = new ATMService(storage);
            svc.Withdraw(user, 80m);

            Assert.Equal(120m, user.Balance);
            Assert.Equal(1, storage.SaveCalls);

            var saved = storage.LastSavedUsers.Find(u => u.PersonalNumber == user.PersonalNumber);
            Assert.NotNull(saved);
            Assert.Equal(120m, saved.Balance);
        }

        [Fact]
        public void Withdraw_DoesNotChangeBalance_WhenInsufficientFunds_And_DoesNotSave()
        {
            var storage = new InMemoryStorage();
            var user = new User
            {
                Id = 3,
                Name = "Low",
                Surname = "Funds",
                PersonalNumber = "PN003",
                Pin = "2222",
                Balance = 50m
            };
            storage.Users.Add(new User
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                PersonalNumber = user.PersonalNumber,
                Pin = user.Pin,
                Balance = user.Balance
            });

            var svc = new ATMService(storage);
            svc.Withdraw(user, 100m);

            // Balance unchanged and SaveUsers should not be called
            Assert.Equal(50m, user.Balance);
            Assert.Equal(0, storage.SaveCalls);
        }

        [Fact]
        public void CheckBalance_WritesBalanceToConsole()
        {
            var storage = new InMemoryStorage();
            var svc = new ATMService(storage);

            var user = new User
            {
                Id = 4,
                Name = "Console",
                Surname = "Reader",
                PersonalNumber = "PN004",
                Pin = "3333",
                Balance = 333m
            };

            var sw = new StringWriter();
            var originalOut = Console.Out;
            try
            {
                Console.SetOut(sw);
                svc.CheckBalance(user);
                Console.Out.Flush();

                var output = sw.ToString();
                Assert.Contains($"Your balance: {user.Balance}GEL", output);
            }
            finally
            {
                Console.SetOut(originalOut);
                sw.Dispose();
            }
        }
    }

    public class UserServiceTests
    {
        private static void WriteUsersJson(string path, List<User> users)
        {
            var json = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(Path.Combine(path, "users.json"), json);
        }

        [Fact]
        public void Register_NewPersonalNumber_ReturnsUser_And_PersistsToFile()
        {
            string originalDir = Directory.GetCurrentDirectory();
            string temp = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(temp);

            try
            {
                Directory.SetCurrentDirectory(temp);

                WriteUsersJson(temp, new List<User>());

                var storage = new JsonStorageService();
                var svc = new UserService(storage);

                var newUser = svc.Register("Jane", "Doe", "PN100");

                Assert.NotNull(newUser);
                Assert.Equal("Jane", newUser.Name);
                Assert.Equal("Doe", newUser.Surname);
                Assert.Equal("PN100", newUser.PersonalNumber);
                Assert.Equal(0m, newUser.Balance);
                Assert.Matches(@"^\d{4}$", newUser.Pin);

                var persisted = storage.LoadUsers();
                Assert.Contains(persisted, u => u.PersonalNumber == "PN100" && u.Name == "Jane");
            }
            finally
            {
                Directory.SetCurrentDirectory(originalDir);
                try { Directory.Delete(temp, true); } catch {  }
            }
        }

        [Fact]
        public void Register_DuplicatePersonalNumber_ReturnsNull_And_WritesMessage()
        {
            string originalDir = Directory.GetCurrentDirectory();
            string temp = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(temp);

            try
            {
                Directory.SetCurrentDirectory(temp);

                var existing = new User
                {
                    Id = 1,
                    Name = "Exist",
                    Surname = "User",
                    PersonalNumber = "PN-DUP",
                    Pin = "1234",
                    Balance = 0
                };

                WriteUsersJson(temp, new List<User> { existing });

                var storage = new JsonStorageService();
                var svc = new UserService(storage);

                var sw = new StringWriter();
                var originalOut = Console.Out;
                try
                {
                    Console.SetOut(sw);
                    var result = svc.Register("New", "User", "PN-DUP");
                    Console.Out.Flush();

                    var output = sw.ToString();
                    Assert.Null(result);
                    Assert.Contains("Personal number already exists.", output);
                }
                finally
                {
                    Console.SetOut(originalOut);
                    sw.Dispose();
                }
            }
            finally
            {
                Directory.SetCurrentDirectory(originalDir);
                try { Directory.Delete(temp, true); } catch {  }
            }
        }

        [Fact]
        public void Login_WithValidCredentials_ReturnsUser_And_InvalidCredentials_ReturnsNull()
        {
            string originalDir = Directory.GetCurrentDirectory();
            string temp = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(temp);

            try
            {
                Directory.SetCurrentDirectory(temp);

                var existing = new User
                {
                    Id = 5,
                    Name = "Auth",
                    Surname = "Test",
                    PersonalNumber = "PN-AUTH",
                    Pin = "4242",
                    Balance = 10m
                };

                WriteUsersJson(temp, new List<User> { existing });

                var storage = new JsonStorageService();
                var svc = new UserService(storage);

                var good = svc.Login("PN-AUTH", "4242");
                Assert.NotNull(good);
                Assert.Equal("Auth", good.Name);

                var bad = svc.Login("PN-AUTH", "0000");
                Assert.Null(bad);
            }
            finally
            {
                Directory.SetCurrentDirectory(originalDir);
                try { Directory.Delete(temp, true); } catch {  }
            }
        }
    }
}