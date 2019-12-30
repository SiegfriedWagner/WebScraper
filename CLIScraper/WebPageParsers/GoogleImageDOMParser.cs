using AngleSharp;
using AngleSharp.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Web;
using System.Web.Helpers;
using System.IO;
using System.Diagnostics;

namespace CLIScraper.WebPageParsers
{
    public class GoogleImageDOMParser
    {
        PageLoader PageLoader { get; set; }
        //public GoogleImageDOMParser(PageLoader pageLoader) { PageLoader = pageLoader; }
        public readonly static string[] ValidImageExtensions = new string[] { "jpg", "jpeg", "png", "bmp", "gif" };
        public static IEnumerable<ImageSearchResult> GetUrls(string html_document)
        {
            var browsingContext = BrowsingContext.New(Configuration.Default);
            var task = browsingContext.OpenAsync(req => req.Content(html_document));
            task.Wait();
            var document = task.Result;
            var test = document.QuerySelectorAll("div.rg_meta");
            foreach (var element in test)
            {
                //dynamic json = JsonConvert.DeserializeObject(element.InnerHtml);
                dynamic json = Json.Decode(element.InnerHtml);
                string name = json.pt.ToString();
                foreach(var invalidchar in Path.GetInvalidFileNameChars())
                {
                    name = name.Replace($"{invalidchar}", "");
                }
                var result = new ImageSearchResult(ImageUrl: json.ou.ToString(), Name: name);
                if (ValidImageExtensions.Contains(result.FileExtension))
                    yield return result;
                else
                    Debug.WriteLine($"Unknow file extension of {result.FileExtension}", "INFO");
            }
        }
    }
}
