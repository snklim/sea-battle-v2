using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SeaBattle.Web.ViewModels
{
    public class CreateGameViewModel
    {
        public IEnumerable<SelectListItem> Users { get; set; }
        public string User { get; set; }
    }
}