using CefSharp;
using CefSharp.OffScreen;
using CLIScraper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CLIScraper
{
    public static class ChromiumBrowserExtension
    {
        public static async Task AwaitInitialization(this ChromiumWebBrowser chromiumWebBrowser)
        {
            if (chromiumWebBrowser.IsBrowserInitialized)
                return;
            var awaiter = new ChromiumWebBrowserInitializationAwaiter()
            {
                browser = chromiumWebBrowser
            };
            await awaiter;
            return;
        }

        public static async Task AwaitPageLoad(this ChromiumWebBrowser chromiumWebBrowser)
        {
            if (!chromiumWebBrowser.IsLoading)
                return;
            var awaiter = new ChromiumWebBrowserLoadingAwaiter()
            {
                browser = chromiumWebBrowser
            };
            await awaiter;
            return;
        }
    }
    public class ChromiumWebBrowserInitializationAwaiter : INotifyCompletion
    {
        internal ChromiumWebBrowser browser { get; set; }
        public ChromiumWebBrowserInitializationAwaiter GetAwaiter()
        {
            return this;
        }
        public bool IsCompleted
        {
           get { return browser.IsBrowserInitialized; }
        }
        public void OnCompleted(Action continuation)
        {
            EventHandler h = null;
            h = (o, e) =>
            {
                browser.BrowserInitialized -= h;
                continuation();
            };
            browser.BrowserInitialized += h;
        }
        public bool GetResult()
        {
            return browser.IsBrowserInitialized;
        }

    }
    public class ChromiumWebBrowserLoadingAwaiter : INotifyCompletion
    {
        internal ChromiumWebBrowser browser { get; set; }
        public ChromiumWebBrowserLoadingAwaiter GetAwaiter()
        {
            return this;
        }
        public bool IsCompleted
        {
            get { return !browser.IsLoading; }
        }
        public void OnCompleted(Action continuation)
        {
            EventHandler<LoadingStateChangedEventArgs> h = null;
            h = (o, e) =>
            {
                if (!e.IsLoading)
                {
                    browser.LoadingStateChanged -= h;
                    continuation();
                }
            };
            browser.LoadingStateChanged += h;
        }
        public bool GetResult()
        {
            return !browser.IsLoading;
        }

    }
}