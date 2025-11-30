namespace Hangman.Models;

public class Player
{
    public string Username { get; set; }
    public string Password { get; set; }
    public int BestScore { get; set; }

    public Player() { }

    public Player(string username, string password)
    {
        Username = username;
        Password = password;
        BestScore = 0;
    }
}
