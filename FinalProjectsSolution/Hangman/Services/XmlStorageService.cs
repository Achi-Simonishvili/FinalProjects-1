using Hangman.Models;
using System.Xml.Linq;

public class XmlStorageService
{
    private readonly string _filePath = "players.xml";

    public List<Player> LoadPlayers()
    {
        List<Player> players = new List<Player>();

        if (!File.Exists(_filePath))
            return players;

        XDocument doc = XDocument.Load(_filePath);

        foreach (var p in doc.Root.Elements("Player"))
        {
            players.Add(new Player(
                p.Element("Username").Value,
                p.Element("Password").Value
            )
            {
                BestScore = int.Parse(p.Element("BestScore").Value)
            });
        }

        return players;
    }

    public void SavePlayers(List<Player> players)
    {
        XDocument doc = new XDocument(
            new XElement("Players",
                players.Select(p =>
                    new XElement("Player",
                        new XElement("Username", p.Username),
                        new XElement("Password", p.Password),
                        new XElement("BestScore", p.BestScore)
                    )
                )
            )
        );

        doc.Save(_filePath);
    }
}
