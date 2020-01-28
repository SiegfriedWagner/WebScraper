using CefSharp;
using CefSharp.OffScreen;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CLIScraper
{
    public class PageLoader
    {
        private static CefSettings cefSettings = null;
        private ChromiumWebBrowser browser;
        private static bool IsInitialized = false;

        private PageLoader()
        {
            browser = new ChromiumWebBrowser();
        }
        public static PageLoader GetPageLoader()
        {
            if (!IsInitialized)
            {
                CefSharpSettings.SubprocessExitIfParentProcessClosed = true;
                cefSettings = new CefSettings()
                {
                    CachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CefSharp\\Cache")
                };
                Cef.Initialize(cefSettings);
                IsInitialized = true;
            }
            return new PageLoader();
        }
        public async Task<string> LoadPageAsync(string url)
        {
            await browser.AwaitInitialization();
            browser.Load(url);
            await browser.AwaitPageLoad();
            return await browser.GetSourceAsync();
        }

    }
}

