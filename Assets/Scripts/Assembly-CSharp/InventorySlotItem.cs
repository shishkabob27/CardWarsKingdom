public class InventorySlotItem
{
	public InventorySlotType SlotType { get; set; }

	public CreatureItem Creature { get; set; }

	public CreatureData DisplayCreature { get; set; }

	public CardItem Card { get; set; }

	public CardData DisplayCard { get; set; }

	public EvoMaterialData EvoMaterial { get; set; }

	public XPMaterialData XPMaterial { get; set; }

	public bool FirstTimeCollected { get; set; }

	public bool GivenForTutorial { get; set; }

	public int Rarity
	{
		get
		{
			switch (SlotType)
			{
			case InventorySlotType.Creature:
				return Creature.Form.Rarity;
			case InventorySlotType.DisplayCreature:
				return DisplayCreature.Rarity;
			case InventorySlotType.Card:
				return Card.Form.Rarity;
			case InventorySlotType.DisplayCard:
				return DisplayCard.Rarity;
			case InventorySlotType.EvoMaterial:
				return EvoMaterial.Rarity;
			case InventorySlotType.XPMaterial:
				return XPMaterial.Rarity;
			default:
				return -1;
			}
		}
	}

	public InventorySlotItem()
	{
	}

	public InventorySlotItem(InventorySlotType type)
	{
		SlotType = type;
	}

	public InventorySlotItem(CreatureItem creature)
	{
		Fill(creature);
	}

	public InventorySlotItem(CreatureData creature)
	{
		Fill(creature);
	}

	public InventorySlotItem(EvoMaterialData evoMaterial)
	{
		Fill(evoMaterial);
	}

	public InventorySlotItem(XPMaterialData xpMaterial)
	{
		Fill(xpMaterial);
	}

	public InventorySlotItem(CardItem card)
	{
		Fill(card);
	}

	public InventorySlotItem(CardData card)
	{
		Fill(card);
	}

	public override string ToString()
	{
		string text = SlotType.ToString();
		if (SlotType == InventorySlotType.Creature)
		{
			text += ": ";
			text = ((Creature == null) ? (text + "null") : (text + Creature.Form.ID));
		}
		else if (SlotType == InventorySlotType.DisplayCreature)
		{
			text += ": ";
			text = ((DisplayCreature == null) ? (text + "null") : (text + DisplayCreature.ID));
		}
		else if (SlotType == InventorySlotType.EvoMaterial)
		{
			text += ": ";
			text = ((EvoMaterial == null) ? (text + "null") : (text + EvoMaterial.ID));
		}
		else if (SlotType == InventorySlotType.XPMaterial)
		{
			text += ": ";
			text = ((XPMaterial == null) ? (text + "null") : (text + XPMaterial.ID));
		}
		else if (SlotType == InventorySlotType.Card)
		{
			text += ": ";
			text = ((Card == null) ? (text + "null") : (text + Card.Form.ID));
		}
		return text;
	}

	public void Clear()
	{
		Creature = null;
		Card = null;
		EvoMaterial = null;
		SlotType = InventorySlotType.Empty;
	}

	public void Fill(CreatureItem creature)
	{
		Clear();
		Creature = creature;
		SlotType = InventorySlotType.Creature;
	}

	public void Fill(CreatureData creature)
	{
		Clear();
		DisplayCreature = creature;
		SlotType = InventorySlotType.DisplayCreature;
	}

	public void Fill(CardItem card)
	{
		Clear();
		Card = card;
		SlotType = InventorySlotType.Card;
	}

	public void Fill(CardData card)
	{
		Clear();
		DisplayCard = card;
		SlotType = InventorySlotType.DisplayCard;
	}

	public void Fill(EvoMaterialData evoMaterial)
	{
		Clear();
		EvoMaterial = evoMaterial;
		SlotType = InventorySlotType.EvoMaterial;
	}

	public void Fill(XPMaterialData xpMaterial)
	{
		Clear();
		XPMaterial = xpMaterial;
		SlotType = InventorySlotType.XPMaterial;
	}

	public int SellPrice()
	{
		switch (SlotType)
		{
		case InventorySlotType.Creature:
			return Creature.GetSellPrice();
		case InventorySlotType.Card:
			return Card.GetSellPrice();
		case InventorySlotType.EvoMaterial:
			return EvoMaterial.SellPrice;
		case InventorySlotType.XPMaterial:
			return XPMaterial.SellPrice;
		default:
			return 0;
		}
	}

	public bool InUse()
	{
		if (SlotType == InventorySlotType.Creature)
		{
			return Singleton<PlayerInfoScript>.Instance.IsCreatureInAnyLoadout(Creature);
		}
		if (SlotType == InventorySlotType.Card)
		{
			return Card.CreatureUID != 0;
		}
		return false;
	}

	public bool IsFavorite()
	{
		if (SlotType == InventorySlotType.Creature)
		{
			return Creature.Favorite;
		}
		return false;
	}

	public bool IsHelper()
	{
		if (SlotType == InventorySlotType.Creature)
		{
			return Singleton<PlayerInfoScript>.Instance.IsCreatureSetAsHelper(Creature);
		}
		return false;
	}

	public bool IsOnExpedition()
	{
		if (SlotType == InventorySlotType.Creature)
		{
			return DetachedSingleton<ExpeditionManager>.Instance.IsCreatureOnExpedition(Creature);
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		InventorySlotItem inventorySlotItem = obj as InventorySlotItem;
		if (inventorySlotItem != null)
		{
			return Equals(inventorySlotItem);
		}
		return base.Equals(inventorySlotItem);
	}

	public bool Equals(InventorySlotItem item)
	{
		if (item == null)
		{
			return false;
		}
		if (this == item)
		{
			return true;
		}
		if (SlotType != item.SlotType)
		{
			return false;
		}
		switch (SlotType)
		{
		case InventorySlotType.Card:
			return Card == item.Card;
		case InventorySlotType.Creature:
			return Creature == item.Creature;
		case InventorySlotType.DisplayCard:
			return DisplayCard == item.DisplayCard;
		case InventorySlotType.DisplayCreature:
			return DisplayCreature == item.DisplayCreature;
		case InventorySlotType.EvoMaterial:
			return EvoMaterial == item.EvoMaterial;
		case InventorySlotType.XPMaterial:
			return XPMaterial == item.XPMaterial;
		default:
			return false;
		}
	}
}
