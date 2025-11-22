namespace GuessNumber.Models
{
    public class Player
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public int BestScore { get; set; }
        public int GamesPlayed { get; set; }

        public Player(string username, string password)
        {
            Username = username;
            Password = password;
            BestScore = 0;
            GamesPlayed = 0;
        }
    }
}