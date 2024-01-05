using System;
using System.Collections.Generic;
using System.Globalization;

public class GachaEventData : ILoadableData
{
	public string ID { get; private set; }

	public string Name { get; private set; }

	public string Description { get; private set; }

	public DateTime StartTime { get; private set; }

	public DateTime EndTime { get; private set; }

	public GachaSlotData Slot { get; private set; }

	public GachaWeightTable WeightsTable { get; private set; }

	public int BonusLevels { get; private set; }

	public int BonusPassiveLevels { get; private set; }

	public int BonusCardSlots { get; private set; }

	public CreatureFaction BonusClassRestriction { get; private set; }

	public void Populate(Dictionary<string, object> dict)
	{
		ID = TFUtils.LoadString(dict, "ID", string.Empty);
		StartTime = DateTime.ParseExact(TFUtils.LoadString(dict, "Start", string.Empty), "M/d/yyyy", CultureInfo.GetCultureInfo("en-US"));
		EndTime = DateTime.ParseExact(TFUtils.LoadString(dict, "End", string.Empty), "M/d/yyyy", CultureInfo.GetCultureInfo("en-US"));
		Slot = GachaSlotDataManager.Instance.GetData(TFUtils.LoadString(dict, "Slot", string.Empty));
		WeightsTable = GachaWeightDataManager.Instance.GetData(TFUtils.LoadString(dict, "WeightsTable", string.Empty));
		Name = TFUtils.LoadLocalizedString(dict, "Name", string.Empty);
		Description = TFUtils.LoadLocalizedString(dict, "Description", string.Empty);
		BonusLevels = TFUtils.LoadInt(dict, "LevelBonus", 0);
		BonusPassiveLevels = TFUtils.LoadInt(dict, "PassiveLevelBonus", 0);
		BonusCardSlots = TFUtils.LoadInt(dict, "CardSlotsBonus", 0);
		string text = TFUtils.LoadString(dict, "BonusClassRestriction", null);
		if (text != null)
		{
			BonusClassRestriction = (CreatureFaction)(int)Enum.Parse(typeof(CreatureFaction), text);
		}
		else
		{
			BonusClassRestriction = CreatureFaction.Count;
		}
	}
}
