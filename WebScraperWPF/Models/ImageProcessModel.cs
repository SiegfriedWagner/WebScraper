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
    class ImageProcessModel
    {
        string cacheDirectory;
        int currentIndex = -1;
        List<ImageSearchResult> ImagesToProcess = null;
        public ImageProcessModel(string cacheDirector)
        {
            ImagesToProcess = new List<ImageSearchResult>();
            
        }
        private static void DebugNotify(object secnder, NotifyCollectionChangedEventArgs e)
        {
            Debug.WriteLine("CollectionChanged call");
        }
        public ImageSearchResult Current
        {
            get
            {
                if (ImagesToProcess.Count == 0 || currentIndex == -1)
                    return null;
                return ImagesToProcess[currentIndex];
            }
        }
        public ImageSearchResult Next()
        {
            if (ImagesToProcess.Count == 0)
                return null;
            currentIndex = Math.Min(ImagesToProcess.Count - 1, currentIndex + 1);
            return ImagesToProcess[currentIndex];
        }
        public ImageSearchResult Previous()
        {
            if (ImagesToProcess.Count == 0)
                return null;
            currentIndex = Math.Max(0, currentIndex - 1);
            return ImagesToProcess[currentIndex];
        }

        internal void Add(ImageSearchResult param)
        {
            if (!ImagesToProcess.Contains(param))
                ImagesToProcess.Add(param);
        }
    }
}
