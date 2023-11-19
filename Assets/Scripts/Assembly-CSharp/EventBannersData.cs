using System;
using System.Collections.Generic;

public class EventBannersData : ILoadableData
{
	public string ID { get; private set; }

	public string EventID { get; private set; }

	public string TemplateID { get; private set; }

	public string StartDate { get; private set; }

	public string EndDate { get; private set; }

	public string SpecificDayRepeats { get; private set; }

	public void Populate(Dictionary<string, object> dict)
	{
		ID = TFUtils.LoadString(dict, "ID", string.Empty);
		EventID = TFUtils.LoadString(dict, "Event_ID", string.Empty);
		TemplateID = TFUtils.LoadString(dict, "Template_ID", string.Empty);
		StartDate = TFUtils.LoadString(dict, "Start_Date", string.Empty);
		EndDate = TFUtils.LoadString(dict, "End_Date", string.Empty);
		SpecificDayRepeats = TFUtils.LoadString(dict, "Specifc_Day_Repeats", string.Empty);
	}

	public int GetIntEventBannerId()
	{
		//Discarded unreachable code: IL_0011, IL_001e
		try
		{
			return Convert.ToInt32(ID);
		}
		catch
		{
			return -1;
		}
	}

	public int GetIntEventId()
	{
		//Discarded unreachable code: IL_0011, IL_001e
		try
		{
			return Convert.ToInt32(EventID);
		}
		catch
		{
			return -1;
		}
	}

	public int GetIntEventTemplateId()
	{
		//Discarded unreachable code: IL_0011, IL_001e
		try
		{
			return Convert.ToInt32(TemplateID);
		}
		catch
		{
			return -1;
		}
	}
}
