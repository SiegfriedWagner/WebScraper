using CLIScraper;
using CLIScraper.WebPageParsers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
namespace WebScraperWPF.Model
{
    public class Model: INotifyCollectionChanged
    {
        private string CacheDirector;
        PageLoader pageLoader;
        public Model(string cacheDirectory)
        {
            pageLoader = PageLoader.GetPageLoader();
            if (!Directory.Exists(cacheDirectory))
                Directory.CreateDirectory(cacheDirectory);
            CacheDirector = cacheDirectory;
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public ObservableCollection<CachedImageSearchResult> SearchResults = new ObservableCollection<CachedImageSearchResult> {
                new CachedImageSearchResult(imageSearchResult:new ImageSearchResult(ImageUrl: @"D:\git\WebScraper\CLIScraper\TestImages\1.jpg", Name: "miecz 1"), imagePathUri:@"D:\git\WebScraper\CLIScraper\TestImages\1.jpg"),
                new CachedImageSearchResult(imageSearchResult:new ImageSearchResult(ImageUrl: @"D:\git\WebScraper\CLIScraper\TestImages\2.jpg", Name: "miecz 2"), imagePathUri:@"D:\git\WebScraper\CLIScraper\TestImages\2.jpg")
        };
    public void SearchPhrase(string phrase)
        {
            PageLoader pageLoader = PageLoader.GetPageLoader();
            List<ImageSearchResult> imageSearchResults = null;
            for (int i = 0; i < 5; i++)
            {
                var task = pageLoader.LoadPageAsync($@"https://www.google.com/search?q={phrase}&source=lnms&tbm=isch");
                task.Wait();
                var imageUrlGenerator = GoogleImageDOMParser.GetUrls(task.Result);
                imageSearchResults = imageUrlGenerator.ToList();
                if (imageSearchResults.Count > 0)
                    break;
            }
            if (imageSearchResults == null)
                throw new InvalidDataException($"After {5} attempts no search results was found!");
            foreach( var imageResult in imageSearchResults)
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                Debug.WriteLine($"Started downloading {imageResult.Name} from [{imageResult.ImageWebUrl}]", "INFO");
                WebClient webClient = new WebClient();
                try
                {
                    webClient.DownloadFile(imageResult.ImageWebUrl, Path.Combine(CacheDirector, $"{imageResult.Name}.{imageResult.FileExtension}"));
                    Debug.WriteLine($"Finished downloading {imageResult.Name} in {watch.ElapsedMilliseconds}ms");
                }
                catch
                {
                    Debug.WriteLine("Exception occured", "ERROR");
                }
                webClient.Dispose();
                watch.Stop();
            }
            //imageSearchResults.ForEach((imageResult) =>
            //// Parallel.ForEach(imageSearchResults, (imageResult) =>
            //{
            //    Stopwatch watch = new Stopwatch();
            //    watch.Start();
            //    Debug.WriteLine($"Started downloading {imageResult.Name} from [{imageResult.ImageWebUrl}]");
            //    WebClient webClient = new WebClient();
            //    try
            //    {
            //        webClient.DownloadFile(imageResult.ImageWebUrl, Path.Combine(CacheDirector, $"{imageResult.Name}.{imageResult.FileExtension}"));
            //        Debug.WriteLine($"Finished downloading {imageResult.Name} in {watch.ElapsedMilliseconds}ms");
            //    }
            //    catch
            //    {
            //        Debug.WriteLine("Exception occured");
            //    }
            //    watch.Stop();
                
            //});
            Debug.WriteLine("All images were downloaded succesfully", "INFO");
        }
    }
}
