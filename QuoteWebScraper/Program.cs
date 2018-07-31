using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace QuoteWebScraper
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();

            // Scraping from https://goodreads.com for desired quotes
            string url = "https://www.goodreads.com/quotes";

            Console.WriteLine("Generate by keyword (separate with spaces or commas): ");
            string keywords = Console.ReadLine();

            Console.WriteLine("\nEnter the number of pages to query: ");
            string numberOfPagesString = Console.ReadLine();

            if (string.IsNullOrEmpty(numberOfPagesString))
                    numberOfPagesString = "1";

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

            else
            {
                url = url + "?page=";
            }

            // Starting timer to compare Async vs Sync
            stopwatch.Start();

            Scraper scraper = new Scraper();
            scraper.PageLooperAsync(url, numOfPages).Wait();

            // Ending timer
            stopwatch.Stop();
            Console.WriteLine($"\nFinished. Time Elapsed: {stopwatch.Elapsed}");

            Console.ReadKey();
        }
    }
}
