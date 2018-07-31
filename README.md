# Quote Web Scraper
- This program will scrape GoodReads' quote library based on user input keywords and number of pages.

### Performance
- This program uses asynchronous programming to take advantage of multi-threading as synchronously downloading data from a webpage can take some time.

The first two figures show the time elapsed to fully run the program for two webpages. The only difference being the first figure ran synchronously, while the second ran asynchronously. There is a noticable performance boost in the latter.

<img src="Performance%20Details/Sync_Motivational_2Pages.png" width="700" >

<img src="Performance%20Details/Async_Motivational_2Pages.png" width="700" >

The same goes for downloading 10 webpages, but the performance boost is much greater as shown below.

<img src="Performance%20Details/Sync_Motivational_10Pages.png" width="700" >

<img src="Performance%20Details/Async_Motivational_10Pages.png" width="700" >

