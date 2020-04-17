using CommunityItaly.Shared.ViewModels;
using CommunityItaly.Web.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommunityItaly.Web.Components.Communities
{
	public partial class CommunitySelect : ComponentBase
	{
		[Inject]
		private IHttpServices Http { get; set; }

		[Parameter]
		public string ShortName { get; set; }

		public IReadOnlyList<CommunityUpdateViewModel> CommunitiesToSelect { get; set; } = new List<CommunityUpdateViewModel>();
		private CommunityUpdateViewModel CommunitySelected { get; set; }

		protected override async Task OnInitializedAsync()
		{
			CommunitiesToSelect = (IReadOnlyList<CommunityUpdateViewModel>)await Http.GetCommunitySelect().ConfigureAwait(false);
			if (!string.IsNullOrEmpty(ShortName))
			{
				CommunitySelected = CommunitiesToSelect.FirstOrDefault(x => x.ShortName == ShortName);
			}
		}

		public class Car
		{
			public string Name { get; set; }
			public double Price { get; set; }

			public Car(string name, double price)
			{
				Name = name;
				Price = price;
			}
		}

		Car value = null;

		Car[] options2 = new[]
		{
				new Car("Volkswagen Golf", 10000),
				new Car("Volkswagen Passat", 11000),
				new Car("Volkswagen Polo", 12000),
				new Car("Ford Focus", 13000),
				new Car("Ford Fiesta", 14000),
				new Car("Ford Fusion", 15000),
				new Car("Ford Mondeo", 16000),
			};
	}
}
