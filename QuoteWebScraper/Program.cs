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
        public static Stopwatch stopwatch = new Stopwatch();

        static void Main(string[] args)
        {
            Console.Title = "Quote Generator by Mike";
            string url = "https://www.goodreads.com/quotes";

            Console.WriteLine("Generate quotes by keyword (separate with spaces/commas): ");
            string keywords = Console.ReadLine();

            Console.WriteLine("\nEnter the page to query.\nTo get a range of pages, enter the start and end numbers of the range, separated by commas: ");
            string numOfPagesString = Console.ReadLine();

            PageAndUrl pageAndUrl = new PageAndUrl(url);
            UserInput.GetPages(pageAndUrl, numOfPagesString);
            UserInput.GetUrl(pageAndUrl, keywords);

            // Starting timer
            stopwatch.Start();

            Scraper scraper = new Scraper();
            scraper.PageLooperAsync(pageAndUrl).Wait();

            // Ending timer
            stopwatch.Stop();

            Console.WriteLine($"\nFinished. Time Elapsed: {stopwatch.Elapsed}");

            Console.ReadKey();
        }
    }
}
