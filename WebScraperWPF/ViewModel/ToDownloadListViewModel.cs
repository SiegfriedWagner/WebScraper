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

namespace WebScraperWPF.ViewModel
{
    public class ToDownloadListViewModel : INotifyPropertyChanged
    {
        Model.Model model;
        public event PropertyChangedEventHandler PropertyChanged;

        public ToDownloadListViewModel()
        {
            model = new Model.Model(Environment.CurrentDirectory + "/cache");
            model.CollectionChanged += OnCollectionChanged;
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
                    model.SearchPhrase(PhraseToSearch);
                }));
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
