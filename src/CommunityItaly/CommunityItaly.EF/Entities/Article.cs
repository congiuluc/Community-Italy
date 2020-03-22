using System;
using System.Collections.Generic;
using System.Text;

namespace CommunityItaly.EF.Entities
{
    public class Article
    {
        public Article(Uri url, DateTime publishDate)
        {
            Id = Guid.NewGuid().ToString("N");
            Url = url;
            PublishDate = publishDate;
        }

        public string Id { get; }
        public Uri Url { get;  }
        public DateTime PublishDate { get; }
        public HashSet<PersonOwned> Authors { get; }
    }
}
