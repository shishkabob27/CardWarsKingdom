using System.Collections.Generic;
using UnityEngine;

public class EvoStatsPanelController : Singleton<EvoStatsPanelController>
{
	public UILabel CreatureName;

	public UILabel CreatureLevel;

	public UILabel CreatureLevelBefore;

	public UILabel CreatureMaxLevel;

	public UILabel CreatureMaxLevelBefore;

	public UILabel[] GemNames;

	public UILabel[] GemDescriptions;

	public UILabel[] StatsBefore = new UILabel[6];

	public UILabel[] StatsAfter = new UILabel[6];

	public UILabel PassiveSkill;

	public UILabel PassiveLevel;

	public UILabel PassiveSkillBefore;

	public UILabel PassiveLevelBefore;

	public CreatureItem ResultCreature;

	public CreatureItem ResultCreatureBefore;

	public Transform SkillCardNodeParent;

	public Transform ZoomPosition;

	private List<CardPrefabScript> mSpawnedActionCards = new List<CardPrefabScript>();

	private GameObject[] SkillCardNodes = new GameObject[5];

	private void Awake()
	{
		for (int i = 0; i < 5; i++)
		{
			SkillCardNodes[i] = SkillCardNodeParent.FindChild("ActionCardSpawnNode_0" + (i + 1)).gameObject;
		}
	}

	public void HideStats()
	{
		ResultCreature = null;
		ResultCreatureBefore = null;
		Unload();
	}

	public void Populate()
	{
		ResultCreature.Form.ParseKeywords();
		if ((bool)CreatureName)
		{
			CreatureName.text = ResultCreature.Form.Name;
		}
		if ((bool)PassiveLevel)
		{
			if (ResultCreature.PassiveLevelString() != ResultCreatureBefore.PassiveLevelString())
			{
				PassiveLevel.text = ResultCreature.PassiveLevelString();
				if ((bool)PassiveSkill)
				{
					PassiveSkill.text = ResultCreature.BuildPassiveDescriptionString(false);
				}
			}
			else
			{
				PassiveLevel.text = string.Empty;
				if ((bool)PassiveSkill)
				{
					PassiveSkill.text = string.Empty;
				}
			}
		}
		for (int i = 0; i < 6; i++)
		{
			StatsAfter[i].text = ResultCreature.GetStat((CreatureStat)i).ToString();
		}
		CreatureMaxLevel.text = ResultCreature.MaxLevel.ToString();
		CreatureLevel.text = ResultCreature.Level.ToString();
		if (ResultCreatureBefore != null)
		{
			PassiveLevelBefore.text = ResultCreatureBefore.PassiveLevelString();
			PassiveSkillBefore.text = ResultCreatureBefore.BuildPassiveDescriptionString(false);
			for (int j = 0; j < 6; j++)
			{
				StatsBefore[j].text = ResultCreatureBefore.GetStat((CreatureStat)j).ToString();
			}
			CreatureMaxLevelBefore.text = ResultCreatureBefore.MaxLevel.ToString();
			CreatureLevelBefore.text = ResultCreatureBefore.Level.ToString();
		}
		PopulateCardsInStats();
	}

	private void PopulateCardsInStats()
	{
		ResultCreature.Form.ParseKeywords();
		foreach (CardPrefabScript mSpawnedActionCard in mSpawnedActionCards)
		{
			NGUITools.Destroy(mSpawnedActionCard.gameObject);
		}
		mSpawnedActionCards.Clear();
		for (int i = 0; i < 5; i++)
		{
			if (ResultCreature.Form.ActionCards[i] != null)
			{
				GameObject gameObject = SkillCardNodes[i].InstantiateAsChild(Singleton<PrefabReferences>.Instance.Card);
				gameObject.ChangeLayer(base.gameObject.layer);
				CardPrefabScript component = gameObject.GetComponent<CardPrefabScript>();
				component.Mode = CardPrefabScript.CardMode.GeneralFrontEnd;
				component.Populate(ResultCreature.Form.ActionCards[i]);
				component.AdjustDepth(i + 1);
				component.SetCardState(CardPrefabScript.HandCardState.InHand);
				mSpawnedActionCards.Add(component);
			}
		}
	}

	public void Unload()
	{
		foreach (CardPrefabScript mSpawnedActionCard in mSpawnedActionCards)
		{
			NGUITools.Destroy(mSpawnedActionCard.gameObject);
		}
		mSpawnedActionCards.Clear();
	}

	public void OnClickCloseResult()
	{
		Singleton<XpFusionController>.Instance.OnClickCloseResult();
	}

	public void UnzoomCard()
	{
		foreach (CardPrefabScript mSpawnedActionCard in mSpawnedActionCards)
		{
			mSpawnedActionCard.Unzoom();
		}
	}
}
