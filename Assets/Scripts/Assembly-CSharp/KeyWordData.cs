using System.Collections.Generic;
using System.Collections.ObjectModel;

public class KeyWordData : ILoadableData
{
	private string _ID;

	private string _DisplayName;

	private string _Description;

	private string _TextColor;

	private List<CreatureStat> _ShowPlayerStats = new List<CreatureStat>();

	private List<CreatureStat> _ShowOpponentStats = new List<CreatureStat>();

	private string _PopupText;

	public List<KeyWordData> RedundantKeywords = new List<KeyWordData>();

	public string ID
	{
		get
		{
			return _ID;
		}
	}

	public string DisplayName
	{
		get
		{
			return _DisplayName;
		}
	}

	public string Description
	{
		get
		{
			return _Description;
		}
	}

	public string TextColor
	{
		get
		{
			return _TextColor;
		}
	}

	public ReadOnlyCollection<CreatureStat> ShowPlayerStats
	{
		get
		{
			return _ShowPlayerStats.AsReadOnly();
		}
	}

	public ReadOnlyCollection<CreatureStat> ShowOpponentStats
	{
		get
		{
			return _ShowOpponentStats.AsReadOnly();
		}
	}

	public string PopupText
	{
		get
		{
			return _PopupText;
		}
	}

	public void Populate(Dictionary<string, object> dict)
	{
		_ID = TFUtils.LoadString(dict, "Text", string.Empty);
		_DisplayName = TFUtils.LoadLocalizedString(dict, "DisplayName", string.Empty);
		_Description = TFUtils.LoadLocalizedString(dict, "Description", string.Empty);
		_TextColor = TFUtils.LoadString(dict, "Color", "FFFFFF");
		_PopupText = TFUtils.LoadLocalizedString(dict, "PopupText", "Missing PopupText");
		_ShowPlayerStats = KeyWordDataManager.ParseStatsList(TFUtils.LoadString(dict, "ShowPlayerStats", string.Empty));
		_ShowOpponentStats = KeyWordDataManager.ParseStatsList(TFUtils.LoadString(dict, "ShowOpponentStats", string.Empty));
		string text = TFUtils.LoadString(dict, "EventFx", string.Empty);
		if (text != string.Empty)
		{
			GameEventFXData data = GameEventFXDataManager.Instance.GetData(text);
			if (data != null)
			{
				data.Keyword = this;
			}
		}
	}
}
