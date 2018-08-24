using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuoteWebScraper
{
    public class QuoteData
    {
        public string quote;
        public string author;

        public QuoteData()
        {
            quote = null;
            author = null;
        }

        public QuoteData(string Quote, string Author)
        {
            quote = Quote;
            author = Author;
        }
    }
}
