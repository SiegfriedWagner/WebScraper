using CLIScraper.WebPageParsers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Emgu.CV;

namespace WebScraperWPF.Models
{
    class ImageProcessModel
    {
        
        string cacheDirectory;
        public Action OnProcessedImageChange;
        int currentIndex = -1;
        List<KeyValuePair<ImageSearchResult, Mat>> ImagesToProcess = null;
        public ImageProcessModel(string cacheDirector)
        {
            ImagesToProcess = new List<KeyValuePair<ImageSearchResult, Mat>>();
            
        }
        public KeyValuePair<ImageSearchResult, Mat> Current
        {
            get
            {
                if (ImagesToProcess.Count == 0 || currentIndex == -1)
                    return new KeyValuePair<ImageSearchResult, Mat>(null, null);
                return ImagesToProcess[currentIndex];
            }
        }
        public void Next()
        {
            if (ImagesToProcess.Count == 0)
                return;
            var prevIndex = currentIndex;
            currentIndex = Math.Min(ImagesToProcess.Count - 1, currentIndex + 1);
            if (prevIndex != currentIndex)
                OnProcessedImageChange.Invoke();
        }
        public void Previous()
        {
            if (ImagesToProcess.Count == 0)
                return;
            var prevIndex = currentIndex;
            currentIndex = Math.Max(0, currentIndex - 1);
            if (prevIndex != currentIndex)
                OnProcessedImageChange.Invoke();
        }

        internal void Add(ImageSearchResult param)
        {
            if (!ImagesToProcess.Where(k => k.Key == param).Any())
            {   
                ImagesToProcess.Add(new KeyValuePair<ImageSearchResult, Mat>(param, null));
            }  
        }
    }
}
