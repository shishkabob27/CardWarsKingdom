using System;
using System.Collections.Generic;

public class EventTemplateData : ILoadableData
{
	public const int numCustomFields = 6;

	public string ID { get; private set; }

	public string Prefab { get; private set; }

	public string BackgroundImage { get; private set; }

	public string TitleText { get; private set; }

	public string TitleTextColor { get; private set; }

	public string TitleTextOutlineColor { get; private set; }

	public string TitleTextWidth { get; private set; }

	public string MainText { get; private set; }

	public string MainTextColor { get; private set; }

	public string MainTextOutlineColor { get; private set; }

	public string MainTextWidth { get; private set; }

	public string SubText { get; private set; }

	public string SubTextColor { get; private set; }

	public string SubTextOutlineColor { get; private set; }

	public string SubTextWidth { get; private set; }

	public Dictionary<string, string> TemplateCustomData { get; private set; }

	public void Populate(Dictionary<string, object> dict)
	{
		ID = TFUtils.LoadString(dict, "ID", string.Empty);
		Prefab = TFUtils.LoadString(dict, "Prefab", string.Empty);
		BackgroundImage = TFUtils.LoadString(dict, "Background_Image", string.Empty);
		TitleText = TFUtils.LoadString(dict, "Title_Text", string.Empty);
		TitleTextColor = TFUtils.LoadString(dict, "Title_Text_Color", string.Empty);
		TitleTextOutlineColor = TFUtils.LoadString(dict, "Title_Text_Outline_Color", string.Empty);
		TitleTextWidth = TFUtils.LoadString(dict, "Title_Text_Width", string.Empty);
		MainText = TFUtils.LoadString(dict, "Main_Text", string.Empty);
		MainTextColor = TFUtils.LoadString(dict, "Main_Text_Color", string.Empty);
		MainTextOutlineColor = TFUtils.LoadString(dict, "Main_Text_Outline_Color", string.Empty);
		MainTextWidth = TFUtils.LoadString(dict, "Main_Text_Width", string.Empty);
		SubText = TFUtils.LoadString(dict, "Sub_Text", string.Empty);
		SubTextColor = TFUtils.LoadString(dict, "Sub_Text_Color", string.Empty);
		SubTextOutlineColor = TFUtils.LoadString(dict, "Sub_Text_Outline_Color", string.Empty);
		SubTextWidth = TFUtils.LoadString(dict, "Sub_Text_Width", string.Empty);
		TemplateCustomData = new Dictionary<string, string>();
		for (int i = 1; i <= 6; i++)
		{
			string text = TFUtils.LoadString(dict, "Template_Data_Type_" + i, string.Empty);
			if (text.Length == 0)
			{
				break;
			}
			string value = TFUtils.LoadString(dict, "Template_Data_Value_" + i, string.Empty);
			TemplateCustomData.Add(text, value);
		}
	}

	public int GetIntEventTemplateId()
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

	public int GetIntTitleTextWidth()
	{
		//Discarded unreachable code: IL_0011, IL_001e
		try
		{
			return Convert.ToInt32(TitleTextWidth);
		}
		catch
		{
			return -1;
		}
	}

	public int GetIntMainTextWidth()
	{
		//Discarded unreachable code: IL_0011, IL_001e
		try
		{
			return Convert.ToInt32(MainTextWidth);
		}
		catch
		{
			return -1;
		}
	}

	public int GetIntSubTextWidth()
	{
		//Discarded unreachable code: IL_0011, IL_001e
		try
		{
			return Convert.ToInt32(SubTextWidth);
		}
		catch
		{
			return -1;
		}
	}
}
