using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using HtmlAgilityPack;

namespace QuoteWebScraper
{
    class Program
    {
        static void Main(string[] args)
        {
            //string url = "https://www.brainyquote.com/topics/motivational?lgm=l";
            string url = "https://www.goodreads.com/quotes/";

            Console.WriteLine("Generate by tag (separate by spaces): ");
            string keywords = Console.ReadLine();

            Console.WriteLine("Enter the number of pages to query: ");
            string numberOfPagesString = Console.ReadLine();
            int numOfPages = Convert.ToInt32(numberOfPagesString);

            

            GetHtmlAsync(url);
            

            Console.ReadKey();
        }

        private static async Task GetHtmlAsync(string url)
        {
            QuoteData quoteData = new QuoteData();
            HtmlWeb web = new HtmlWeb();

            var htmlDocument = await web.LoadFromWebAsync(url);

            // Stores the specified descendants for the quotes and authors
            var quoteHtml = htmlDocument.DocumentNode.Descendants("div")
                .Where(node => node.GetAttributeValue("class", "").Equals("quoteText")).ToList();

            var authorHtml = htmlDocument.DocumentNode.Descendants("a")
                .Where(node => node.GetAttributeValue("href", "").Contains("/author/") 
                && node.GetAttributeValue("class","").Contains("authorOrTitle")).ToList();

            int listLength = quoteHtml.Count;

            // The quotes on this URI get converted into the following strings
            string[] quoteDelim = new string[] { "&ldquo;", "&rdquo;" };
            string extractedQuote;

            for (int i = 0; i < listLength; i++)
            {
                extractedQuote = quoteHtml[i].InnerHtml
                    .Split(quoteDelim, StringSplitOptions.None)[1]
                    .Replace("<br>", "\n");

                quoteData.quotes.Add("\"" + extractedQuote + "\"");
                quoteData.authors.Add(authorHtml[i].InnerText);
            }
            
            Console.WriteLine();
        }
    }
}
