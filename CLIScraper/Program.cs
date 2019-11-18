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
        [Option(shortName: 'n', longName: "name", Required = true, HelpText = "Number of images to download")]
        public ushort NumberOfImages { get; set; }
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
                var task = pageLoader.LoadPage($@"https://www.google.com/search?q={o.SearchPhrase}&source=lnms&tbm=isch");
                task.Wait();
                var imageUrlGenerator = GoogleImageDOMParser.GetUrls(task.Result);
                var imageSearchResults = imageUrlGenerator.Take(o.NumberOfImages).ToList();
                var a = Parallel.ForEach(imageSearchResults, (imageResult) =>
                {
                    Stopwatch watch = new Stopwatch();
                    watch.Start();
                    Console.WriteLine($"Started downloading {imageResult.Name} from [{imageResult.ImageUrl}]");
                    WebClient webClient = new WebClient();
                    webClient.DownloadFile(imageResult.ImageUrl, Path.Combine(o.ResultDirectory, $"{imageResult.Name}.{imageResult.FileExtension}"));
                    watch.Stop();
                    Console.WriteLine($"Finished downloading {imageResult.Name} in {watch.ElapsedMilliseconds}ms");
                });
            });
            Console.WriteLine("All images were downloaded succesfully");
            Console.ReadKey();

        }
    }
}