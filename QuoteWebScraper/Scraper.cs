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
        private List<QuoteData> QuotesAndAuthorsText;
        private List<QuoteHtmlData> QuoteAndAuthorNodes;
        private List<Task<HtmlDocument>> htmlDocsList;

        public Scraper()
        {
            web                     = new HtmlWeb();
            QuotesAndAuthorsText    = new List<QuoteData>();
            QuoteAndAuthorNodes     = new List<QuoteHtmlData>();
            htmlDocsList            = new List<Task<HtmlDocument>>();
        }
        #endregion

        public async Task PageScraperAsync(PageAndUrl pageAndUrl)
        {
            #region Asyncronously loads web pages
            if (pageAndUrl.pages.Count == 1)
            {
                int startPage;

                if(pageAndUrl.pages[0] == 1)    startPage = pageAndUrl.pages[0];
                else                            startPage = await MaxPossiblePageCheck(pageAndUrl.url, pageAndUrl.pages[0]);

                string urlPage = pageAndUrl.url + $"{startPage}&utf8=✓";
                htmlDocsList.Add(Task.Run(() => web.LoadFromWebAsync(urlPage)));
            }
            else
            {
                int startPage   = pageAndUrl.pages[0];
                int endPage     = await MaxPossiblePageCheck(pageAndUrl.url, pageAndUrl.pages[1]);

                for (int i = startPage; i <= endPage; i++)
                    htmlDocsList.Add(Task.Run(() => web.LoadFromWebAsync(pageAndUrl.url + $"{i}&utf8=✓")));
            }

            var allHtmlDocuments = await Task.WhenAll(htmlDocsList);
            Console.WriteLine("\nWebpages Downloaded.\n");
            #endregion

            #region Exatracts quote and author html nodes from each page
            foreach (HtmlDocument document in allHtmlDocuments)
                QuoteAndAuthorNodes.Add(GetQuoteAndAuthorNode(document));
            
            QuoteHtmlData[] QuoteAuthorNodesArray = QuoteAndAuthorNodes.ToArray();
            Console.WriteLine("Nodes extracted.\n");
            #endregion

            #region Stores quotes & authors into quoteData
            foreach (QuoteHtmlData node in QuoteAuthorNodesArray)
                GetQuotesAndAuthorsText(node);

            Console.WriteLine("Quotes and authors parsed.\n");
            #endregion
        }

        private async Task<int> MaxPossiblePageCheck(string url, int page)
        {
            char[] delim = new char[] { ' ', ')' };

            HtmlDocument firstPage = await web.LoadFromWebAsync(url);

            List<HtmlNode> resultsTotal = firstPage.DocumentNode.Descendants("span").Where(node => node.GetAttributeValue("class", "").Equals("smallText")).ToList();

            string numOfResultsString = resultsTotal[0].InnerHtml.Split(delim, StringSplitOptions.RemoveEmptyEntries)[3].Replace(",", "");

            int results = Convert.ToInt32(numOfResultsString);

            // Each page has 30 results max
            if (results <= 30)  return 1;

            // Check if use input page greater than max result page
            if (results > 30 && (results / (page*30)) < 1)
                return (int)Math.Ceiling((double)results / 30);

            return page;
        }

        private QuoteHtmlData GetQuoteAndAuthorNode(HtmlDocument htmlDoc)
        {
            QuoteHtmlData quoteHtmlData = new QuoteHtmlData();

            quoteHtmlData.quotesHtml = htmlDoc.DocumentNode.Descendants("div")
                                               .Where(node => node.GetAttributeValue("class", "").Equals("quoteText")).ToList();

            quoteHtmlData.authorsHtml = htmlDoc.DocumentNode.Descendants("a")
                                               .Where(node => node.GetAttributeValue("href", "").Contains("/author/") && node.GetAttributeValue("class", "").Contains("authorOrTitle")).ToList();

            quoteHtmlData.numOfQuotes = quoteHtmlData.quotesHtml.Count;

            return quoteHtmlData;
        }

        private void GetQuotesAndAuthorsText(QuoteHtmlData quoteAuthorNode)
        {
            string[] delim = new string[] { "&ldquo;", "&rdquo;" };

            for (int j = 0; j < quoteAuthorNode.numOfQuotes; j++)
            {
                string extractedQuote = quoteAuthorNode.quotesHtml[j].InnerHtml.Split(delim, StringSplitOptions.None)[1].Replace("<br>", "\n");

                QuoteData quoteAuthorPair = new QuoteData
                (
                    Quote:  @"" + extractedQuote + "",
                    Author: quoteAuthorNode.authorsHtml[j].InnerText.Replace("&#39;", "'")
                );

                QuotesAndAuthorsText.Add(quoteAuthorPair);
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
