using CLIScraper.WebPageParsers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using WebScraperWPF.Model;
using System.Windows.Input;
using WebScraperWPF.Commands;
using WebScraperWPF.Models;
using System.Windows.Media.Imaging;
using Emgu.CV;
using System.Windows;
using System.Runtime.InteropServices;
using System.Windows.Shapes;
using WebScraperWPF.Behaviors;

namespace WebScraperWPF.ViewModel
{
    public class ToDownloadListViewModel : INotifyPropertyChanged
    {
        SearchImageModel searchModel;
        ImageProcessModel imageProcessModel;
        public event PropertyChangedEventHandler PropertyChanged;
        public ToDownloadListViewModel()
        {
            searchModel = new SearchImageModel(Environment.CurrentDirectory + "/cache");
            searchModel.CollectionChanged += OnCollectionChanged;
            imageProcessModel = new ImageProcessModel(Environment.CurrentDirectory + "/processcache");
            imageProcessModel.OnProcessedImageChange += () => { OnPropertyChanged(new PropertyChangedEventArgs(nameof(ProcessedImage))); };
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Console.WriteLine(sender.ToString());
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(SearchResults)));
        }

        public string PhraseToSearch {get; set;}
        public ICommand SearchPhrase
        {
            get
            {
                return new GenericActionCommand(new Action(() => {
                    searchModel.SearchPhrase(PhraseToSearch);
                }));
            }
        }

        public ICommand SelectImage
        {
            get
            {
                return new GenericActionCommand<ImageSearchResult>(new Action<ImageSearchResult>((param) =>
                {
                    imageProcessModel.Add(param);
                }));
            }
        }
        public BitmapSource ProcessedImage
        {
            get
            {
                if (imageProcessModel.Current.Key != null)
                {
                    if (imageProcessModel.Current.Value == null)
                        return new BitmapImage(new Uri(imageProcessModel.Current.Key.ImagePathUri));
                    else
                        return imageProcessModel.Current.Value.ToBitmapSource();
                }
                return null;
            }
        }
        public ICommand RotateRight
        {
            get
            {
                return new GenericActionCommand(new Action(() =>
                {

                    imageProcessModel.Next();
                }));
            }
        }
        public ICommand RotateLeft
        {
            get
            {
                return new GenericActionCommand(new Action(() =>
                {
                    imageProcessModel.Previous();
                }));
            }
        }
        public ObservableCollection<ImageSearchResult> SearchResults
        {
            get
            {
                return searchModel.SearchResults;
            }
            private set
            {
                searchModel.SearchResults = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(SearchResults)));
            }
        }
         
        protected void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, args);
        }

        public Selection Selection
        {
            set;  get;
        }

        public ICommand Resize
        {
            get
            {
                return new GenericActionCommand(new Action(() =>
                {
                    if (Selection != null)
                        Console.WriteLine($"Left: {Selection.Left} Top: {Selection.Top} Width: {Selection.Width} Height: {Selection.Height}");
                }));
            }
        }
    }

}
