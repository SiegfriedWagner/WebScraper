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
namespace WebScraperWPF.ViewModel
{
    public class ToDownloadListViewModel : INotifyPropertyChanged
    {
        Model.Model model;
        public event PropertyChangedEventHandler PropertyChanged;
        
        public ToDownloadListViewModel()
        {
            model = new Model.Model(@"D:\git\WebScraper\CLIScraper\cache");
            model.CollectionChanged += OnCollectionChanged;
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Console.WriteLine(sender.ToString());
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(SearchResults)));
        }

        private string searchPhrase = "Default";
        public string SearchPhrase { 
            get { return searchPhrase; } 
            set 
            {   
                searchPhrase = value;
                var task = new Task(() => model.SearchPhrase(value));
                task.Start();
                task.Wait();
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(SearchPhrase)));
            }  
        }
        
        public ObservableCollection<CachedImageSearchResult> SearchResults
        {
            get
            {
                return model.SearchResults;
            }
            private set
            {
                model.SearchResults = value;
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
