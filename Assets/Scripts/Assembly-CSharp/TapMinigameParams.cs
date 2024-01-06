using System.Collections.Generic;
using System.IO;

public class TapMinigameParams : DataManager<DummyData>
{
	public class TapMinigameRange
	{
		public string DisplayText;

		public float BeforeTapTime;

		public float AfterTapTime;

		public int AttackBonus;

		public int DefenseBonus;
	}

	private float _NoteDuration;

	private float _NoteDurationChange;

	private float _NoteRotation;

	private float _NoteFadeInTime;

	private float _NoteSpacingMin;

	private float _NoteSpacingMax;

	private float _NoteSpacingChange;

	private float _MaxTapRange;

	private float _DoubleUpChance;

	private static TapMinigameParams _instance;

	public List<TapMinigameRange> TapRanges = new List<TapMinigameRange>();

	public static float NoteDuration
	{
		get
		{
			return Instance._NoteDuration;
		}
	}

	public static float NoteDurationChange
	{
		get
		{
			return Instance._NoteDurationChange;
		}
	}

	public static float NoteRotation
	{
		get
		{
			return Instance._NoteRotation;
		}
	}

	public static float NoteFadeInTime
	{
		get
		{
			return Instance._NoteFadeInTime;
		}
	}

	public static float NoteSpacingMin
	{
		get
		{
			return Instance._NoteSpacingMin;
		}
	}

	public static float NoteSpacingMax
	{
		get
		{
			return Instance._NoteSpacingMax;
		}
	}

	public static float NoteSpacingChange
	{
		get
		{
			return Instance._NoteSpacingChange;
		}
	}

	public static float MaxTapRange
	{
		get
		{
			return Instance._MaxTapRange;
		}
	}

	public static float DoubleUpChance
	{
		get
		{
			return Instance._DoubleUpChance;
		}
	}

	public static TapMinigameParams Instance
	{
		get
		{
			if (_instance == null)
			{
				string path = Path.Combine(SQSettings.CDN_URL, "Blueprints", "db_TapMinigame.json");
				_instance = new TapMinigameParams(path);
			}
			return _instance;
		}
	}

	public TapMinigameParams(string path)
	{
		base.FilePath = path;
	}

	protected override void ParseRows(List<object> jlist)
	{
		if (jlist.Count == 0)
		{
			return;
		}
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		TapRanges.Clear();
		_MaxTapRange = 0f;
		foreach (object item in jlist)
		{
			Dictionary<string, object> dictionary2 = (Dictionary<string, object>)item;
			string text = TFUtils.LoadString(dictionary2, "Parameter", string.Empty);
			if (text == "_RANGE")
			{
				TapMinigameRange tapMinigameRange = new TapMinigameRange();
				tapMinigameRange.DisplayText = TFUtils.LoadString(dictionary2, "RangeText", string.Empty);
				tapMinigameRange.BeforeTapTime = TFUtils.LoadFloat(dictionary2, "RangeBeforeTap", 9999f);
				tapMinigameRange.AfterTapTime = TFUtils.LoadFloat(dictionary2, "RangeAfterTap", 9999f);
				tapMinigameRange.AttackBonus = TFUtils.LoadInt(dictionary2, "AttackBonus", 0);
				tapMinigameRange.DefenseBonus = TFUtils.LoadInt(dictionary2, "DefenseBonus", 0);
				TapRanges.Add(tapMinigameRange);
				if (tapMinigameRange.AfterTapTime != 9999f && tapMinigameRange.AfterTapTime > _MaxTapRange)
				{
					_MaxTapRange = tapMinigameRange.AfterTapTime;
				}
			}
			else
			{
				object value = dictionary2["Value"];
				dictionary.Add(text, value);
			}
		}
		_NoteDuration = TFUtils.LoadFloat(dictionary, "NoteDuration", 1f);
		_NoteDurationChange = TFUtils.LoadFloat(dictionary, "NoteDurationChange", 0f);
		_NoteRotation = TFUtils.LoadFloat(dictionary, "NoteRotation", 0f);
		_NoteFadeInTime = TFUtils.LoadFloat(dictionary, "NoteFadeInTime", 0f);
		_NoteSpacingMin = TFUtils.LoadFloat(dictionary, "NoteSpacingMin", 0f);
		_NoteSpacingMax = TFUtils.LoadFloat(dictionary, "NoteSpacingMax", 0f);
		_NoteSpacingChange = TFUtils.LoadFloat(dictionary, "NoteSpacingChange", 0f);
		_DoubleUpChance = TFUtils.LoadFloat(dictionary, "DoubleUpChance", 0f);
	}
}
