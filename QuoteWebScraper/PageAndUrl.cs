using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuoteWebScraper
{
    public class PageAndUrl
    {
        public List<int> pages;
        public string url;

        public PageAndUrl(string url)
        {
            pages = new List<int>(0);
            this.url = url;
        }
    }
}
