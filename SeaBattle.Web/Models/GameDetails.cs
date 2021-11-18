using SeaBattle.Domain;

namespace SeaBattle.Web.Models
{
    public class GameDetails
    {
        public const string Bot = "bot";
        public Game Game { get; set; }
        public bool WithBot => SecondPlayer.UserName == Bot;
        public PlayerDetails FirstPlayer { get; set; }
        public PlayerDetails SecondPlayer { get; set; }
        public bool GameIsOver { get; set; }
    }
}