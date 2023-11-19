using System.Collections.Generic;
using MiniJSON;

public class LeaderItem
{
	public LeaderData Form;

	public LeaderData SelectedSkin;

	public List<LeaderData> OwnedSkins = new List<LeaderData>();

	public LeaderItem(string leaderID)
	{
		Form = LeaderDataManager.Instance.GetData(leaderID);
		SelectedSkin = Form;
	}

	public LeaderItem(LeaderData leader)
	{
		Form = leader;
		SelectedSkin = Form;
	}

	public LeaderItem(Dictionary<string, object> dict)
	{
		Deserialize(dict);
	}

	public string Serialize()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("ID", Form.ID);
		dictionary.Add("Skin", SelectedSkin.ID);
		List<string> list = new List<string>(OwnedSkins.Count);
		foreach (LeaderData ownedSkin in OwnedSkins)
		{
			list.Add(ownedSkin.ID);
		}
		dictionary.Add("OwnedSkins", list);
		return Json.Serialize(dictionary);
	}

	private void Deserialize(Dictionary<string, object> dict)
	{
		Form = LeaderDataManager.Instance.GetData((string)dict["ID"]);
		if (dict.ContainsKey("OwnedSkins"))
		{
			string[] array = dict["OwnedSkins"] as string[];
			if (array != null)
			{
				string[] array2 = array;
				foreach (object obj in array2)
				{
					LeaderData data = LeaderDataManager.Instance.GetData(obj as string);
					if (data != null)
					{
						OwnedSkins.Add(data);
					}
				}
			}
		}
		string text = TFUtils.LoadString(dict, "Skin", null);
		if (text != null)
		{
			SelectedSkin = LeaderDataManager.Instance.GetData(text);
			if (!OwnedSkins.Contains(SelectedSkin))
			{
				SelectedSkin = null;
			}
		}
		if (SelectedSkin == null)
		{
			SelectedSkin = Form;
		}
	}

	public LeaderData GetNextSkin(LeaderData currentSkin, int direction)
	{
		if (Form.AlternateSkins.Count == 0)
		{
			return null;
		}
		if (currentSkin == Form)
		{
			if (direction == 1)
			{
				return Form.AlternateSkins[0];
			}
			return Form.AlternateSkins[Form.AlternateSkins.Count - 1];
		}
		int num = Form.AlternateSkins.IndexOf(currentSkin);
		if (num == -1)
		{
			return null;
		}
		int num2 = num + direction;
		if (num2 < 0 || num2 >= Form.AlternateSkins.Count)
		{
			return Form;
		}
		return Form.AlternateSkins[num2];
	}

	public bool IsSkinOwned(LeaderData skin)
	{
		return skin == Form || OwnedSkins.Contains(skin);
	}
}
