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
using WebScraperWPF.Behaviors;
using Emgu.CV.Structure;

namespace WebScraperWPF.Models
{
    class ImageProcessModel
    {
        
        string cacheDirectory;
        public Action OnProcessedImageChange;
        int currentIndex = -1;
        List<KeyValuePair<ImageSearchResult, Emgu.CV.Image<Bgr, byte>>> ImagesToProcess = null;
        public ImageProcessModel(string cacheDirector)
        {
            ImagesToProcess = new List<KeyValuePair<ImageSearchResult, Emgu.CV.Image<Bgr, byte>>>();
            
        }
        public KeyValuePair<ImageSearchResult, Image<Bgr, byte>> Current
        {
            get
            {
                if (ImagesToProcess.Count == 0 || currentIndex == -1)
                    return new KeyValuePair<ImageSearchResult, Emgu.CV.Image<Bgr, byte>>(null, null);
                return ImagesToProcess[currentIndex];
            }
            private set
            {
                ImagesToProcess[currentIndex] = value;
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
                ImagesToProcess.Add(new KeyValuePair<ImageSearchResult, Emgu.CV.Image<Bgr, byte>>(param, null));
            }  
        }
        public void CropCurrent(Selection selection)
        {
            if (Current.Value == null)
            {
                var img = new Image<Bgr, byte>(Current.Key.ImagePathUri);
                img.ROI = new Rectangle(
                    (int) (selection.Left * img.Width), 
                    (int) (selection.Top * img.Height), 
                    (int) (selection.Width * img.Width), 
                    (int) (selection.Height * img.Height));
                ImagesToProcess[currentIndex] = new KeyValuePair<ImageSearchResult, Image<Bgr, byte>>(Current.Key, img.Copy());
                img.Dispose();
            }
            OnProcessedImageChange.Invoke();
        }
        public void ResetCurrent()
        {
            if (ImagesToProcess[currentIndex].Value == null)
                return;
            ImagesToProcess[currentIndex].Value.Dispose();
            ImagesToProcess[currentIndex] = new KeyValuePair<ImageSearchResult, Image<Bgr, byte>>(Current.Key, null);
            OnProcessedImageChange.Invoke();
        }
    }
}
