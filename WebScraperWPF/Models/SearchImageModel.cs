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
using System.Windows.Data;

namespace WebScraperWPF.Model
{
    public class SearchImageModel: INotifyCollectionChanged
    {
        private string CacheDirector;
        PageLoader pageLoader;
        private object searchResultsLock = new object();
        public SearchImageModel(string cacheDirectory)
        {
            pageLoader = PageLoader.GetPageLoader();
            if (!Directory.Exists(cacheDirectory))
                Directory.CreateDirectory(cacheDirectory);
            CacheDirector = cacheDirectory;
            CollectionChanged += DebugNotify;
            SearchResults = new ObservableCollection<CachedImageSearchResult>();
            BindingOperations.EnableCollectionSynchronization(SearchResults, searchResultsLock);
            
        }

        private static void DebugNotify(object secnder, NotifyCollectionChangedEventArgs e)
        {
            Debug.WriteLine("CollectionChanged call");
        }   
        ~SearchImageModel()
        {
            Directory.Delete(CacheDirector, true);
        }
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public ObservableCollection<CachedImageSearchResult> SearchResults
        {
            set; get;
        }
    public async void SearchPhrase(string phrase)
        {
            PageLoader pageLoader = PageLoader.GetPageLoader();
            List<ImageSearchResult> imageSearchResults = null;
            for (int i = 0; i < 5; i++)
            {
                var page = await pageLoader.LoadPageAsync($@"https://www.google.com/search?q={phrase}&source=lnms&tbm=isch");
                var imageUrlGenerator = GoogleImageDOMParser.GetUrls(page);
                imageSearchResults = imageUrlGenerator.ToList();
                if (imageSearchResults.Count > 0)
                    break;
            }
            if (imageSearchResults == null)
                throw new InvalidDataException($"After {5} attempts no search results was found!");

            //imageSearchResults.ForEach((imageResult) =>
            await Task.Factory.StartNew(() =>
            {
                Parallel.ForEach(imageSearchResults, (imageResult) =>
                {
                    Stopwatch watch = new Stopwatch();
                    watch.Start();
                    Debug.WriteLine($"Started downloading {imageResult.Name} from [{imageResult.ImageWebUrl}]");
                    WebClient webClient = new WebClient();
                    try
                    {
                        var filePath = Path.Combine(CacheDirector, $"{imageResult.Name}.{imageResult.FileExtension}");
                        webClient.DownloadFile(imageResult.ImageWebUrl, filePath);
                        lock (searchResultsLock)
                        {
                            var added = new CachedImageSearchResult(imageResult, filePath);
                            SearchResults.Add(added);
                            //CollectionChanged.BeginInvoke(SearchResults, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, added), (o) => { }, SearchResults);
                        }
                        Debug.WriteLine($"Finished downloading {imageResult.Name} in {watch.ElapsedMilliseconds}ms", "INFO");
                    }
                    catch
                    {
                        Debug.WriteLine("Exception occured");
                    }
                    watch.Stop();

                });
            });
            Debug.WriteLine("All images were downloaded succesfully", "INFO");
        }
    }
}
