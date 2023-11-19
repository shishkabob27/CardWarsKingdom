using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class ExpeditionManager : DetachedSingleton<ExpeditionManager>
{
	public List<ExpeditionItem> CurrentExpeditions = new List<ExpeditionItem>();

	public int AvailableExpeditionCount()
	{
		int num = 0;
		foreach (ExpeditionItem currentExpedition in CurrentExpeditions)
		{
			if (!currentExpedition.InProgress)
			{
				num++;
			}
		}
		return num;
	}

	public int CompletedExpeditionCount()
	{
		uint num = TFUtils.ServerTime.UnixTimestamp();
		int num2 = 0;
		foreach (ExpeditionItem currentExpedition in CurrentExpeditions)
		{
			if (currentExpedition.InProgress && num >= currentExpedition.EndTime)
			{
				num2++;
			}
		}
		return num2;
	}

	public void CompletedOrAvailableExpeditionCount(out int completed, out int total)
	{
		uint num = TFUtils.ServerTime.UnixTimestamp();
		total = 0;
		completed = 0;
		foreach (ExpeditionItem currentExpedition in CurrentExpeditions)
		{
			if (!currentExpedition.InProgress)
			{
				total++;
			}
			else if (num >= currentExpedition.EndTime)
			{
				total++;
				completed++;
			}
		}
	}

	public void AssignNewExpeditions()
	{
		CurrentExpeditions.RemoveAll((ExpeditionItem m) => !m.InProgress);
		GenerateExpeditions(Singleton<PlayerInfoScript>.Instance.SaveData.ExpeditionSlots);
		ExpeditionStartController.RefreshListIfShowing();
		Singleton<PlayerInfoScript>.Instance.SaveData.ExpeditionRefreshTime = TFUtils.ServerTime.UnixTimestamp() + (uint)(ExpeditionParams.ExpeditionRefreshHours * 60 * 60);
	}

	public void GenerateExpeditions(int count)
	{
		for (int i = 0; i < count; i++)
		{
			ExpeditionItem expeditionItem = new ExpeditionItem();
			expeditionItem.Difficulty = ExpeditionDifficultyDataManager.Instance.RollDifficulty();
			expeditionItem.NameData = RollName(expeditionItem.Difficulty);
			expeditionItem.FavoredClass = RollFaction();
			expeditionItem.CreatureCount = expeditionItem.Difficulty.RollCreatureCount();
			expeditionItem.Duration = expeditionItem.Difficulty.RollDuration();
			CurrentExpeditions.Add(expeditionItem);
		}
		SortExpeditions();
	}

	private ExpeditionNameData RollName(ExpeditionDifficultyData difficulty)
	{
		List<ExpeditionNameData> list = new List<ExpeditionNameData>();
		for (int i = 0; i < 2; i++)
		{
			ExpeditionNameData name;
			foreach (ExpeditionNameData item in ExpeditionNameDataManager.Instance.GetDatabase())
			{
				name = item;
				if ((name.Difficulty == -1 || name.Difficulty == difficulty.Difficulty) && (i != 0 || CurrentExpeditions.Find((ExpeditionItem m) => m.NameData == name) == null))
				{
					list.Add(name);
				}
			}
			if (list.Count > 0)
			{
				return list.RandomElement();
			}
		}
		return ExpeditionNameDataManager.Instance.GetDatabase()[0];
	}

	private CreatureFaction RollFaction()
	{
		if (Random.Range(0f, 1f) < ExpeditionParams.UntypedChance)
		{
			return CreatureFaction.Count;
		}
		return (CreatureFaction)Random.Range(1, 6);
	}

	private void SortExpeditions()
	{
		CurrentExpeditions.Sort(delegate(ExpeditionItem lhs, ExpeditionItem rhs)
		{
			if (lhs.InProgress != rhs.InProgress)
			{
				return rhs.InProgress.CompareTo(lhs.InProgress);
			}
			if (lhs.InProgress && rhs.InProgress)
			{
				return lhs.EndTime.CompareTo(rhs.EndTime);
			}
			return (lhs.Duration != rhs.Duration) ? lhs.Duration.CompareTo(rhs.Duration) : lhs.NameData.Name.CompareTo(rhs.NameData.Name);
		});
	}

	public void BeginExpedition(ExpeditionItem expedition, List<InventorySlotItem> creatures)
	{
		expedition.EndTime = TFUtils.ServerTime.UnixTimestamp() + (uint)expedition.Duration;
		expedition.UsedCreatureUIDs.Clear();
		foreach (InventorySlotItem creature in creatures)
		{
			expedition.UsedCreatureUIDs.Add(creature.Creature.UniqueId);
		}
		Singleton<KFFNotificationManager>.Instance.scheduleAdventureCompleteNotification(expedition.Duration, expedition.NameData.ID);
		SortExpeditions();
		Singleton<PlayerInfoScript>.Instance.Save();
	}

	public void CancelExpedition(ExpeditionItem expedition)
	{
		expedition.EndTime = 0u;
		expedition.UsedCreatureUIDs.Clear();
		Singleton<KFFNotificationManager>.Instance.cancelLocalNotification(expedition.NameData.ID);
		SortExpeditions();
		Singleton<PlayerInfoScript>.Instance.Save();
	}

	public void ClaimExpedition(ExpeditionItem expedition, out List<GeneralReward> loot)
	{
		CurrentExpeditions.Remove(expedition);
		Singleton<KFFNotificationManager>.Instance.cancelLocalNotification(expedition.NameData.ID);
		float num = 0f;
		foreach (int usedCreatureUID in expedition.UsedCreatureUIDs)
		{
			InventorySlotItem creatureItem = Singleton<PlayerInfoScript>.Instance.SaveData.GetCreatureItem(usedCreatureUID);
			if (creatureItem != null)
			{
				float num2 = creatureItem.Creature.StarRating;
				if (creatureItem.Creature.Form.Faction == expedition.FavoredClass)
				{
					num2 *= ExpeditionParams.FavoredFactionValue;
				}
				num += num2;
			}
		}
		num *= 1f + Random.Range(0f - ExpeditionParams.OutcomeRandomness, ExpeditionParams.OutcomeRandomness);
		num = Mathf.Max(num, 1f);
		loot = expedition.RollRewards(num);
		foreach (GeneralReward item in loot)
		{
			item.Grant("expedition reward");
		}
		if (Singleton<PlayerInfoScript>.Instance.SaveData.PlayersLastSavedLevel != Singleton<PlayerInfoScript>.Instance.RankXpLevelData.mCurrentLevel)
		{
			DetachedSingleton<StaminaManager>.Instance.RefillStamina();
		}
		Singleton<PlayerInfoScript>.Instance.Save();
	}

	public string Serialize()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[");
		foreach (ExpeditionItem currentExpedition in CurrentExpeditions)
		{
			stringBuilder.Append("{");
			stringBuilder.Append("\"Difficulty\":" + currentExpedition.Difficulty.Difficulty + ",");
			stringBuilder.Append("\"Name\":\"" + currentExpedition.NameData.ID + "\",");
			stringBuilder.Append("\"Duration\":" + currentExpedition.Duration + ",");
			stringBuilder.Append("\"Class\":" + (int)currentExpedition.FavoredClass + ",");
			stringBuilder.Append("\"CreatureCount\":" + currentExpedition.CreatureCount + ",");
			if (currentExpedition.InProgress)
			{
				stringBuilder.Append("\"EndTime\":" + currentExpedition.EndTime + ",");
				stringBuilder.Append("\"Creatures\":[");
				foreach (int usedCreatureUID in currentExpedition.UsedCreatureUIDs)
				{
					stringBuilder.Append(usedCreatureUID + ",");
				}
				stringBuilder.Append("]");
			}
			stringBuilder.Append("},");
		}
		stringBuilder.Append("]");
		return stringBuilder.ToString();
	}

	public void Deserialize(object[] list)
	{
		CurrentExpeditions.Clear();
		foreach (object obj in list)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			ExpeditionItem expeditionItem = new ExpeditionItem();
			expeditionItem.Difficulty = ExpeditionDifficultyDataManager.Instance.GetDataByDifficulty(TFUtils.LoadInt(dictionary, "Difficulty", 1));
			if (expeditionItem.Difficulty == null)
			{
				expeditionItem.Difficulty = ExpeditionDifficultyDataManager.Instance.GetDataByDifficulty(1);
			}
			expeditionItem.NameData = ExpeditionNameDataManager.Instance.GetData(TFUtils.LoadString(dictionary, "Name", string.Empty));
			if (expeditionItem.NameData == null)
			{
				expeditionItem.NameData = ExpeditionNameDataManager.Instance.GetDatabase()[0];
			}
			expeditionItem.Duration = TFUtils.LoadInt(dictionary, "Duration", 1);
			expeditionItem.FavoredClass = (CreatureFaction)TFUtils.LoadInt(dictionary, "Class", 6);
			expeditionItem.CreatureCount = TFUtils.LoadInt(dictionary, "CreatureCount", 1);
			expeditionItem.EndTime = TFUtils.LoadUint(dictionary, "EndTime", 0u);
			if (expeditionItem.InProgress)
			{
				int[] array = dictionary["Creatures"] as int[];
				if (array != null)
				{
					int[] array2 = array;
					foreach (int item in array2)
					{
						expeditionItem.UsedCreatureUIDs.Add(item);
					}
				}
			}
			CurrentExpeditions.Add(expeditionItem);
		}
	}

	public bool IsCreatureOnExpedition(CreatureItem creature)
	{
		if (creature.FromOtherPlayer)
		{
			return false;
		}
		foreach (ExpeditionItem currentExpedition in CurrentExpeditions)
		{
			if (currentExpedition.InProgress && currentExpedition.UsedCreatureUIDs.Contains(creature.UniqueId))
			{
				return true;
			}
		}
		return false;
	}
}
