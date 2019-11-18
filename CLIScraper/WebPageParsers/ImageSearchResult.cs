using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLIScraper.WebPageParsers
{
    public struct ImageSearchResult
    {
        public string ImageUrl { get; private set; }
        public string Name { get; private set; }
        public string FileExtension 
        { 
            get 
            {
                return ImageUrl.Split('.').LastOrDefault().ToLower();
            } 
        }
        public ImageSearchResult(string ImageUrl, string Name)
        {
            this.Name = Name;
            this.ImageUrl = ImageUrl;
        }
    }
}
