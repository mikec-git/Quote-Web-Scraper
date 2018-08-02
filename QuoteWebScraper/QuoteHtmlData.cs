using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace QuoteWebScraper
{
    class QuoteHtmlData
    {
        public List<HtmlNode> quotesHtml;
        public List<HtmlNode> authorsHtml;
        public int numOfQuotes;

        public QuoteHtmlData()
        {
            quotesHtml = new List<HtmlNode>();
            authorsHtml = new List<HtmlNode>();
            numOfQuotes = 0;
        }
    }
}
