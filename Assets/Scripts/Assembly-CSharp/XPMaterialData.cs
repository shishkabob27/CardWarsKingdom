using System;
using System.Collections.Generic;

public class XPMaterialData : ILoadableData
{
	public string ID { get; private set; }

	public CreatureFaction Faction { get; private set; }

	public string UICardFrame { get; private set; }

	public string UITexture { get; private set; }

	public string Name { get; private set; }

	public string Desc { get; private set; }

	public int FeedXP { get; private set; }

	public bool FactionLimited { get; private set; }

	public int SellPrice { get; private set; }

	public int Rarity { get; private set; }

	public bool AlreadySeen { get; set; }

	public bool AlreadyCollected { get; set; }

	public void Populate(Dictionary<string, object> dict)
	{
		ID = TFUtils.LoadString(dict, "ID", string.Empty);
		Faction = (CreatureFaction)(int)Enum.Parse(typeof(CreatureFaction), TFUtils.LoadString(dict, "Faction", "Colorless"));
		UICardFrame = TFUtils.LoadString(dict, "UICardFrame", string.Empty);
		UITexture = TFUtils.LoadString(dict, "UITexture", string.Empty);
		Name = TFUtils.LoadLocalizedString(dict, "Name", string.Empty);
		Desc = TFUtils.LoadLocalizedString(dict, "Desc", string.Empty);
		FeedXP = TFUtils.LoadInt(dict, "FeedXP", 0);
		FactionLimited = TFUtils.LoadBool(dict, "FactionLimited", false);
		SellPrice = TFUtils.LoadInt(dict, "SellPrice", 0);
		Rarity = TFUtils.LoadInt(dict, "Rarity", 0);
	}
}
