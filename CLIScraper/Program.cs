using CLIScraper.WebPageParsers;
using CommandLine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
namespace CLIScraper
{
    class Options
    {
        [Option(shortName: 's', longName: "search", Required = true, HelpText = "Search phrase.")]
        public string SearchPhrase { get; set; }
        [Option(shortName: 'o', longName: "output_dir", Required = true, HelpText = "Output directory")]
        public string ResultDirectory { get; set; }
        [Option(shortName: 'n', longName: "number", Required = true, HelpText = "Number of images to download")]
        public ushort NumberOfImages { get; set; }
        [Option(shortName: 'a', longName:"attempts", Required = false, Default = (ushort) 5, HelpText = "Number of attempts of getting search result")]
        public ushort Attempts { get; set; }
    }

    class Program
    {
        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args).WithParsed<Options>(o =>
            {
                if (o.ResultDirectory == "pwd")
                    o.ResultDirectory = Directory.GetCurrentDirectory();
                if (!Directory.Exists(o.ResultDirectory))
                    Directory.CreateDirectory(o.ResultDirectory);
                PageLoader pageLoader = PageLoader.GetPageLoader();
                List<ImageSearchResult> imageSearchResults = null;
                for (int i = 0; i < o.Attempts; i++)
                {
                    var task = pageLoader.LoadPageAsync($@"https://www.google.com/search?q={o.SearchPhrase}&source=lnms&tbm=isch");
                    task.Wait();
                    var imageUrlGenerator = GoogleImageDOMParser.GetUrls(task.Result);
                    imageSearchResults = imageUrlGenerator.Take(o.NumberOfImages).ToList();
                    if (imageSearchResults.Count > 0)
                        break;
                }
                if (imageSearchResults == null)
                    throw new InvalidDataException($"After {o.Attempts} attempts no search results was found!");
                var a = Parallel.ForEach(imageSearchResults, (imageResult) =>
                {
                    Stopwatch watch = new Stopwatch();
                    watch.Start();
                    Console.WriteLine($"Started downloading {imageResult.Name} from [{imageResult.ImageWebUrl}]");
                    WebClient webClient = new WebClient();
                    webClient.DownloadFile(imageResult.ImageWebUrl, Path.Combine(o.ResultDirectory, $"{imageResult.Name}.{imageResult.FileExtension}"));
                    watch.Stop();
                    Console.WriteLine($"Finished downloading {imageResult.Name} in {watch.ElapsedMilliseconds}ms");
                });
                Console.WriteLine("All images were downloaded succesfully");
                Console.ReadKey();
            });
           

        }
    }
}