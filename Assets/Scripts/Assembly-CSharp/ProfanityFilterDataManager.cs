using System.Collections.Generic;
using System.IO;
using System.Text;

public class ProfanityFilterDataManager : DataManager<ProfanityFilterData>
{
	private static ProfanityFilterDataManager _instance;

	private List<string> profanityFilter;

	private string[] profanityReplacements;

	public static ProfanityFilterDataManager Instance
	{
		get
		{
			if (_instance == null)
			{
				string filePath = Path.Combine("Blueprints", "db_Profanity.json");
				_instance = new ProfanityFilterDataManager(filePath);
			}
			return _instance;
		}
	}

	public ProfanityFilterDataManager(string filePath)
	{
		base.FilePath = filePath;
	}

	protected override void PostLoad()
	{
		ReadProfanityList();
	}

	public bool ContainsProfanity(string name)
	{
		List<ProfanityFilterData> database = GetDatabase();
		if (database == null || database.Count == 0)
		{
			return false;
		}
		string text = name.ToLower();
		for (int i = 0; i < database.Count; i++)
		{
			if (!(database[i].Language == "ZH"))
			{
				string text2 = database[i].BadWord.Trim().ToLower();
				if (!(text2 == string.Empty) && text.Contains(text2))
				{
					return true;
				}
			}
		}
		return false;
	}

	private void ReadProfanityList()
	{
		List<ProfanityFilterData> database = GetDatabase();
		if (database == null || database.Count == 0)
		{
			return;
		}
		string text = KFFLocalization.ReturnLang().ToString();
		profanityFilter = new List<string>();
		for (int i = 0; i < database.Count; i++)
		{
			if (database[i].Language == null || database[i].Language == text)
			{
				profanityFilter.Add(database[i].BadWord.ToLower());
			}
		}
		profanityFilter.Sort((string a, string b) => b.Length.CompareTo(a.Length));
		BuildProfanityReplacements();
	}

	private void BuildProfanityReplacements()
	{
		if (profanityFilter != null && profanityFilter.Count != 0)
		{
			string text = string.Empty;
			profanityReplacements = new string[profanityFilter[0].Length + 1];
			for (int i = 0; i < profanityReplacements.Length; i++)
			{
				profanityReplacements[i] = text;
				text += "*";
			}
		}
	}

	public string ReplaceProfanity(string input)
	{
		if (profanityFilter == null || profanityFilter.Count == 0)
		{
			return input;
		}
		StringBuilder stringBuilder = new StringBuilder(input.ToLower());
		for (int i = 0; i < profanityFilter.Count; i++)
		{
			stringBuilder.Replace(profanityFilter[i], profanityReplacements[profanityFilter[i].Length]);
		}
		StringBuilder stringBuilder2 = new StringBuilder(input);
		for (int j = 0; j < stringBuilder.Length; j++)
		{
			if (stringBuilder[j] == '*')
			{
				stringBuilder2[j] = '*';
			}
		}
		return stringBuilder2.ToString();
	}
}
