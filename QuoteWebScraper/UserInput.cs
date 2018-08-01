using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuoteWebScraper
{
    class UserInput
    {
        private static char[] stringSplit = new char[] { ',' , ':', '.' , '!' };
        private static char[] delim = new char[] { ' ', ',' };

        public static PageAndUrl GetUrlAndPages(string url, string keywords, string numOfPagesString)
        {
            PageAndUrl pageAndUrl = new PageAndUrl(url);

            numOfPagesString = PageEmptyCheck(numOfPagesString);

            string[] pageRange = numOfPagesString.Split(stringSplit, StringSplitOptions.RemoveEmptyEntries);
            
            // Getting Page Numbers to Query
            if (pageRange.Length > 1)
            {
                pageAndUrl.pages = NumOfPagesArray(pageRange);

                if (pageAndUrl.pages.Distinct().Count() == 1)
                    pageAndUrl.pages = new List<int> { pageAndUrl.pages.Distinct().First() };
            }
            else
            {
                pageAndUrl.pages = new List<int> { Convert.ToInt32(pageRange.First()) };
            }

            // Getting URL Stem
            if (!string.IsNullOrEmpty(keywords))
            {
                string[] keywordList = keywords.Split(delim, StringSplitOptions.RemoveEmptyEntries);
                int numOfKeywords = keywordList.Length;

                pageAndUrl.url = pageAndUrl.url + $"/tag?id={keywordList[0]}";

                if (numOfKeywords > 1)
                {
                    for (int i = 1; i < numOfKeywords; i++)
                        pageAndUrl.url = pageAndUrl.url + $"+{keywordList[i]}";
                }

                pageAndUrl.url = pageAndUrl.url + "&page=";
            }
            else { pageAndUrl.url = pageAndUrl.url + "?page="; }

            return pageAndUrl;
        }

        private static string PageEmptyCheck(string numOfPagesString)
        {
            if (string.IsNullOrEmpty(numOfPagesString)) return "1";
            else                                        return numOfPagesString;
        }

        private static List<int> NumOfPagesArray(string[] pageRange)
        {
            List<string> pageRangeTwo = pageRange.Take(2).ToList();
            List<int> pages = pageRangeTwo.ConvertAll(x => Convert.ToInt32(x));

            for (int i = 0; i < pages.Count; i++)
            {
                if (pages[i] <= 0)
                    pages[i] = 1;
            }

            return pages;
        }
    }
}
