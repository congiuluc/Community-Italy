using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CommunityItaly.EF.Entities
{
    public class Community
    {
        public Community(string name)
        {
            this.Name = name;
        }
        public string Name { get; }
        public Uri Logo { get; private set; }
        public Uri WebSite { get; private set; }
        public HashSet<Person> Managers { get; } = new HashSet<Person>();
        public void AddManager(Person manager)
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
    }
}
