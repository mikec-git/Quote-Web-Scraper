using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace QuoteWebScraper
{
    public class Scraper
    {
        QuoteData quoteData;

        HtmlWeb web;
        List<Task<HtmlDocument>> htmlDocuments;

        List<HtmlNode> quoteHtml;
        List<HtmlNode> authorHtml;

        public Scraper()
        {
            quoteData = new QuoteData();

            web = new HtmlWeb();
            htmlDocuments = new List<Task<HtmlDocument>>();

            quoteHtml = new List<HtmlNode>();
            authorHtml = new List<HtmlNode>();
        }

        public async Task PageLooperAsync(string url, int pages)
        {
            string urlPage;
            int listLength;

            for (int i = 1; i <= pages; i++)
            {
                urlPage = url + $"{i}&utf8=✓";
                htmlDocuments.Add(Task.Run(() => web.LoadFromWebAsync(urlPage)));
            }

            var allHtmlDocuments = await Task.WhenAll(htmlDocuments);

            foreach(HtmlDocument document in allHtmlDocuments)
            {
                GetQuoteAndAuthor(document, out listLength);
                SeparateIntoQuotesAndAuthors(listLength);
            }
        }

        private void GetQuoteAndAuthor(HtmlDocument htmlDoc, out int listLength)
        {
            // Stores the specified descendants for the quotes and authors
            quoteHtml = htmlDoc.DocumentNode.Descendants("div")
                .Where(node => node.GetAttributeValue("class", "").Equals("quoteText")).ToList();

            authorHtml = htmlDoc.DocumentNode.Descendants("a")
                .Where(node => node.GetAttributeValue("href", "").Contains("/author/")
                && node.GetAttributeValue("class", "").Contains("authorOrTitle")).ToList();

            listLength = quoteHtml.Count;
        }

        private void SeparateIntoQuotesAndAuthors(int listLength)
        {
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
    }
}
