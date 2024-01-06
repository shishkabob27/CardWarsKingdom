using System;
using System.Collections.Generic;
using System.IO;

public class XPTableDataManager : DataManager<XPTableData>
{
	private static XPTableDataManager _instance;

	public static XPTableDataManager Instance
	{
		get
		{
			if (_instance == null)
			{
				string path = Path.Combine(SQSettings.CDN_URL, "Blueprints", "db_XPTables.json");
				_instance = new XPTableDataManager(path);
			}
			return _instance;
		}
	}

	public XPTableDataManager(string path)
	{
		base.FilePath = path;
	}

	protected override void ParseRows(List<object> jlist)
	{
		if (jlist.Count == 0)
		{
			return;
		}
		List<XPTableData> list = new List<XPTableData>();
		Dictionary<string, object> dictionary = (Dictionary<string, object>)jlist[0];
		foreach (KeyValuePair<string, object> item in dictionary)
		{
			if (!(item.Key == "Level"))
			{
				list.Add(new XPTableData(item.Key));
			}
		}
		foreach (object item2 in jlist)
		{
			int num = 0;
			int count = list.Count;
			Dictionary<string, object> dictionary2 = (Dictionary<string, object>)item2;
			foreach (KeyValuePair<string, object> item3 in dictionary2)
			{
				if (item3.Key == "Level")
				{
					continue;
				}
				if ((string)item3.Value == string.Empty)
				{
					num++;
					continue;
				}
				for (int i = num; i < count && !(list[i].ID == item3.Key); i++)
				{
					num++;
				}
				int xpToReach = Convert.ToInt32(item3.Value);
				list[num].AddLevel(xpToReach);
				num++;
			}
		}
		foreach (XPTableData item4 in list)
		{
			Database.Add(item4.ID, item4);
			DatabaseArray.Add(item4);
		}
	}
}
