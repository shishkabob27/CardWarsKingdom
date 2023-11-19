using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

public class CardData : ILoadableData
{
	public class ConditionalValue<T>
	{
		private class Condition
		{
			public MethodInfo ConditionFunction;

			public List<string> Parameters;

			public bool Negated;
		}

		private List<Condition> Conditions;

		public T BaseValue;

		public T ConditionPassedValue;

		public ConditionalValue(string valueString, T defaultValue)
		{
			ParseValueString(valueString, defaultValue);
		}

		public ConditionalValue(Dictionary<string, object> dict, string key, T defaultValue)
		{
			BaseValue = defaultValue;
			object value;
			if (dict.TryGetValue(key, out value))
			{
				string valueString = value as string;
				ParseValueString(valueString, defaultValue);
			}
		}

		private void ParseValueString(string valueString, T defaultValue)
		{
			if (valueString.Contains("?"))
			{
				Type typeFromHandle = typeof(CardConditions);
				Conditions = new List<Condition>();
				string[] array = valueString.Split('?');
				string text = array[0].Trim();
				int num = 0;
				int num2 = 0;
				for (int i = 0; i < text.Length; i++)
				{
					if (text[i] == '(')
					{
						num++;
					}
					else if (text[i] == ')')
					{
						num--;
					}
					if ((text[i] == ',' || i == text.Length - 1) && num == 0)
					{
						Condition condition = new Condition();
						string text2 = text.Substring(num2, 1 + i - num2).Trim();
						int num3 = text2.IndexOf('(');
						int num4 = text2.IndexOf(')');
						string text3 = text2.Substring(0, num3);
						if (text3.StartsWith("!"))
						{
							condition.Negated = true;
							text3 = text3.Substring(1);
						}
						else
						{
							condition.Negated = false;
						}
						condition.ConditionFunction = typeFromHandle.GetMethod(text3, BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
						condition.Parameters = new List<string>();
						string text4 = text2.Substring(num3 + 1, num4 - num3 - 1);
						string[] array2 = text4.Split(',');
						string[] array3 = array2;
						foreach (string text5 in array3)
						{
							condition.Parameters.Add(text5.Trim());
						}
						Conditions.Add(condition);
						num2 = i + 1;
					}
				}
				string text6 = array[1].Trim();
				string[] array4 = text6.Split(':');
				ConditionPassedValue = (T)Convert.ChangeType(array4[0].Trim(), typeof(T));
				if (array4.Length > 1 && array4[1].Length > 0)
				{
					BaseValue = (T)Convert.ChangeType(array4[1].Trim(), typeof(T));
				}
				else
				{
					BaseValue = defaultValue;
				}
				return;
			}
			try
			{
				BaseValue = (T)Convert.ChangeType(valueString, typeof(T));
			}
			catch (Exception)
			{
			}
		}

		public T GetConditionalValue(CardConditionData data, CardData card)
		{
			//Discarded unreachable code: IL_0095
			if (Conditions != null)
			{
				try
				{
					object[] parameters = new object[1] { data };
					foreach (Condition condition in Conditions)
					{
						data.Parameters = condition.Parameters;
						if ((bool)condition.ConditionFunction.Invoke(null, parameters) == condition.Negated)
						{
							return BaseValue;
						}
					}
				}
				catch (Exception)
				{
					return BaseValue;
				}
				return ConditionPassedValue;
			}
			return BaseValue;
		}
	}

	public const int MAX_TARGET_FILTERS = 2;

	private bool mOverrideShowPlayerStats;

	private bool mOverrideShowOpponentStats;

	private List<CreatureStat> mShowPlayerStats = new List<CreatureStat>();

	private List<CreatureStat> mShowOpponentStats = new List<CreatureStat>();

	private EffectMultiplier mMultiplier;

	private Dictionary<string, EffectMultiplier> Multipliers = new Dictionary<string, EffectMultiplier>();

	public Dictionary<StatusData, StatusInfo> StatusValues = new Dictionary<StatusData, StatusInfo>();

	public List<KeyWordData> DescriptionKeywords = new List<KeyWordData>();

