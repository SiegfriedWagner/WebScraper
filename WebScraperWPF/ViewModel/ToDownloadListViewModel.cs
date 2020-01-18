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
            imageProcessModel = new ImageProcessModel(); 
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
                return new GenericActionCommand<CachedImageSearchResult>(new Action<CachedImageSearchResult>((param) =>
                {
                    if (!imageProcessModel.ImagesToProcess.Contains(param))
                        imageProcessModel.ImagesToProcess.Add(param);
                }));
            }
        }
        public ObservableCollection<CachedImageSearchResult> SearchResults
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
    }

}
