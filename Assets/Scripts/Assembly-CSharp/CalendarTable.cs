using System.Collections.Generic;

public class CalendarTable : ILoadableData
{
	public List<CalendarGift> Entries = new List<CalendarGift>();

	public string ID { get; private set; }

	public void Populate(Dictionary<string, object> dict)
	{
		ID = TFUtils.LoadString(dict, "Calendar", string.Empty);
		AddRow(dict);
	}

	public void AddRow(Dictionary<string, object> dict)
	{
		CalendarGift calendarGift = new CalendarGift();
		Entries.Add(calendarGift);
		calendarGift.Populate(dict);
	}
}
