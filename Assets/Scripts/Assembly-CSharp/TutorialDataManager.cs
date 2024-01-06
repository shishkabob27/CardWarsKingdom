using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class TutorialDataManager : DataManager<TutorialState>
{
	public class TutorialBlock
	{
		public string ID;

		public string StartState;

		public bool Completed;

		public List<string>[] CardOverrides = new List<string>[6];

		public List<string>[] EnemyCardOverrides = new List<string>[6];

		public TutorialBlock(TutorialState startState)
		{
			StartState = startState.ID;
			ID = startState.Block;
		}
	}

	private static TutorialDataManager _instance;

	private Dictionary<string, TutorialBlock> mBlocks = new Dictionary<string, TutorialBlock>();

	public static TutorialDataManager Instance
	{
		get
		{
			if (_instance == null)
			{
				string path = Path.Combine(SQSettings.CDN_URL, "Blueprints", "db_Tutorials.json");
				_instance = new TutorialDataManager(path);
			}
			return _instance;
		}
	}

	public TutorialDataManager(string path)
	{
		base.FilePath = path;
	}

	public void AddBlock(TutorialState startState)
	{
		if (!mBlocks.ContainsKey(startState.Block))
		{
			mBlocks.Add(startState.Block, new TutorialBlock(startState));
		}
	}

	public TutorialBlock GetBlock(string blockId)
	{
		TutorialBlock value;
		if (!mBlocks.TryGetValue(blockId, out value))
		{
			return null;
		}
		return value;
	}

	public void ResetBlockCompletion()
	{
		foreach (KeyValuePair<string, TutorialBlock> mBlock in mBlocks)
		{
			mBlock.Value.Completed = false;
		}
	}

	public void DebugCompleteAllBlocksExcept(List<string> blockIds)
	{
		foreach (KeyValuePair<string, TutorialBlock> mBlock in mBlocks)
		{
			mBlock.Value.Completed = !blockIds.Contains(mBlock.Key);
		}
	}

	public string Serialize()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[");
		foreach (KeyValuePair<string, TutorialBlock> mBlock in mBlocks)
		{
			TutorialBlock value = mBlock.Value;
			if (value.Completed)
			{
				stringBuilder.Append("\"" + value.ID + "\"");
				stringBuilder.Append(',');
			}
		}
		stringBuilder.Append("]");
		return stringBuilder.ToString();
	}

	public void Deserialize(object[] tutorialArray)
	{
		ResetBlockCompletion();
		foreach (object obj in tutorialArray)
		{
			string blockId = Convert.ToString(obj);
			TutorialBlock block = GetBlock(blockId);
			if (block != null)
			{
				block.Completed = true;
			}
		}
	}

	public void DebugCompleteAllTutorials(bool ftueOnly)
	{
		foreach (KeyValuePair<string, TutorialBlock> mBlock in mBlocks)
		{
			if (ftueOnly)
			{
				TutorialState data = GetData(mBlock.Value.StartState);
				if (data.Trigger != "FTUE")
				{
					continue;
				}
			}
			mBlock.Value.Completed = true;
		}
	}
}
