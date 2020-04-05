using System;
using System.Collections.Generic;

namespace CommunityItaly.EF.Entities
{
	public class Article
    {
        public Article(Uri url, DateTime publishDate, string name)
        {
            Id = Guid.NewGuid().ToString("N");
            Url = url;
            Name = name;
            PublishDate = publishDate;
        }

        public string Id { get; }
        public string Name { get; }
        public Uri Url { get;  }
        public DateTime PublishDate { get; }
        public HashSet<PersonOwned> Authors { get; }
        public bool Confirmed { get; private set; }
        public void SetConfirmation(bool confirmation) => Confirmed = confirmation;
        public void AddAuthor(PersonOwned author)
        {
            Authors.Add(author);
        }
        public void RemoveAuthor(string id)
        {
            Authors.RemoveWhere(x => x.Id == id);
        }
    }
}
