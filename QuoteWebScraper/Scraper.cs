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
        #region Initializing instance variables
        private HtmlWeb web;
        private QuoteData quoteData;
        private List<Task<HtmlDocument>> htmlDocuments;
        private List<Task<QuoteHtmlData>> htmlNodes;
        
        public Scraper()
        {
            web             = new HtmlWeb();
            quoteData       = new QuoteData();
            htmlDocuments   = new List<Task<HtmlDocument>>();
            htmlNodes       = new List<Task<QuoteHtmlData>>();
        }
        #endregion

        public async Task PageLooperAsync(PageAndUrl pageAndUrl)
        {
            #region Asyncronously loads web pages
            if (pageAndUrl.pages.Count == 1)
            {
                if (pageAndUrl.pages.First() != 1)
                    pageAndUrl.pages[0] = await CheckMaxPageNumber(pageAndUrl.url, pageAndUrl.pages.First());

                string urlPage = pageAndUrl.url + $"{pageAndUrl.pages.First()}&utf8=✓";

                htmlDocuments.Add(Task.Run(() => web.LoadFromWebAsync(urlPage)));
            }

            else
            {
                int startPage = pageAndUrl.pages.Min();
                int minIndex = pageAndUrl.pages.IndexOf(startPage);

                int endPage = pageAndUrl.pages.Max();
                int maxIndex = pageAndUrl.pages.IndexOf(endPage);

                pageAndUrl.pages[maxIndex] = await CheckMaxPageNumber(pageAndUrl.url, endPage);
                endPage = pageAndUrl.pages[maxIndex];

                if (endPage < startPage)
                    startPage = endPage;

                for (int i = startPage; i <= endPage; i++)
                {
                    string urlPage = pageAndUrl.url + $"{i}&utf8=✓";
                    htmlDocuments.Add(Task.Run(() => web.LoadFromWebAsync(urlPage)));
                }
            }

            var allHtmlDocuments = await Task.WhenAll(htmlDocuments);
            Console.WriteLine("\nWebpages Downloaded.\n");
            #endregion

            #region Exatracts quote/author html nodes from each page
            foreach (HtmlDocument document in allHtmlDocuments)
                htmlNodes.Add(Task.Run(() => GetQuoteAndAuthor(document)));

            var allNodes = await Task.WhenAll(htmlNodes);
            Console.WriteLine("Nodes extracted.\n");
            #endregion

            #region Stores quotes & authors into data instance
            foreach (QuoteHtmlData data in allNodes)
                SeparateIntoQuotesAndAuthors(data);

            Console.WriteLine("Quotes and authors parsed.\n");
            #endregion
        }

        private async Task<int> CheckMaxPageNumber(string url, int page)
        {
            char[] delim = new char[] { ' ', ')' };
            string extractedQuote;

            HtmlDocument lastPageDoc = new HtmlDocument();
            lastPageDoc = await web.LoadFromWebAsync(url);

            var resultsTotal = lastPageDoc.DocumentNode.Descendants("span")
                .Where(node => node.GetAttributeValue("class", "").Equals("smallText")).ToList();

            extractedQuote = resultsTotal.First().InnerHtml.Split(delim, StringSplitOptions.RemoveEmptyEntries)[3].Replace(",", "");

            int results = Convert.ToInt32(extractedQuote);

            // Each page has 30 results max
            if (results <= 30)
                return 1;   

            else if (results > 30 && (results / (page * 30)) < 1)
                return (int)Math.Ceiling((double)results / 30);

            return page;
        }

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
    }
}
