﻿using System;
using System.Collections.Generic;

namespace CommunityItaly.Shared.ViewModels
{
	public class CommunityViewModel
	{
		public string Name { get; set; }
		public string WebSite { get; set; }
		public List<PersonUpdateViewModel> Managers { get; set; } = new List<PersonUpdateViewModel>();
	}

	public class CommunityUpdateViewModel : CommunityViewModel
	{
		public string ShortName { get; set; }
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
