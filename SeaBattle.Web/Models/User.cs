using Microsoft.AspNetCore.Identity;

namespace SeaBattle.Web.Models
{
    public class User : IdentityUser
    {
        public int Year { get; set; }
    }
}