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
using AngleSharp.Html.Dom;
using System.Text.RegularExpressions;

namespace CLIScraper.WebPageParsers
{
    public class GoogleImageDOMParser
    {
        PageLoader PageLoader { get; set; }
        //public GoogleImageDOMParser(PageLoader pageLoader) { PageLoader = pageLoader; }
        public readonly static string[] ValidImageExtensions = new string[] { "jpg", "jpeg", "png", "bmp", "gif" };
        public static IEnumerable<Func<string, Task<ImageSearchResult>>> GetUrls(string html_document)
        {
            var browsingContext = BrowsingContext.New(Configuration.Default);
            var task = browsingContext.OpenAsync(req => req.Content(html_document));
            task.Wait();
            var document = task.Result;
            //var test = document.QuerySelectorAll("div.rg_meta");
            // var test = document.QuerySelectorAll("img.rg_i");
            var test = document.QuerySelectorAll("a.rg_l");
            foreach (var element in test)
            {
                IHtmlImageElement htmlimage = element as IHtmlImageElement;
                if (htmlimage != null && htmlimage.Source.StartsWith("https"))
                {
                    Debug.WriteLine("Element is url");
                }
                else if (htmlimage != null && htmlimage.Source != "")
                {
                    yield return new Func<string, Task<ImageSearchResult>>((path) => 
                    {
                        return new Task<ImageSearchResult>(() => 
                        {
                            return ImageSearchResult.FromBase64(path, htmlimage.Source); 
                        });
                    });
                }
                else if (htmlimage != null)
                {
                    Regex re = new Regex("data-src=\".*?\"");
                    var match = re.Match(htmlimage.OuterHtml);
                    if (match.Success)
                    {
                        var url = match.Value.Substring("data-src=\"".Length, match.Value.Length - "data-src=\"".Length - 1);
                        yield return new Func<string, Task<ImageSearchResult>>((path) =>
                        {
                            return new Task<ImageSearchResult>(() =>
                            {
                                return ImageSearchResult.FromWebUrl(path, url);
                            });
                        });
                    }
                    else
                        throw new Exception("Unhandled data source");
                }
                IHtmlAnchorElement htmlanchor = element as IHtmlAnchorElement;
                if (htmlanchor != null)
                {
                    var imglink = htmlanchor.Href.Replace(@"%3A", ":").Replace(@"%2F", @"/");
                    var match = Regex.Match(imglink, @"imgurl.*?&");
                    if (match.Success)
                    {
                        imglink = match.Value.Replace(@"imgurl=", "").Replace("&", "");
                        yield return new Func<string, Task<ImageSearchResult>>((path) =>
                        {
                            return new Task<ImageSearchResult>(() =>
                            {
                                return ImageSearchResult.FromWebUrl(path, imglink);
                            });
                        });

                    }
                    var a = 10;
                }
                else
                {
                    Debug.WriteLine("Element is not a HTML image");
                }
            }
        }
    }
}
