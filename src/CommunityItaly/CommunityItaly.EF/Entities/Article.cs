using System;
using System.Collections.Generic;
using System.Text;

namespace CommunityItaly.EF.Entities
{
    public class Article
    {
        public Article()
        {
            Id = Guid.NewGuid().ToString("N");
        }

        public string Id { get; }

        public HashSet<Person> Authors { get; }
    }
}
