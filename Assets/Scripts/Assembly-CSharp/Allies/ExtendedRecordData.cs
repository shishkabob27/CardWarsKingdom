using System;
using System.Collections.Generic;

namespace Allies
{
	public class ExtendedRecordData
	{
		public DateTime datetime;

		public ExtendedRecordData(Dictionary<string, object> dict)
		{
			datetime = DateTime.Parse(dict["date"].ToString());
		}

		public static List<ExtendedRecordData> ProcessList(List<object> data)
		{
			List<ExtendedRecordData> list = new List<ExtendedRecordData>(data.Count);
			foreach (object datum in data)
			{
				Dictionary<string, object> dict = (Dictionary<string, object>)datum;
				list.Add(new ExtendedRecordData(dict));
			}
			return list;
		}
	}
}
