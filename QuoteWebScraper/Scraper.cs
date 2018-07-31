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
        List<Task<QuoteHtmlData>> htmlNodes;
        List<HtmlNode> quoteHtml;
        List<HtmlNode> authorHtml;

        public Scraper()
        {
            quoteData   = new QuoteData();
            web         = new HtmlWeb();

            htmlDocuments   = new List<Task<HtmlDocument>>();
            htmlNodes   = new List<Task<QuoteHtmlData>>();
            quoteHtml   = new List<HtmlNode>();
            authorHtml  = new List<HtmlNode>();
        }

        public async Task PageLooperAsync(string url, int pages)
        {
            string urlPage;

            // Stores async processes into a list of tasks
            for (int i = 1; i <= pages; i++)
            {
                urlPage = url + $"{i}&utf8=✓";
                htmlDocuments.Add(Task.Run(() => web.LoadFromWebAsync(urlPage)));
            }

            var allHtmlDocuments = await Task.WhenAll(htmlDocuments);
            Console.WriteLine("Webpages Downloaded.\n");

            // Exatracts the quote/author html nodes from each webpage
            foreach(HtmlDocument document in allHtmlDocuments)
                htmlNodes.Add(GetQuoteAndAuthor(document));

            var allNodes = await Task.WhenAll(htmlNodes);
            Console.WriteLine("Nodes extracted.\n");

            // Stores each quote and author into the QuoteData instance
            foreach(QuoteHtmlData data in allNodes)
                SeparateIntoQuotesAndAuthors(data);
        }

        //public async Task PageLooperSync(string url, int pages)
        //{
        //    string urlPage;
        //    int listLength;

        //    for (int i = 1; i <= pages; i++)
        //    {
        //        urlPage = url + $"{i}&utf8=✓";
        //        HtmlDocument doc = await web.LoadFromWebAsync(urlPage);

        //        GetQuoteAndAuthor(doc, out listLength);
        //        SeparateIntoQuotesAndAuthors(listLength);
        //    }
        //}

        private async Task<QuoteHtmlData> GetQuoteAndAuthor(HtmlDocument htmlDoc)
        {
            QuoteHtmlData quoteHtmlData = new QuoteHtmlData();

            // Stores the specified descendants for the quotes and authors
            quoteHtml = htmlDoc.DocumentNode.Descendants("div")
                .Where(node => node.GetAttributeValue("class", "").Equals("quoteText")).ToList();

            authorHtml = htmlDoc.DocumentNode.Descendants("a")
                .Where(node => node.GetAttributeValue("href", "").Contains("/author/") && 
                        node.GetAttributeValue("class", "").Contains("authorOrTitle")).ToList();
            
            quoteHtmlData.quotesHtml = quoteHtml;
            quoteHtmlData.authorsHtml = authorHtml;
            quoteHtmlData.numOfQuotes = quoteHtml.Count;

            return quoteHtmlData;
        }

        private void SeparateIntoQuotesAndAuthors(QuoteHtmlData data)
        {
            // Double quotes for this URL show up as the following deliminators
            string[] quoteDelim = new string[] { "&ldquo;", "&rdquo;" };
            string extractedQuote;

            for (int j = 0; j < data.numOfQuotes; j++)
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
