using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommunityItaly.Web.Stores
{
	public class Routes
	{
		public static string EventEdit(string id) => $"/EventEdit/{id}";
		public static string EventCreate() => $"/EventCreate";
	}
}
