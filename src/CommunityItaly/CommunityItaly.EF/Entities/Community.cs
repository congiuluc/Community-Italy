using System;
using System.Collections.Generic;

namespace CommunityItaly.EF.Entities
{
    public class Community
    {
        public Community(string name)
        {
            this.Name = name;
        }
        public string Name { get; }
        public byte[] Logo { get; private set; }
        public Uri WebSite { get; private set; }
        public HashSet<Person> Managers { get; } = new HashSet<Person>();
        public void AddManager(Person manager)
        {
            Managers.Add(manager);
        }
        public void AddLogo(byte[] logo)
        {
            Logo = logo;
        }
        public void AddWebSite(Uri website)
        {
            WebSite = website;
        }
    }
}
