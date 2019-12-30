using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLIScraper.WebPageParsers
{
    public class ImageSearchResult
    {
        public string ImageWebUrl { get; protected set; }
        public string Name { get; protected set; }
        public string FileExtension 
        { 
            get 
            {
                return ImageWebUrl.Split('.').LastOrDefault().ToLower();
            } 
        }
        public ImageSearchResult(string ImageUrl, string Name)
        {
            this.Name = Name;
            this.ImageWebUrl = ImageUrl;
        }
    }
}
