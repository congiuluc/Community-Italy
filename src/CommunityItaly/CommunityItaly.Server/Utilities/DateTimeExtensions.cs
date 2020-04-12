using System;

namespace CommunityItaly.Server
{
	public static class DateTimeExtensions
	{
		public static DateTime StartOfMonth(this DateTime date)
		{
			date = date.AddDays(-date.Day + 1).StartOfDay();
			return date;
		}

		public static DateTime EndOfMonth(this DateTime date)
		{
			date = date.AddDays(-date.Day).AddMonths(1).EndOfDay();
			return date;
		}

		public static DateTime StartOfDay(this DateTime date)
		{
			return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0, date.Kind);
		}

		public static DateTime EndOfDay(this DateTime date)
		{
			return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, 59, date.Kind);
		}
	}
}
