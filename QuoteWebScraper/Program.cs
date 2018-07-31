using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Net.Http;
using HtmlAgilityPack;

namespace QuoteWebScraper
{
    class Program
    {
        static void Main(string[] args)
        {
            // Scraping from https://goodreads.com for desired quotes
            string url = "https://www.goodreads.com/quotes";

            Console.WriteLine("Generate by keyword (separate with spaces or commas): ");
            string keywords = Console.ReadLine();

            Console.WriteLine("Enter the number of pages to query: ");
            string numberOfPagesString = null; ;
            do
            {
                numberOfPagesString = Console.ReadLine();

                if (string.IsNullOrEmpty(numberOfPagesString))
                    numberOfPagesString = "0";

            } while (string.IsNullOrEmpty(numberOfPagesString));

            int numOfPages = Convert.ToInt32(numberOfPagesString);
            if (numOfPages <= 0) numOfPages = 1;

            if (!string.IsNullOrEmpty(keywords))
            {
                char[] delim = new char[] { ' ', ',' };
                string[] keywordList = keywords.Split(delim, StringSplitOptions.RemoveEmptyEntries);
                int numOfKeywords = keywordList.Length;

                url = url + $"/tag?id={keywordList[0]}";

                if (numOfKeywords > 1)
                {
                    for (int i = 1; i < numOfKeywords; i++)
                        url = url + $"+{keywordList[i]}";
                }

                url = url + "&page=";
            }
            else { url = url + "?page="; }

            GetHtmlAsync(url, numOfPages);
            
            Console.ReadKey();
        }

        private static async Task GetHtmlAsync(string url, int pages)
        {
            QuoteData quoteData = new QuoteData();
            HtmlWeb web = new HtmlWeb();

            string urlPage;

            for (int i = 1; i <= pages; i++)
            {
                urlPage = url + $"{i}&utf8=✓";

                var htmlDocument = await web.LoadFromWebAsync(urlPage);

                // Stores the specified descendants for the quotes and authors
                var quoteHtml = htmlDocument.DocumentNode.Descendants("div")
                    .Where(node => node.GetAttributeValue("class", "").Equals("quoteText")).ToList();

                var authorHtml = htmlDocument.DocumentNode.Descendants("a")
                    .Where(node => node.GetAttributeValue("href", "").Contains("/author/")
                    && node.GetAttributeValue("class", "").Contains("authorOrTitle")).ToList();

                int listLength = quoteHtml.Count;

                // Double quotes for this URL show up as the following deliminators
                string[] quoteDelim = new string[] { "&ldquo;", "&rdquo;" };
                string extractedQuote;

                for (int j = 0; j < listLength; j++)
                {
                    extractedQuote = quoteHtml[j].InnerHtml
                        .Split(quoteDelim, StringSplitOptions.None)[1]
                        .Replace("<br>", "\n");

                    quoteData.quotes.Add("\"" + extractedQuote + "\"");
                    quoteData.authors.Add(authorHtml[j].InnerText);
                }
            }

            Console.WriteLine();
        }
    }
}
