using System;

namespace CommunityItaly.Shared.ViewModels
{
	public class PersonViewModel
	{
        public string Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public byte[] Picture { get; set; }
        public string MVP_Code { get; set; }
    }

    public class PersonViewModelReadOnly
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public Uri Picture { get; set; }
        public string MVP_Code { get; set; }
    }
}
