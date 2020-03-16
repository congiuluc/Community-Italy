using System;
using System.Collections.Generic;
using System.Text;

namespace CommunityItaly.Shared.ViewModels
{
	public class PagedViewModel<T>
		where T : class
	{
		public int CurrentPage { get; set; }
		public int Total { get; set; }
		public IEnumerable<T> Entities { get; set; }
	}
}
