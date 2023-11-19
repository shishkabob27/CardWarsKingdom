using System.Collections.Generic;

public class TutorialBoardData : ILoadableData
{
	private string _ID;

	private List<TutorialBoardEntry> _boardEntries = new List<TutorialBoardEntry>();

	public string ID
	{
		get
		{
			return _ID;
		}
	}

	public IList<TutorialBoardEntry> Entries
	{
		get
		{
			return _boardEntries.AsReadOnly();
		}
	}

	public TutorialBoardData(string boardID = null)
	{
		_ID = boardID;
	}

	public void Populate(Dictionary<string, object> dict)
	{
	}

	public void AddEntry(TutorialBoardEntry entry)
	{
		_boardEntries.Add(entry);
	}
}
