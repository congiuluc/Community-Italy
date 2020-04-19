using System;

namespace CommunityItaly.EF.Entities
{
    public class Person
    {
        public Person(string id, string name, string surname)
        {
            Name = name;
            Surname = surname;
            Id = id;
        }
        public string Id { get; private set; }
        public string Name { get; }
        public string Surname { get; }
        public Uri Picture { get; private set; }
        public bool Confirmed { get; private set; }

        public string MVP_Code { get; private set; }
        public bool IsMVP => !string.IsNullOrEmpty(MVP_Code);
        public void SetMVPCode(string mvp_code) 
        {
            if (!string.IsNullOrEmpty(mvp_code))
                MVP_Code = mvp_code; 
        }

        public void SetPicture(Uri picture) => Picture = picture;
        public void SetConfirmation(bool confirmation) => Confirmed = confirmation;

        public PersonOwned ToOwned()
        {
            var p = new PersonOwned(Id, Name, Surname);
            p.SetPicture(Picture);
            p.SetMVPCode(MVP_Code);
            p.SetConfirmation(Confirmed);
            return p;
        }
    }

    public class PersonOwned : Person
    {
        public PersonOwned(string id, string name, string surname) : base(id, name, surname)
        {
        }
    }
}
