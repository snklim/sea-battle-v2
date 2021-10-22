using SeaBattle.Domain;

namespace SeaBattle.Web.Models
{
    public class GameDetails
    {
        public Changes[] ChangesList { get; set; }
        public string Message { get; set; }
    }
}