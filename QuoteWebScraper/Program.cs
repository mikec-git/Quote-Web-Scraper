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
            string url = "https://www.goodreads.com/quotes";

            Console.WriteLine("Generate by keyword (separate with spaces or commas): ");
            string keywords = Console.ReadLine();

            Console.WriteLine("\nEnter the number of pages to query. If you want a range of pages, enter the numbers separated by commas: ");
            string numOfPagesString = Console.ReadLine();

            PageAndUrl pageAndUrl = UserInput.GetUrlAndPages(url, keywords, numOfPagesString);

            // Starting timer to compare Async vs Sync
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
