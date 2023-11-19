using System.Collections.Generic;
using System.IO;

public class TutorialBoardDataManager : DataManager<TutorialBoardData>
{
	private static TutorialBoardDataManager _instance;

	public static TutorialBoardDataManager Instance
	{
		get
		{
			if (_instance == null)
			{
				string path = Path.Combine("Blueprints", "db_TutorialBoard.json");
				_instance = new TutorialBoardDataManager(path);
			}
			return _instance;
		}
	}

	public TutorialBoardDataManager(string path)
	{
		base.FilePath = path;
	}

	protected override void ParseRows(List<object> jlist)
	{
		TutorialBoardData tutorialBoardData = null;
		foreach (object item in jlist)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)item;
			string text = TFUtils.LoadString(dictionary, "boardID", string.Empty);
			if (tutorialBoardData == null || text != "^")
			{
				tutorialBoardData = new TutorialBoardData(text);
				if (!Database.ContainsKey(text))
				{
					Database.Add(text, tutorialBoardData);
				}
				DatabaseArray.Add(tutorialBoardData);
			}
			string text2 = TFUtils.LoadString(dictionary, "CreatureID", string.Empty);
			if (text2 == string.Empty)
			{
				tutorialBoardData.AddEntry(null);
				continue;
			}
			TutorialBoardEntry entry = new TutorialBoardEntry(dictionary);
			tutorialBoardData.AddEntry(entry);
		}
	}
}
