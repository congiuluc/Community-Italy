using System;
using System.Collections.Generic;

namespace CommunityItaly.Shared.ViewModels
{
	public class CommunityViewModel
	{
		public string Name { get; set; }
		public Uri WebSite { get; set; }
		public IEnumerable<PersonUpdateViewModel> Managers { get; set; } = new List<PersonUpdateViewModel>();
	}

	public class CommunityUpdateViewModel : CommunityViewModel
	{
		public string Id { get; set; }
		public bool Confirmed { get; set; }
		public Uri Logo { get; set; }

		public static CommunityUpdateViewModel Create(CommunityViewModel vm)
		{
			return new CommunityUpdateViewModel
			{
				Name = vm.Name,
				WebSite = vm.WebSite,
				Managers = vm.Managers
			};
		}
	}
}
