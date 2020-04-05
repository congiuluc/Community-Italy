using System;

namespace CommunityItaly.Shared.ViewModels
{
	public class PersonBaseViewModel
	{        
        public string Name { get; set; }
        public string Surname { get; set; }
        public string MVP_Code { get; set; }
    }

    public class PersonViewModel : PersonBaseViewModel
    {
        public byte[] Picture { get; set; }
    }

    public class PersonUpdateViewModel : PersonBaseViewModel
    {
        public string Id { get; set; }
        public bool Confirmed { get; set; }
        public Uri Picture { get; set; }

        public static PersonUpdateViewModel Create(PersonViewModel vm)
        {
            return new PersonUpdateViewModel
            {
                Name = vm.Name,
                Surname = vm.Surname,
                MVP_Code = vm.MVP_Code
            };
        }
    }
}
