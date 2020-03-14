using System;

namespace CommunityItaly.EF.Entities
{
    public class Person
    {
        public Person(string name, string surname)
        {
            Name = name;
            Surname = surname;
            Id = Guid.NewGuid().ToString("N");
        }
        public string Id { get; }
        public string Name { get; }
        public string Surname { get; }
        public byte[] Picture { get; private set; }
        public string MVP_Code { get; private set; }
        public bool IsMVP => !string.IsNullOrEmpty(MVP_Code);

        public void AddMVPCode(string mvp_code) => MVP_Code = mvp_code;
        public void AddPicture(byte[] picture) => Picture = picture;
    }
}
