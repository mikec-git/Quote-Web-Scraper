using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuoteWebScraper
{
    class UserInput
    {
        private static char[] stringSplit   = new char[] { ',' , ':', '.' , '!' };
        private static char[] delim         = new char[] { ' ', ',' };

        public static void GetPages(PageAndUrl pageAndUrl, string numOfPagesString)
        {
            numOfPagesString = NoPageInputCheck(numOfPagesString);

            string[] pageRangeString = numOfPagesString.Split(stringSplit, StringSplitOptions.RemoveEmptyEntries);
            List<string> pageRange = new List<string>() { pageRangeString[0] };

            int pageRangeStrlength = pageRangeString.Length;

            if (pageRangeStrlength > 1)
            {
                for (int i = 0; i < pageRangeStrlength; i++)
                {
                    if (!pageRange[0].Equals(pageRangeString[i]))
                    {
                        pageRange.Add(pageRangeString[i]);
                        break;
                    }
                }
            }

            List<int> pages = pageRange.ConvertAll(x => Convert.ToInt32(x));

            for (int i = 0; i < pageRange.Count; i++)
            {
                if (pages[i] <= 0) pages[i] = 1;
            }

            pageAndUrl.pages = pages.Distinct().ToList();
        }

        public static void GetUrl(PageAndUrl pageAndUrl, string keywords)
        {
            StringBuilder newUrl = new StringBuilder();
            newUrl.Append(pageAndUrl.url);

            // Getting URL Stem
            if (!string.IsNullOrEmpty(keywords))
            {
                string[] keywordList = keywords.Split(delim, StringSplitOptions.RemoveEmptyEntries);
                int numOfKeywords    = keywordList.Length;

                newUrl.Append($"/tag?id={keywordList[0]}");

                if (numOfKeywords > 1)
                {
                    for (int i = 1; i < numOfKeywords; i++)
                        newUrl.Append($"+{keywordList[i]}");
                }
            }
            newUrl.Append("&page=");
            pageAndUrl.url = newUrl.ToString();
        }

        private static string NoPageInputCheck(string numOfPagesString)
        {
            if (string.IsNullOrEmpty(numOfPagesString)) return "1";
            else                                        return numOfPagesString;
        }        
    }
}
