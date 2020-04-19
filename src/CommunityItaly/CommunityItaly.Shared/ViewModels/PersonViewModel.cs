using System;

namespace CommunityItaly.Shared.ViewModels
{

    public class PersonViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string MVP_Code { get; set; }
    }

    public class PersonUpdateViewModel : PersonViewModel
    {
        public bool Confirmed { get; set; }
        public Uri Picture { get; set; }

        public static PersonUpdateViewModel Create(PersonViewModel vm)
        {
            return new PersonUpdateViewModel
            {
                Id = vm.Id,
                Name = vm.Name,
                Surname = vm.Surname,
                MVP_Code = vm.MVP_Code
            };
        }
    }
}
