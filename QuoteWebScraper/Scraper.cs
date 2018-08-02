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
        private List<QuoteData> quoteData;
        private List<Task<HtmlDocument>> htmlDocuments;
        private List<Task<QuoteHtmlData>> htmlNodes;

        public Scraper()
        {
            web             = new HtmlWeb();
            quoteData       = new List<QuoteData>();
            htmlDocuments   = new List<Task<HtmlDocument>>();
            htmlNodes       = new List<Task<QuoteHtmlData>>();
        }
        #endregion

        public async Task PageLooperAsync(PageAndUrl pageAndUrl)
        {
            #region Asyncronously loads web pages
            if (pageAndUrl.pages.Count == 1)
            {
                if (pageAndUrl.pages[0] != 1)
                {
                    pageAndUrl.pages[0] = await CheckMaxPageNumber(pageAndUrl.url, pageAndUrl.pages[0]);
                }

                string urlPage = pageAndUrl.url + $"{pageAndUrl.pages[0]}&utf8=✓";
                htmlDocuments.Add(Task.Run(() => web.LoadFromWebAsync(urlPage)));
            }
            else
            {
                int startPage   = pageAndUrl.pages.Min();
                int endPage     = pageAndUrl.pages.Max();
                int maxIndex    = pageAndUrl.pages.IndexOf(endPage);

                pageAndUrl.pages[maxIndex] = await CheckMaxPageNumber(pageAndUrl.url, endPage);
                endPage = pageAndUrl.pages[maxIndex];

                if (endPage < startPage)
                {
                    startPage = endPage;
                }

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
            {
                htmlNodes.Add(Task.Run(() => GetQuoteAndAuthor(document)));
            }

            QuoteHtmlData[] allNodes = await Task.WhenAll(htmlNodes);
            Console.WriteLine("Nodes extracted.\n");
            #endregion

            #region Stores quotes & authors into data instance
            foreach (QuoteHtmlData data in allNodes)
            {
                SeparateIntoQuotesAndAuthors(data);
            }

            Console.WriteLine("Quotes and authors parsed.\n");
            #endregion
        }

        private async Task<int> CheckMaxPageNumber(string url, int page)
        {
            HtmlDocument firstPage = await web.LoadFromWebAsync(url);
            List<HtmlNode> resultsTotal = firstPage.DocumentNode.Descendants("span").Where(node => node.GetAttributeValue("class", "").Equals("smallText")).ToList();
            char[] delim = new char[] { ' ', ')' };

            string numOfResultsString = resultsTotal[0].InnerHtml.Split(delim, StringSplitOptions.RemoveEmptyEntries)[3].Replace(",", "");
            int results = Convert.ToInt32(numOfResultsString);

            // Each page has 30 results max
            if (results <= 30)
            {
                return 1;
            }
            // Checking if inputted page exceeds total number of result pages
            else if (results > 30 && (results / (page*30)) < 1)
            {
                return (int)Math.Ceiling((double)results / 30);
            }

            return page;
        }

        private async Task<QuoteHtmlData> GetQuoteAndAuthor(HtmlDocument htmlDoc)
        {
            QuoteHtmlData quoteHtmlData = new QuoteHtmlData();

            quoteHtmlData.quotesHtml =  htmlDoc.DocumentNode.Descendants("div")
                                               .Where(node => node.GetAttributeValue("class","").Equals("quoteText")).ToList();

            quoteHtmlData.authorsHtml = htmlDoc.DocumentNode.Descendants("a")
                                               .Where(node => node.GetAttributeValue("href","").Contains("/author/") && node.GetAttributeValue("class","").Contains("authorOrTitle")).ToList();
            
            quoteHtmlData.numOfQuotes = quoteHtmlData.quotesHtml.Count;

            return quoteHtmlData;
        }

        private void SeparateIntoQuotesAndAuthors(QuoteHtmlData quoteData)
        {
            string[] quoteDelim = new string[] { "&ldquo;", "&rdquo;" };
            string quotation = "&#39;";
            string extractedQuote;
            int numberOfQuotes = quoteData.numOfQuotes;

            for (int j = 0; j < numberOfQuotes; j++)
            {
                extractedQuote = quoteData.quotesHtml[j].InnerHtml.Split(quoteDelim, StringSplitOptions.None)[1].Replace("<br>", "\n");

                QuoteData tempData = new QuoteData()
                {
                    quotes = "\"" + extractedQuote + "\"",
                    authors = quoteData.authorsHtml[j].InnerText.Replace(quotation, "'")
                };

                this.quoteData.Add(tempData);
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
