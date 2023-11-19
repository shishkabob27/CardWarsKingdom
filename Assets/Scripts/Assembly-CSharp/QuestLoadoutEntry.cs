using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestLoadoutEntry
{
	public class DropInfoClass
	{
		public int SoftCurrency;

		public int SocialCurrency;

		public CreatureData Creature;

		public int CreatureLevel;

		public string CreatureGroup;

		public EvoMaterialData EvoMaterial;

		public XPMaterialData XPMaterial;

		public CardData Card;

		public GachaSlotData GachaKey;
	}

	private CreatureData _Creature;

	private int _CreatureLevel;

	private int _PassiveLevel;

	private int _AppearWeight;

	private float _HPFactor;

	private float _INTFactor;

	private float _STRFactor;

	private int _DEXFactor;

	private int _DEFFactor;

	private int _RESFactor;

	private int _AttackCostFactor;

	private bool _InOrderDraw;

	private float[] _DropWeights = new float[12];

	private float _DropWeightsTotal;

	private DropInfoClass _DropInfo = new DropInfoClass();

	public CreatureData Creature
	{
		get
		{
			return _Creature;
		}
	}

	public int CreatureLevel
	{
		get
		{
			return _CreatureLevel;
		}
	}

	public int PassiveLevel
	{
		get
		{
			return _PassiveLevel;
		}
	}

	public int AppearWeight
	{
		get
		{
			return _AppearWeight;
		}
	}

	public float HPFactor
	{
		get
		{
			return _HPFactor;
		}
	}

	public float STRFactor
	{
		get
		{
			return _STRFactor;
		}
	}

	public float INTFactor
	{
		get
		{
			return _INTFactor;
		}
	}

	public int DEXFactor
	{
		get
		{
			return _DEXFactor;
		}
	}

	public int RESFactor
	{
		get
		{
			return _RESFactor;
		}
	}

	public int DEFFactor
	{
		get
		{
			return _DEFFactor;
		}
	}

	public int AttackCostFactor
	{
		get
		{
			return _AttackCostFactor;
		}
	}

	public int DeployCostFactor { get; private set; }

	public bool InOrderDraw
	{
		get
		{
			return _InOrderDraw;
		}
	}

	public DropInfoClass DropInfo
	{
		get
		{
			return _DropInfo;
		}
	}

	public float CreatureScale { get; private set; }

	public CardData[] AugmentCards { get; private set; }

	public bool DirectDamageImmune { get; private set; }

	public bool PoisonImmune { get; private set; }

	public QuestLoadoutEntry(Dictionary<string, object> dict)
	{
		string iD = TFUtils.LoadString(dict, "CreatureID", string.Empty);
		_Creature = CreatureDataManager.Instance.GetData(iD);
		if (_Creature == null)
		{
			_Creature = CreatureDataManager.Instance.GetNonMuseumCreatureData(iD);
		}
		if (_Creature == null)
		{
			_Creature = CreatureDataManager.Instance.GetTutorialCreatureData(iD);
		}
		_CreatureLevel = TFUtils.LoadInt(dict, "CreatureLevel", 1);
		_PassiveLevel = TFUtils.LoadInt(dict, "PassiveLevel", 1);
		_AppearWeight = TFUtils.LoadInt(dict, "Weight", 1);
		_HPFactor = TFUtils.LoadFloat(dict, "HPFactor", 1f);
		_STRFactor = TFUtils.LoadFloat(dict, "STRFactor", 1f);
		_INTFactor = TFUtils.LoadFloat(dict, "INTFactor", 1f);
		_DEXFactor = TFUtils.LoadInt(dict, "DEXFactor", 0);
		_RESFactor = TFUtils.LoadInt(dict, "RESFactor", 0);
		_DEFFactor = TFUtils.LoadInt(dict, "DEFFactor", 0);
		_AttackCostFactor = TFUtils.LoadInt(dict, "AttackCostFactor", 0);
		DeployCostFactor = TFUtils.LoadInt(dict, "DeployCostFactor", 0);
		_InOrderDraw = TFUtils.LoadBool(dict, "InOrderDraw", false);
		CreatureScale = TFUtils.LoadFloat(dict, "Scale", 1f);
		DirectDamageImmune = TFUtils.LoadBool(dict, "DirectDamageImmune", false);
		PoisonImmune = TFUtils.LoadBool(dict, "PoisonImmune", false);
		AugmentCards = new CardData[3];
		for (int i = 0; i < 3; i++)
		{
			string text = TFUtils.LoadString(dict, "AugmentCard" + (i + 1), null);
			if (text != null)
			{
				AugmentCards[i] = CardDataManager.Instance.GetData(text);
			}
		}
		_DropWeights[1] = TFUtils.LoadFloat(dict, "SoftCurrencyWeight", 0f);
		_DropWeights[2] = TFUtils.LoadFloat(dict, "SocialCurrencyWeight", 0f);
		_DropWeights[11] = TFUtils.LoadFloat(dict, "HardCurrencyWeight", 0f);
		_DropWeights[8] = TFUtils.LoadFloat(dict, "CreatureWeight", 0f);
		_DropWeights[7] = TFUtils.LoadFloat(dict, "MaterialWeight", 0f);
		_DropWeights[5] = TFUtils.LoadFloat(dict, "XPMaterialWeight", 0f);
		_DropWeights[6] = TFUtils.LoadFloat(dict, "AugmentWeight", 0f);
		_DropWeights[10] = TFUtils.LoadFloat(dict, "GachaKeyWeight", 0f);
		_DropWeightsTotal = 0f;
		for (int j = 0; j < 12; j++)
		{
			_DropWeightsTotal += _DropWeights[j];
		}
		_DropInfo.SoftCurrency = TFUtils.LoadInt(dict, "SoftCurrencyAmount", 0);
		_DropInfo.SocialCurrency = TFUtils.LoadInt(dict, "SocialCurrencyAmount", 0);
		_DropInfo.Creature = CreatureDataManager.Instance.GetData(TFUtils.LoadString(dict, "DropCreatureID", string.Empty));
		_DropInfo.CreatureLevel = TFUtils.LoadInt(dict, "DropCreatureLevel", 0);
		_DropInfo.CreatureGroup = TFUtils.LoadString(dict, "DropCreatureGroup", null);
		_DropInfo.EvoMaterial = EvoMaterialDataManager.Instance.GetData(TFUtils.LoadString(dict, "MaterialID", string.Empty));
		_DropInfo.XPMaterial = XPMaterialDataManager.Instance.GetData(TFUtils.LoadString(dict, "XPMaterialID", string.Empty));
		_DropInfo.Card = CardDataManager.Instance.GetData(TFUtils.LoadString(dict, "AugmentID", string.Empty));
		_DropInfo.GachaKey = GachaSlotDataManager.Instance.GetData(TFUtils.LoadString(dict, "GachaKeyID", string.Empty));
		if (_DropWeights[8] > 0f && _DropInfo.Creature == null)
		{
			_DropWeightsTotal -= _DropWeights[8];
			_DropWeights[8] = 0f;
		}
		if (_DropWeights[7] > 0f && _DropInfo.EvoMaterial == null)
		{
			_DropWeightsTotal -= _DropWeights[7];
			_DropWeights[7] = 0f;
		}
		if (_DropWeights[5] > 0f && _DropInfo.XPMaterial == null)
		{
			_DropWeightsTotal -= _DropWeights[5];
			_DropWeights[5] = 0f;
		}
		if (_DropWeights[6] > 0f && _DropInfo.XPMaterial == null)
		{
			_DropWeightsTotal -= _DropWeights[6];
			_DropWeights[6] = 0f;
		}
		if (_DropWeights[10] > 0f && _DropInfo.GachaKey == null)
		{
			_DropWeightsTotal -= _DropWeights[10];
			_DropWeights[10] = 0f;
		}
	}

	public QuestLoadoutEntry(CreatureItem creature, int level, DropTypeEnum dropType, DropInfoClass dropInfo, float creatureScale)
	{
		_Creature = creature.Form;
		_CreatureLevel = level;
		_PassiveLevel = creature.PassiveSkillLevel;
		AugmentCards = new CardData[3];
		for (int i = 0; i < 3; i++)
		{
			if (creature.ExCards[i] != null)
			{
				AugmentCards[i] = creature.ExCards[i].Card.Form;
			}
		}
		_DropWeights[(int)dropType] = 1f;
		_DropWeightsTotal = 1f;
		_DropInfo = dropInfo;
		CreatureScale = creatureScale;
		_HPFactor = 1f;
		_INTFactor = 1f;
		_STRFactor = 1f;
		_DEXFactor = 0;
		_DEFFactor = 0;
		_RESFactor = 0;
	}

	public CreatureItem BuildCreatureItem()
	{
		CreatureItem creatureItem = new CreatureItem(Creature);
		creatureItem.QuestLoadoutEntryData = this;
		creatureItem.Xp = creatureItem.XPTable.GetXpToReachLevel(CreatureLevel);
		creatureItem.SetStarRatingToMatchCurrentXP();
		creatureItem.PassiveSkillLevel = PassiveLevel;
		for (int i = 0; i < 3; i++)
		{
			if (AugmentCards[i] != null)
			{
				creatureItem.ExCards[i] = new InventorySlotItem(new CardItem(AugmentCards[i]));
			}
		}
		return creatureItem;
	}

	public DropTypeEnum RandomizeDropType(bool canLootCards, bool canLootRunes, bool canLootCreatures)
	{
		float num = _DropWeightsTotal;
		if (num == 0f)
		{
			return DropTypeEnum.None;
		}
		float[] array;
		switch (Singleton<PlayerInfoScript>.Instance.StateData.ActiveQuestBonus)
		{
		case QuestBonusType.BonusCreatureDrop:
			array = TransferWeightToType(DropTypeEnum.Creature, 0.5f);
			break;
		case QuestBonusType.BonusEvoMaterialDrop:
			array = TransferWeightToType(DropTypeEnum.EvoMaterial, 0.5f);
			break;
		case QuestBonusType.BonusCardDrop:
			array = TransferWeightToType(DropTypeEnum.Card, 0.5f);
			break;
		case QuestBonusType.BonusSocialCurrencyDrop:
			array = TransferWeightToType(DropTypeEnum.SocialCurrency, 1f);
			break;
		default:
			array = new float[_DropWeights.Length];
			_DropWeights.CopyTo(array, 0);
			break;
		}
		if (!canLootCards)
		{
			num -= array[6];
			array[6] = 0f;
		}
		if (!canLootRunes)
		{
			num -= array[7];
			array[7] = 0f;
		}
		if (!canLootCreatures)
		{
			num -= array[8];
			array[8] = 0f;
		}
		if (num == 0f)
		{
			return DropTypeEnum.None;
		}
		float num2;
		do
		{
			num2 = UnityEngine.Random.Range(0f, num);
		}
		while (num2 == num);
		for (int i = 0; i < array.Length; i++)
		{
			num2 -= array[i];
			if (num2 < 0f)
			{
				return (DropTypeEnum)i;
			}
		}
		return DropTypeEnum.Count;
	}

	private float[] TransferWeightToType(DropTypeEnum dropType, float percentAmount)
	{
		float num = _DropWeights[(int)dropType] * percentAmount;
		float[] array = new float[_DropWeights.Length];
		_DropWeights.CopyTo(array, 0);
		for (int i = 0; i < 12; i++)
		{
			if (i != (int)dropType)
			{
				float num2 = Math.Min(num, array[i]);
				array[i] -= num2;
				array[(int)dropType] += num2;
				num -= num2;
				if (num <= 0f)
				{
					break;
				}
			}
		}
		return array;
	}
}
