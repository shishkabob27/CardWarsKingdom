using System.Collections.Generic;
using System.IO;

public class CalendarGiftDataManager : DataManager<CalendarTable>
{
	private static CalendarGiftDataManager _instance;

	public static CalendarGiftDataManager Instance
	{
		get
		{
			if (_instance == null)
			{
				string path = Path.Combine("Blueprints", "db_CalendarGift.json");
				_instance = new CalendarGiftDataManager(path);
			}
			return _instance;
		}
	}

	public CalendarGiftDataManager(string path)
	{
		base.FilePath = path;
	}

	protected override void ParseRows(List<object> jlist)
	{
		CalendarTable calendarTable = null;
		foreach (object item in jlist)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)item;
			string text = TFUtils.LoadString(dictionary, "Calendar", string.Empty);
			if (calendarTable == null || text != "^")
			{
				calendarTable = new CalendarTable();
				calendarTable.Populate(dictionary);
				Database.Add(calendarTable.ID, calendarTable);
				DatabaseArray.Add(calendarTable);
			}
			else
			{
				calendarTable.AddRow(dictionary);
			}
		}
	}
}
