using System;
using System.Collections.Generic;

public class SpecialSaleData : ILoadableData
{
	public class CreatureQuantity
	{
		public CreatureData Data;

		public int Quantity;
	}

	public class CardQuantity
	{
		public CardData Data;

		public int Quantity;
	}

	public class EvoMaterialQuantity
	{
		public EvoMaterialData Data;

		public int Quantity;
	}

	private string PrerequisiteSaleString;

	public string ID { get; private set; }

	public string ProductID { get; private set; }

	public string Name { get; private set; }

	public string Description { get; private set; }

	public string PackTitle { get; private set; }

	public string BadgeText { get; private set; }

	public string Texture { get; private set; }

	public string BadgeTexture { get; private set; }

	public SpecialSaleData PrerequisiteSale { get; private set; }

	public int ShowDuration { get; private set; }

	public int CooldownMin { get; private set; }

	public int CooldownMax { get; private set; }

	public DateTime StartDate { get; private set; }

	public DateTime EndDate { get; private set; }

	public int Priority { get; private set; }

	public bool Repeatable { get; private set; }

	public List<GeneralReward> Items { get; private set; }

	public void Populate(Dictionary<string, object> dict)
	{
		ID = TFUtils.LoadString(dict, "ID", string.Empty);
		ProductID = TFUtils.LoadString(dict, "ProductID", string.Empty);
		Name = TFUtils.LoadLocalizedString(dict, "Name", string.Empty);
		Description = TFUtils.LoadLocalizedString(dict, "Description", string.Empty);
		PackTitle = TFUtils.LoadLocalizedString(dict, "PackTitle", string.Empty);
		BadgeText = TFUtils.LoadLocalizedString(dict, "BadgeText", string.Empty);
		Texture = "UI/GeneralBundle/SaleArt/" + TFUtils.LoadString(dict, "Texture", string.Empty);
		BadgeTexture = "UI/GeneralBundle/SaleArt/" + TFUtils.LoadString(dict, "BadgeTexture", string.Empty);
		PrerequisiteSaleString = TFUtils.LoadString(dict, "PrerequisiteSale", null);
		ShowDuration = TFUtils.LoadInt(dict, "ShowDuration", 0);
		CooldownMin = TFUtils.LoadInt(dict, "CooldownMin", 0);
		CooldownMax = TFUtils.LoadInt(dict, "CooldownMax", 0);
		Priority = TFUtils.LoadInt(dict, "Priority", 0);
		Repeatable = TFUtils.LoadBool(dict, "Repeatable", false);
		string text = TFUtils.LoadString(dict, "StartDate", null);
		if (text != null)
		{
			StartDate = DateTime.Parse(text);
		}
		else
		{
			StartDate = DateTime.MinValue;
		}
		string text2 = TFUtils.LoadString(dict, "EndDate", null);
		if (text2 != null)
		{
			EndDate = DateTime.Parse(text2);
		}
		else
		{
			EndDate = DateTime.MaxValue;
		}
		Items = GeneralReward.ParseFromJson(dict);
	}

	public void PostLoad()
	{
		if (PrerequisiteSaleString != null)
		{
			PrerequisiteSale = SpecialSaleDataManager.Instance.GetData(PrerequisiteSaleString);
			PrerequisiteSaleString = null;
		}
	}
}
