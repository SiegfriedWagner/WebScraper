using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CLIScraper.WebPageParsers
{
    public class ImageSearchResult
    {

        public string ImagePathUri { get; protected set; }
        public string Name { get; protected set; }
        public string FileExtension 
        {
            protected set; get;
        }
        public ImageSearchResult(string Name, string ImagePathUri, string FileExtension)
        {
            this.Name = Name;
            this.ImagePathUri = ImagePathUri;
            this.FileExtension = FileExtension;
        }
        public static ImageSearchResult FromWebUrl(string OutputDirectory, string WebUrl)
        {
            var name = Guid.NewGuid().ToString();
            WebClient client = new WebClient();
            string outputPath = Path.Combine(OutputDirectory, name);
            client.DownloadFile(WebUrl, outputPath);
            if (File.Exists(outputPath))
                return new ImageSearchResult(name, outputPath, "");
            else
                return null;
        }
        public static ImageSearchResult FromBase64(string OutputDirectory, string SourceData, string Name)
        {
            Regex rx = new Regex(@"^data:image/.*;base64,");
            var match = rx.Match(SourceData);
            string extension = "";
            if (match.Success)
            {
                extension = "." + match.Value.Replace(";base64,", "").Replace("data:image/", "");
                SourceData = SourceData.Substring(match.Length, SourceData.Length - match.Length);
            }
            else
                return null;
            byte[] imageBytes = Convert.FromBase64String(SourceData);
            string outputPath = Path.Combine(OutputDirectory, Name + extension);
            FileStream fileStream = new FileStream(outputPath, FileMode.OpenOrCreate);
            fileStream.Write(imageBytes, 0, imageBytes.Length);
 
            return new ImageSearchResult(Name: Name, ImagePathUri: outputPath, FileExtension: extension);
        }
        public static ImageSearchResult FromBase64(string OutputDirectory, string source)
        {
            var name = Guid.NewGuid().ToString();
            return FromBase64(OutputDirectory, source, name);
                
        }

    }
}
