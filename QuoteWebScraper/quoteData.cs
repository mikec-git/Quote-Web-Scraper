using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuoteWebScraper
{
    public class QuoteData
    {
        public List<string> quotes;
        public List<string> authors;

        public QuoteData()
        {
            quotes = new List<string>(0);
            authors = new List<string>(0);
        }
    }
}
