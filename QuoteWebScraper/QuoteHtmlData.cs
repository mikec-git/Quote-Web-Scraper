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
            quotesHtml = new List<HtmlNode>(0);
            authorsHtml = new List<HtmlNode>(0);
            numOfQuotes = 0;
        }
    }
}
