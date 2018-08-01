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
        HtmlWeb web;
        QuoteData quoteData;
        
        List<Task<HtmlDocument>> htmlDocuments;
        List<Task<QuoteHtmlData>> htmlNodes;
        
        public Scraper()
        {
            web             = new HtmlWeb();
            quoteData       = new QuoteData();
            htmlDocuments   = new List<Task<HtmlDocument>>();
            htmlNodes       = new List<Task<QuoteHtmlData>>();
        }

        public async Task PageLooperAsync(PageAndUrl pageAndUrl)
        {
            // Stores async processes into a list of tasks
            if (pageAndUrl.pages.Count == 1)
            {
                string urlPage = pageAndUrl.url + $"{pageAndUrl.pages.First()}&utf8=✓";
                htmlDocuments.Add(Task.Run(() => web.LoadFromWebAsync(urlPage)));
            }
            else
            {
                int startPage = pageAndUrl.pages.Min();
                int endPage = pageAndUrl.pages.Max();

                for (int i = startPage; i <= endPage; i++)
                {
                    string urlPage = pageAndUrl.url + $"{i}&utf8=✓";
                    htmlDocuments.Add(Task.Run(() => web.LoadFromWebAsync(urlPage)));
                }
            }

            var allHtmlDocuments = await Task.WhenAll(htmlDocuments);
            Console.WriteLine("\nWebpages Downloaded.\n");

            // Exatracts the quote/author html nodes from each webpage
            foreach(HtmlDocument document in allHtmlDocuments)
                htmlNodes.Add(Task.Run(() => GetQuoteAndAuthor(document)));

            var allNodes = await Task.WhenAll(htmlNodes);
            Console.WriteLine("Nodes extracted.\n");

            // Stores each quote and author into the QuoteData instance
            foreach(QuoteHtmlData data in allNodes)
                SeparateIntoQuotesAndAuthors(data);

            Console.WriteLine("Quotes and authors parsed.\n");
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

            quoteHtmlData.quotesHtml =  htmlDoc.DocumentNode.Descendants("div")
                                        .Where(node => node.GetAttributeValue("class", "").Equals("quoteText")).ToList();

            quoteHtmlData.authorsHtml = htmlDoc.DocumentNode.Descendants("a")
                                        .Where(node => node.GetAttributeValue("href", "").Contains("/author/") && 
                                        node.GetAttributeValue("class", "").Contains("authorOrTitle")).ToList();
            
            quoteHtmlData.numOfQuotes = quoteHtmlData.quotesHtml.Count;

            return quoteHtmlData;
        }

        private void SeparateIntoQuotesAndAuthors(QuoteHtmlData data)
        {
            string[] quoteDelim = new string[] { "&ldquo;", "&rdquo;" };
            string extractedQuote;

            for (int j = 0; j < data.numOfQuotes; j++)
            {
                extractedQuote = data.quotesHtml[j].InnerHtml
                                .Split(quoteDelim, StringSplitOptions.None)[1]
                                .Replace("<br>", "\n");

                quoteData.quotes.Add("\"" + extractedQuote + "\"");
                quoteData.authors.Add(data.authorsHtml[j].InnerText.Replace("&#39;", "'"));
            }
        }
    }
}
