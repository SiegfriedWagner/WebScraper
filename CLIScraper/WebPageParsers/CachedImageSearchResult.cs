using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLIScraper.WebPageParsers
{
    public struct CachedImageSearchResult
    {
        public ImageSearchResult ImageSearchResult    
        {
            get; private set;
        }
        public string ImagePathUri
        {
            get; private set;
        }
        public CachedImageSearchResult(ImageSearchResult imageSearchResult, string imagePathUri)
        {
            ImageSearchResult = imageSearchResult;
            ImagePathUri = imagePathUri;
        }
    }
}
