using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommunityItaly.Web.Stores
{
	public class Routes
	{
		public static string EventEdit(string id) => $"/EditEvent/{id}";
	}
}
