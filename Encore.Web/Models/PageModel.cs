namespace Encore.Web.Models
{
    using System.Collections.Generic;
    
    public class PageModel
    {
        public PageModel()
        {
            Pages = new List<NavigationPage>();
        }

        public string Username { get; set; }

        public string Title { get; set; }

        public string ViewModel { get; set; }

        public string RecordId { get; set; }

        public bool HasPages
        {
            get { return Pages.Count > 0; } 
        }

        public List<NavigationPage> Pages { get; set; }

        public class NavigationPage
        {
            public string Url { get; set; }

            public string Title { get; set; }

            public string Icon { get; set; }

            public bool Current { get; set; }

            public string Active { get { return Current ? "active" : string.Empty; } }
        }
    }
}
