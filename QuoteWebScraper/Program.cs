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
            string url = "https://www.brainyquote.com/topics/motivational";

            GetHtmlAsync(url);
            

            Console.ReadKey();
        }

        private static async Task GetHtmlAsync(string url)
        {
            QuoteData quoteData = new QuoteData();
            
            HtmlWeb web = new HtmlWeb();
            var htmlDocument = await web.LoadFromWebAsync(url);

            var quoteHtml = htmlDocument.DocumentNode.Descendants("a")
                .Where(node => node.GetAttributeValue("class","").Contains("b-qt")).ToList();

            var authorHtml = htmlDocument.DocumentNode.Descendants("a")
                .Where(node => node.GetAttributeValue("class","").Contains("bq-aut")).ToList();

            int quoteListLength = quoteHtml.Count;

            for (int i = 0; i < quoteListLength; i++)
            {
                quoteData.quotes.Add(quoteHtml[i].InnerText.Replace("&#39;", "'"));
                quoteData.authors.Add(authorHtml[i].InnerText);
            }

            for (int i = 0; i < quoteListLength; i++)
            {
                Console.WriteLine(quoteData.quotes[i]);
                Console.WriteLine(quoteData.authors[i]);
                Console.WriteLine();
            }



            Console.WriteLine();
        }
    }
}
