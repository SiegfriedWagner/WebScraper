﻿using CLIScraper.WebPageParsers;
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
            imageProcessModel = new ImageProcessModel(Environment.CurrentDirectory + "/processcache"); 
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
        public ImageSearchResult ProcessedImage
        {
            get
            {
                return imageProcessModel.Current;
            }
        }
        public ICommand RotateRight
        {
            get
            {
                return new GenericActionCommand(new Action(() =>
                {
                    var curr = imageProcessModel.Current;
                    var next = imageProcessModel.Next();
                    if (curr != next)
                        OnPropertyChanged(new PropertyChangedEventArgs(nameof(ProcessedImage)));
                }));
            }
        }
        public ICommand RotateLeft
        {
            get
            {
                return new GenericActionCommand(new Action(() =>
                {
                    var curr = imageProcessModel.Current;
                    var next = imageProcessModel.Previous();
                    if (curr != next)
                        OnPropertyChanged(new PropertyChangedEventArgs(nameof(ProcessedImage)));
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
    }

}
