using System;
using System.Collections.Generic;

namespace CommunityItaly.EF.Entities
{
    public class CommunityBase<T>
        where T : Person
    {
        public CommunityBase(string name)
        {
            Name = name;
            ShortName = name
                .Replace(" ","-")
                .ToLowerInvariant();
        }
        public string ShortName { get; }
        public string Name { get; set; }
        public Uri Logo { get; private set; }
        public Uri WebSite { get; private set; }
        public HashSet<T> Managers { get; } = new HashSet<T>();
        public void AddManager(T manager)
        {
            Managers.Add(manager);
        }
        public void RemoveManager(string id)
        {
            Managers.RemoveWhere(x => x.Id == id);
        }
        public void SetLogo(Uri logo)
        {
            Logo = logo;
        }
        public void SetWebSite(Uri website)
        {
            WebSite = website;
        }

        public CommunityOwned ToOwned()
        {
            var c = new CommunityOwned(Name);
            c.SetLogo(Logo);
            c.SetWebSite(WebSite);
            foreach (var m in Managers)
            {
                c.AddManager(m.ToOwned());
            }
            return c;
        }
    }


    public class Community : CommunityBase<Person>
    {
        public Community(string shortName)
            : base(shortName)
        {
        }
    }

    public class CommunityOwned : CommunityBase<PersonOwned>
    {
        public CommunityOwned(string shortName)
            : base(shortName)
        {
        }       
    }
}
