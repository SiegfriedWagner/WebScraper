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
        [Option(shortName: 'o', longName: "output_dir", Required = true, HelpText = "Output directory")]
        public string ResultDirectory { get; set; }
        [Option(shortName: 'n', longName: "number", Required = true, HelpText = "Number of images to download")]
        public ushort NumberOfImages { get; set; }
        [Option(shortName: 'a', longName:"attempts", Required = false, Default = (ushort) 5, HelpText = "Number of attempts of getting search result")]
        public ushort Attempts { get; set; }
        [Value(0, HelpText = "Phrases to search")]
        public IEnumerable<string> SearchPhrases { get; set; }
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
                List<Func<string, Task<ImageSearchResult>>> imageSearchResultFuncs = null;
                foreach (var SearchPhrase in o.SearchPhrases)
                {
                    var OutputDirectory = Path.Combine(o.ResultDirectory, SearchPhrase);
                    Directory.CreateDirectory(OutputDirectory);
                    for (int i = 0; i < o.Attempts; i++)
                    {
                        var task = pageLoader.LoadPageAsync($@"https://www.google.com/search?q={SearchPhrase}&source=lnms&tbm=isch");
                        task.Wait();
                        var imageUrlGenerator = GoogleImageDOMParser.GetUrls(task.Result);
                        imageSearchResultFuncs = imageUrlGenerator.Take(o.NumberOfImages).ToList();
                        if (imageSearchResultFuncs.Count > 0)
                            break;
                    }
                    if (imageSearchResultFuncs == null)
                        throw new InvalidDataException($"After {o.Attempts} attempts no search results was found!");
                    var imageSearchResultTasks = imageSearchResultFuncs
                        .AsParallel()
                        .Select(m => m(OutputDirectory))
                        .ToList();
                    var a = Parallel.ForEach(imageSearchResultTasks, (imageResultTask) =>
                    {
                        Stopwatch watch = new Stopwatch();
                        watch.Start();
                        imageResultTask.Start();
                        imageResultTask.Wait();
                        watch.Stop();
                        Console.WriteLine($"Finished downloading {imageResultTask.Result.Name} in {watch.ElapsedMilliseconds}ms");
                    });
                    
                }
                Console.WriteLine("All images were downloaded succesfully");
                Console.ReadKey();
            });
           

        }
    }
}