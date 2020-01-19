using CLIScraper.WebPageParsers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebScraperWPF.Models
{
    class ImageProcessModel : INotifyCollectionChanged
    {
        string cacheDirectory;
        public ImageProcessModel(string cacheDirector)
        {

            ImagesToProcess = new ObservableCollection<ImageSearchResult>();
            CollectionChanged += DebugNotify;
        }
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public ObservableCollection<ImageSearchResult> ImagesToProcess
        {
            set; get;
        }
        private static void DebugNotify(object secnder, NotifyCollectionChangedEventArgs e)
        {
            Debug.WriteLine("CollectionChanged call");
        }
    }
}
