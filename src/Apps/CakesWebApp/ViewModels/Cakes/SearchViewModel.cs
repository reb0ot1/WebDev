using System.Collections.Generic;

namespace CakesWebApp.ViewModels.Cakes
{
    public class SearchViewModel
    {
        public ByIdViewModel[] Cakes { get; set; }

        public string SearchText { get; set; }
    }
}
