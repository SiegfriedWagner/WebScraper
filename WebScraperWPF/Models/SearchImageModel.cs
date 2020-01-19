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
        private string CacheDirectory;
        PageLoader pageLoader;
        private object searchResultsLock = new object();
        public SearchImageModel(string cacheDirectory)
        {
            pageLoader = PageLoader.GetPageLoader();
            if (!Directory.Exists(cacheDirectory))
                Directory.CreateDirectory(cacheDirectory);
            CacheDirectory = cacheDirectory;
            CollectionChanged += DebugNotify;
            SearchResults = new ObservableCollection<ImageSearchResult>();
            BindingOperations.EnableCollectionSynchronization(SearchResults, searchResultsLock);
            
        }

        private static void DebugNotify(object secnder, NotifyCollectionChangedEventArgs e)
        {
            Debug.WriteLine("CollectionChanged call");
        }   
        ~SearchImageModel()
        {
            Directory.Delete(CacheDirectory, true);
        }
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public ObservableCollection<ImageSearchResult> SearchResults
        {
            set; get;
        }
    public async void SearchPhrase(string phrase)
    {
            PageLoader pageLoader = PageLoader.GetPageLoader();
            List<Func<string, Task<ImageSearchResult>>> imageSearchResultsFunc = null;
            for (int i = 0; i < 5; i++)
            {
                var page = await pageLoader.LoadPageAsync($@"https://www.google.com/search?q={phrase}&source=lnms&tbm=isch");
                var imageUrlGenerator = GoogleImageDOMParser.GetUrls(page);
                imageSearchResultsFunc = imageUrlGenerator.ToList();
                if (imageSearchResultsFunc.Count > 0)
                    break;
            }
            if (imageSearchResultsFunc == null)
                throw new InvalidDataException($"After {5} attempts no search results was found!");
            var imageSearchResultTasks = imageSearchResultsFunc
                .AsParallel()
                .Select(o => o(CacheDirectory))
                .ToList();
            imageSearchResultTasks.ForEach((imageResultTask) =>
            //await Task.Factory.StartNew(() =>
            {
            //    Parallel.ForEach(imageSearchResultTasks, (imageResultTask) =>
            //    {
                    Stopwatch watch = new Stopwatch();
                    watch.Start();
                    //Debug.WriteLine($"Started downloading {imageResultTask.Name} from [{imageResultTask.ImageWebUrl}]");
                    try
                    {
                        imageResultTask.RunSynchronously();
                        imageResultTask.Wait();
                        //var filePath = Path.Combine(CacheDirector, $"{imageResultTask.Name}.{imageResultTask.FileExtension}");
                        //webClient.DownloadFile(imageResultTask.ImageWebUrl, filePath);
                        lock (searchResultsLock)
                        {
                            var added = imageResultTask.Result;
                            SearchResults.Add(added);
                            //CollectionChanged.BeginInvoke(SearchResults, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, added), (o) => { }, SearchResults);
                        }
                        Debug.WriteLine($"Finished downloading {imageResultTask.Result.Name} in {watch.ElapsedMilliseconds}ms", "INFO");
                    }
                    catch
                    {
                        Debug.WriteLine("Exception occured");
                    }
                    watch.Stop();

                });
            //});
            Debug.WriteLine("All images were downloaded succesfully", "INFO");
        }
    }
}
