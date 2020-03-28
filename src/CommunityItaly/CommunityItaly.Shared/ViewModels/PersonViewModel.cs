using System;

namespace CommunityItaly.Shared.ViewModels
{
	public class PersonBaseViewModel
	{
        public string Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string MVP_Code { get; set; }
    }

    public class PersonViewModel : PersonBaseViewModel
    {
        public byte[] Picture { get; set; }
    }

    public class PersonViewModelReadOnly : PersonBaseViewModel
    {
        public Uri Picture { get; set; }
    }
}
