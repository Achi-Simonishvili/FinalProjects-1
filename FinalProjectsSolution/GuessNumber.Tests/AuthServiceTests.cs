using GuessNumber.Models;
using GuessNumber.Services;

namespace GuessNumber.Tests
{
    public class AuthServiceTests : IDisposable
    {
        private readonly string _tempDir;
        private readonly string _originalDir;

        public AuthServiceTests()
        {
            _originalDir = Directory.GetCurrentDirectory();
            _tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(_tempDir);
            Directory.SetCurrentDirectory(_tempDir);
        }

        public void Dispose()
        {
            try
            {
                Directory.SetCurrentDirectory(_originalDir);
                Directory.Delete(_tempDir, true);
            }
            catch {  }
        }

        [Fact]
        public void Register_NewUser_CreatesEntry_And_ReturnsTrue()
        {
            var svc = new AuthService();

            bool ok = svc.Register("alice", "pwd", out string message);
            Assert.True(ok);
            Assert.Equal("Registration successful!", message);

            var lines = File.ReadAllLines("players.csv");
            Assert.True(lines.Length >= 2);
            var added = lines.Skip(1).Select(l => l.Split(',')[0]).Contains("alice");
            Assert.True(added);
        }

        [Fact]
        public void Register_DuplicateUser_ReturnsFalse()
        {
            var svc = new AuthService();
            bool first = svc.Register("bob", "p1", out _);
            Assert.True(first);

            bool second = svc.Register("bob", "p2", out string msg2);
            Assert.False(second);
            Assert.Equal("Username already exists!", msg2);
        }

        [Fact]
        public void Login_WithValidCredentials_ReturnsPlayer()
        {
            var svc = new AuthService();
            svc.Register("charlie", "secret", out _);

            bool ok = svc.Login("charlie", "secret", out Player player, out string msg);
            Assert.True(ok);
            Assert.Equal("Login successful!", msg);
            Assert.NotNull(player);
            Assert.Equal("charlie", player.Username);
            Assert.Equal("secret", player.Password);
        }

        [Fact]
        public void Login_WithInvalidCredentials_ReturnsFalse()
        {
            var svc = new AuthService();
            svc.Register("dave", "x", out _);

            bool ok = svc.Login("dave", "wrong", out Player player, out string msg);
            Assert.False(ok);
            Assert.Null(player);
            Assert.Equal("Invalid username or password!", msg);
        }

        [Fact]
        public void UpdatePlayer_UpdatesCsvRecord()
        {
            var svc = new AuthService();
            svc.Register("eva", "pw", out _);

            bool ok = svc.Login("eva", "pw", out Player p, out _);
            Assert.True(ok);

            p.BestScore = 42;
            p.GamesPlayed = 3;
            svc.UpdatePlayer(p);

            var line = File.ReadAllLines("players.csv").Skip(1)
                .FirstOrDefault(l => l.Split(',')[0] == "eva");
            Assert.NotNull(line);

            var parts = line.Split(',');
            Assert.Equal("eva", parts[0]);
            Assert.Equal("pw", parts[1]);
            Assert.Equal("42", parts[2]);
            Assert.Equal("3", parts[3]);
        }
    }
}