using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Parser.Html;

namespace QuoteWebScraper
{
    class Program
    {
        static void Main(string[] args)
        {
            var parser = new HtmlParser();

            var webSource = "https://www.goodreads.com/quotes/tag?utf8=%E2%9C%93&id=motivational";

            var document = parser.Parse(webSource);


        }
    }
}
