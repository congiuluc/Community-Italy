using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommunityItaly.Web.Pages.Communities
{
	public partial class CommunityReadOnly : ComponentBase
	{
		[Parameter]
		public string Name { get; set; }
	}
}