	private bool mKeywordsParsed;

	public string ID { get; private set; }

	public string Name { get; private set; }

	public string Description { get; private set; }

	public string AssetBundle { get; private set; }

	public string UITexture { get; private set; }

	public SelectionType TargetType1 { get; private set; }

	public SelectionType TargetType2 { get; private set; }

	public MethodInfo[] TargetFilters1 { get; private set; }

	public MethodInfo[] TargetFilters2 { get; private set; }

	public CreatureFaction Faction { get; private set; }

	public ObscuredInt Cost { get; private set; }

	public int SellPrice { get; private set; }

	public bool IsLeaderCard { get; private set; }

	public string CardFrame { get; private set; }

	public int Rarity { get; private set; }

	public string TypeText { get; private set; }

	public ConditionalValue<int> NumberOfAttacks { get; private set; }

	public AttackRange TargetGroup { get; private set; }

	public AttackRange Target2Group { get; private set; }

	public AttackBase AttackBase { get; private set; }

	public string DirectDamageFX { get; private set; }

	public GameEventFXSpawnPoint FXSpawnPoint { get; private set; }

	public ConditionalValue<int> DrawCards { get; private set; }

	public ConditionalValue<int> GainAP { get; private set; }

	public ConditionalValue<int> DrainAP { get; private set; }

	public ConditionalValue<float> SuckAP { get; private set; }

	public ConditionalValue<int> PullCreatures { get; private set; }

	public int CostToCreate { get; private set; }

	public int CostToUnsocket { get; private set; }

	public bool OneShot { get; private set; }

	public StatusRemovalData RemoveStatusData { get; private set; }

	public bool AnyPositiveEffects { get; private set; }

	public bool AlreadySeen { get; set; }

	public bool AlreadyCollected { get; set; }

	public int MaxConditionalAttacks()
	{
		return Mathf.Max(NumberOfAttacks.BaseValue, NumberOfAttacks.ConditionPassedValue);
	}

	public EffectMultiplier GetMultiplier(string key)
	{
		if (Multipliers.ContainsKey(key))
		{
			return Multipliers[key];
		}
		return mMultiplier;
	}

	private void ParseMultipliers(string rawString)
	{
		string[] array = rawString.Trim().Split(',');
		string[] array2 = array;
		foreach (string text in array2)
		{
			string[] array3 = text.Trim().Split(':');
			if (array3.Length > 1)
			{
				string key = array3[0].Trim();
				string value = array3[1].Trim();
				EffectMultiplier value2 = (EffectMultiplier)(int)Enum.Parse(typeof(EffectMultiplier), value, true);
				Multipliers.Add(key, value2);
			}
			else
			{
				mMultiplier = (EffectMultiplier)(int)Enum.Parse(typeof(EffectMultiplier), array3[0], true);
			}
		}
	}

	public void Populate(Dictionary<string, object> dict)
	{
		Type typeFromHandle = typeof(TargetFilters);
		ID = TFUtils.LoadString(dict, "ID", string.Empty);
		Name = TFUtils.LoadLocalizedString(dict, "Name", string.Empty);
		Description = TFUtils.LoadLocalizedString(dict, "Description", string.Empty);
		AssetBundle = TFUtils.LoadString(dict, "UIBundle", null);
		string text = ((AssetBundle == null) ? "UI/Icons_ActionPortraits/" : ("UI/Icons_ActionPortraits/" + AssetBundle + "/"));
		UITexture = text + TFUtils.LoadString(dict, "UITexture", string.Empty);
		Faction = (CreatureFaction)(int)Enum.Parse(typeof(CreatureFaction), TFUtils.LoadString(dict, "Faction", string.Empty), true);
		SellPrice = TFUtils.LoadInt(dict, "SellPrice", 1);
		Rarity = TFUtils.LoadInt(dict, "Rarity", 1);
		OneShot = TFUtils.LoadBool(dict, "OneShot", false);
		IsLeaderCard = TFUtils.LoadBool(dict, "IsLeaderCard", false);
		string text2 = ((AssetBundle == null) ? "UI/CardFrames/" : ("UI/CardFrames/" + AssetBundle + "/"));
		CardFrame = text2 + TFUtils.LoadString(dict, "CardFrame", string.Empty);
		TypeText = TFUtils.LoadLocalizedString(dict, "TypeText", string.Empty);
		CostToCreate = TFUtils.LoadInt(dict, "CostToCreate", 0);
		CostToUnsocket = TFUtils.LoadInt(dict, "CostToUnsocket", 0);
		Cost = TFUtils.LoadInt(dict, "Cost", 1);
		TargetType1 = (SelectionType)(int)Enum.Parse(typeof(SelectionType), TFUtils.LoadString(dict, "TargetType1", "None"));
		TargetType2 = (SelectionType)(int)Enum.Parse(typeof(SelectionType), TFUtils.LoadString(dict, "TargetType2", "None"));
		TargetFilters1 = new MethodInfo[2];
		for (int i = 1; i <= 2; i++)
		{
			TargetFilters1[i - 1] = typeFromHandle.GetMethod(TFUtils.LoadString(dict, "Target1Filter" + i, string.Empty), BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
		}
		TargetFilters2 = new MethodInfo[2];
		for (int j = 1; j <= 2; j++)
		{
			TargetFilters2[j - 1] = typeFromHandle.GetMethod(TFUtils.LoadString(dict, "Target2Filter" + j, string.Empty), BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
		}
		NumberOfAttacks = new ConditionalValue<int>(dict, "AttackNum", 0);
		TargetGroup = (AttackRange)(int)Enum.Parse(typeof(AttackRange), TFUtils.LoadString(dict, "TargetGroup", "None"), true);
		Target2Group = (AttackRange)(int)Enum.Parse(typeof(AttackRange), TFUtils.LoadString(dict, "Target2Group", "None"), true);
		AttackBase = (AttackBase)(int)Enum.Parse(typeof(AttackBase), TFUtils.LoadString(dict, "AttackBase", "None"), true);
		DirectDamageFX = TFUtils.LoadString(dict, "CardFX", string.Empty);
		string text3 = TFUtils.LoadString(dict, "FXSpawnPoint", null);
		if (text3 != null)
		{
			FXSpawnPoint = (GameEventFXSpawnPoint)(int)Enum.Parse(typeof(GameEventFXSpawnPoint), text3);
		}
		DrawCards = new ConditionalValue<int>(dict, "DrawCards", 0);
		GainAP = new ConditionalValue<int>(dict, "GainAP", 0);
		DrainAP = new ConditionalValue<int>(dict, "DrainAP", 0);
		SuckAP = new ConditionalValue<float>(dict, "SuckAP", 0f);
		PullCreatures = new ConditionalValue<int>(dict, "Pull", 0);
		string rawString = TFUtils.LoadString(dict, "Multiplier", "None");
		ParseMultipliers(rawString);
		StatusData data = StatusDataManager.Instance.GetData("RemoveStatus");
		foreach (string key in dict.Keys)
		{
			StatusData data2 = StatusDataManager.Instance.GetData(key);
			if (data2 == null)
			{
				continue;
			}
			string text4 = TFUtils.LoadString(dict, key, string.Empty);
			text4 = text4.Trim();
			if (!string.IsNullOrEmpty(text4))
			{
				StatusInfo value = default(StatusInfo);
				string[] array = text4.Split('>');
				value.Value = new ConditionalValue<float>(array[0].Trim(), 0f);
				StatusValues.Add(data2, value);
				if (data2.StatusType == StatusType.Buff && data2 != data)
				{
					AnyPositiveEffects = true;
				}
			}
		}
		RemoveStatusData = new StatusRemovalData();
		string text5 = TFUtils.LoadString(dict, "RemoveStatusTargets", null);
		if (text5 != null)
		{
			string[] array2 = text5.Split(',');
			string[] array3 = array2;
			foreach (string text6 in array3)
			{
				string text7 = text6.Trim();
				if (text7 == "RandomBuff")
				{
					RemoveStatusData.RandomBuffs = true;
					continue;
				}
				if (text7 == "RandomDebuff")
				{
					RemoveStatusData.RandomDebuffs = true;
					continue;
				}
				StatusData data3 = StatusDataManager.Instance.GetData(text7);
				if (data3 != null)
				{
					if (RemoveStatusData.Targets == null)
					{
						RemoveStatusData.Targets = new List<StatusData>();
					}
					RemoveStatusData.Targets.Add(data3);
				}
			}
		}
		if (DrawCards.BaseValue > 0 || DrawCards.ConditionPassedValue > 0 || GainAP.BaseValue > 0 || GainAP.ConditionPassedValue > 0 || DrainAP.BaseValue > 0 || DrainAP.ConditionPassedValue > 0 || SuckAP.BaseValue > 0f || SuckAP.ConditionPassedValue > 0f || NumberOfAttacks.BaseValue > 0 || NumberOfAttacks.ConditionPassedValue > 0)
		{
			AnyPositiveEffects = true;
		}
		string text8 = TFUtils.LoadString(dict, "ShowPlayerStats", string.Empty);
		if (text8 != string.Empty)
		{
			mOverrideShowPlayerStats = true;
			if (text8.ToLower() != "none")
			{
				mShowPlayerStats = KeyWordDataManager.ParseStatsList(text8);
			}
		}
		string text9 = TFUtils.LoadString(dict, "ShowOpponentStats", string.Empty);
		if (text9 != string.Empty)
		{
			mOverrideShowOpponentStats = true;
			if (text9.ToLower() != "none")
			{
				mShowOpponentStats = KeyWordDataManager.ParseStatsList(text9);
			}
		}
	}

	public void ParseKeywords()
	{
		if (mKeywordsParsed)
		{
			return;
		}
		DescriptionKeywords.Clear();
		foreach (KeyWordData item in KeyWordDataManager.Instance.GetDatabase())
		{
			if (Description.Contains(item.DisplayName))
			{
				DescriptionKeywords.Add(item);
			}
		}
		DescriptionKeywords.RemoveAll((KeyWordData m) => DescriptionKeywords.Find((KeyWordData m2) => m2.RedundantKeywords.Contains(m)) != null);
		mKeywordsParsed = true;
	}

	public void BuildDescriptionString(UILabel label, string baseColorString)
	{
		ParseKeywords();
		label.color = Color.white;
		string text = baseColorString + Description;
		foreach (KeyWordData descriptionKeyword in DescriptionKeywords)
		{
			string newValue = "[" + descriptionKeyword.TextColor + "]" + descriptionKeyword.DisplayName + baseColorString;
			text = text.Replace(descriptionKeyword.DisplayName, newValue);
		}
		label.text = text;
	}

	public List<CreatureStat> GetStatHints(PlayerType player)
	{
		ParseKeywords();
		if ((player != PlayerType.User) ? mOverrideShowOpponentStats : mOverrideShowPlayerStats)
		{
			return (player != PlayerType.User) ? mShowOpponentStats : mShowPlayerStats;
		}
		List<CreatureStat> list = new List<CreatureStat>();
		foreach (KeyWordData descriptionKeyword in DescriptionKeywords)
		{
			ReadOnlyCollection<CreatureStat> readOnlyCollection = ((player != PlayerType.User) ? descriptionKeyword.ShowOpponentStats : descriptionKeyword.ShowPlayerStats);
			foreach (CreatureStat item in readOnlyCollection)
			{
				if (!list.Contains(item))
				{
					list.Add(item);
				}
			}
		}
		return list;
	}

	public bool RequiresTargetSelection()
	{
		return TargetType2 == SelectionType.Lane || AttackBase != AttackBase.None;
	}

	public bool HasKeywords(string[] keywordIDs)
	{
		ParseKeywords();
		string keyword;
		for (int i = 0; i < keywordIDs.Length; i++)
		{
			keyword = keywordIDs[i];
			if (DescriptionKeywords.Find((KeyWordData m) => m.DisplayName == keyword) == null)
			{
				return false;
			}
		}
		return true;
	}
}
