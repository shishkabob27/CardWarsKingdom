using UnityEngine;

public class CreatureStatsPanel : MonoBehaviour
{
	public UILabel InfoName;

	public UILabel InfoDetails;

	public UISprite InfoFactionIcon;

	public Transform InfoRarityStarsParent;

	public Transform[] InfoRarityStars = new Transform[5];

	public UILabel InfoXPCount;

	public UILabel InfoXPPercent;

	public UISprite InfoXPBar;

	public UILabel InfoLevel;

	public UILabel InfoLevelNumber;

	public UILabel[] InfoStats = new UILabel[6];

	public UILabel InfoType;

	public UILabel InfoPassiveLevel;

	public UILabel InfoPassiveSkill;

	public Transform PortraitSpawnNode;

	public UILabel TeamCost;

	public UILabel AttackCost;

	public GameObject MaxLevelToggle;

	public UILabel MaxOrCurrentLabel;

	public bool ShouldTweenInRarityStars;

	public AnimationCurve TweenInStarCurve;

	private bool mShowMaxLevel;

	private GameObject mSpawnedCreaturePortrait;

	public UITweenController[] PulseStarTweens;

	public UITweenController[] UnPulseStarTweens;

	public InventorySlotItem CreatureItem { get; private set; }

	public InventoryTile InventoryTile { get; private set; }

	public void Populate(InventorySlotItem creatureSlot, bool maxLevel = false)
	{
		Unload();
		mShowMaxLevel = maxLevel;
		Singleton<SLOTAudioManager>.Instance.PlaySound("ui/SFX_GachaBanner");
		if (MaxLevelToggle != null)
		{
			SetLabelText(MaxOrCurrentLabel, KFFLocalization.Get((!mShowMaxLevel) ? "!!CURRENT_STATS" : "!!MAX_STATS"));
		}
		CreatureItem = creatureSlot;
		CreatureItem creature = creatureSlot.Creature;
		creature.Form.ParseKeywords();
		SetLabelText(InfoName, creature.Form.Name);
		if (InfoFactionIcon != null)
		{
			InfoFactionIcon.spriteName = creature.Form.Faction.IconTexture();
		}
		int num = creature.Xp;
		int passiveSkillLevel = creature.PassiveSkillLevel;
		int starRating = creature.StarRating;
		if (mShowMaxLevel)
		{
			creature.StarRating = CreatureStarRatingDataManager.Instance.MaxStarRating();
			creature.Xp = creature.XPTable.GetXpToReachLevel(creature.MaxLevel);
			if (creature.Form.PassiveData != null)
			{
				creature.PassiveSkillLevel = creature.Form.PassiveData.MaxLevel;
			}
		}
		XPLevelData levelData = creature.GetLevelData();
		levelData.PopulateUI(true, InfoLevel, InfoXPCount, InfoXPPercent, InfoXPBar);
		SetLabelText(InfoLevelNumber, levelData.mCurrentLevel.ToString());
		for (int i = 0; i < InfoStats.Length; i++)
		{
			SetLabelText(InfoStats[i], creature.GetStat((CreatureStat)i) + ((!((CreatureStat)i).IsPercent()) ? string.Empty : "%"));
		}
		SetLabelText(InfoDetails, creature.DetailsFormatString());
		SetLabelText(InfoPassiveLevel, creature.PassiveLevelDetailFormatString());
		SetLabelText(InfoPassiveSkill, creature.BuildPassiveDescriptionString(false));
		UpdateStars(creature);
		SetLabelText(TeamCost, creature.currentTeamCost.ToString());
		SetLabelText(AttackCost, creature.Form.AttackCost.ToString());
		creature.Xp = num;
		creature.PassiveSkillLevel = passiveSkillLevel;
		creature.StarRating = starRating;
		SetLabelText(InfoType, creature.Form.GetClassString());
		if (PortraitSpawnNode != null)
		{
			mSpawnedCreaturePortrait = PortraitSpawnNode.InstantiateAsChild(Singleton<PrefabReferences>.Instance.InventoryTile);
			mSpawnedCreaturePortrait.ChangeLayer(base.gameObject.layer);
			InventoryTile = mSpawnedCreaturePortrait.GetComponent<InventoryTile>();
			InventoryTile.Populate(creatureSlot);
			InventoryTile.SetAsDisplayOnly();
			InventoryTile.ShowRarityFrame();
		}
	}

	private void OnEnable()
	{
		RepositionRarityStarsGrid();
	}

	private void RepositionRarityStarsGrid()
	{
		if (InfoRarityStarsParent != null)
		{
			UIGrid component = InfoRarityStarsParent.GetComponent<UIGrid>();
			if (component != null && component.pivot == UIWidget.Pivot.Center)
			{
				component.Reposition();
			}
		}
	}

	private void GetInfoRarityStars()
	{
		if (InfoRarityStarsParent != null)
		{
			for (int i = 0; i < 5; i++)
			{
				InfoRarityStars[i] = InfoRarityStarsParent.Find("Icon_RarityStar_" + (i + 1));
			}
		}
	}

	private void UpdateStars(CreatureItem creature)
	{
		if (!(InfoRarityStarsParent != null))
		{
			return;
		}
		GetInfoRarityStars();
		for (int i = 0; i < 5; i++)
		{
			bool active = i < creature.StarRating;
			if (InfoRarityStars[i].GetComponent<UISprite>() != null)
			{
				UISprite component = InfoRarityStars[i].GetComponent<UISprite>();
				component.gameObject.SetActive(active);
				component.alpha = ((!ShouldTweenInRarityStars) ? 1 : 0);
				component.transform.localScale = Vector3.one;
			}
		}
		RepositionRarityStarsGrid();
	}

	public void TweenInRarityStars(float inDelayUntilStart = 0f)
	{
		if (!ShouldTweenInRarityStars)
		{
			return;
		}
		float num = 0.2f;
		GetInfoRarityStars();
		for (int i = 0; i < 5; i++)
		{
			UISprite component = InfoRarityStars[i].GetComponent<UISprite>();
			TweenAlpha component2 = component.GetComponent<TweenAlpha>();
			if (component2 != null)
			{
				Object.Destroy(component2);
			}
			TweenScale component3 = component.GetComponent<TweenScale>();
			if (component3 != null)
			{
				Object.Destroy(component3);
			}
			if (component.gameObject.activeSelf)
			{
				TweenAlpha tweenAlpha = component.gameObject.AddComponent<TweenAlpha>();
				tweenAlpha.duration = 0.15f;
				tweenAlpha.from = 0f;
				tweenAlpha.to = 1f;
				tweenAlpha.delay = inDelayUntilStart + (float)i * num;
				tweenAlpha.PlayForward();
				TweenScale tweenScale = component.gameObject.AddComponent<TweenScale>();
				tweenScale.duration = 0.25f;
				tweenScale.from = Vector3.one * 4f;
				tweenScale.to = Vector3.one;
				tweenScale.delay = inDelayUntilStart + (float)i * num;
				if (TweenInStarCurve != null)
				{
					tweenScale.animationCurve = TweenInStarCurve;
				}
				tweenScale.PlayForward();
			}
		}
	}

	public void PreviewStarUpgrade()
	{
		StopPreviewStarUpgrade();
		int starRating = CreatureItem.Creature.StarRating;
		bool active = true;
		for (int i = 0; i < InfoRarityStars.Length; i++)
		{
			InfoRarityStars[i].gameObject.SetActive(active);
			if (i == starRating - 1)
			{
				active = false;
			}
			if (i == starRating)
			{
				PulseStarTweens[i].Play();
			}
		}
		RepositionRarityStarsGrid();
	}

	public void StopPreviewStarUpgrade()
	{
		for (int i = 0; i < PulseStarTweens.Length; i++)
		{
			if (i < 5)
			{
				PulseStarTweens[i].StopAndReset();
			}
		}
		for (int j = 0; j < UnPulseStarTweens.Length; j++)
		{
			if (j < 5)
			{
				UnPulseStarTweens[j].Play();
			}
		}
		UpdateStars(CreatureItem.Creature);
	}

	public void Unload()
	{
		if (mSpawnedCreaturePortrait != null)
		{
			NGUITools.Destroy(mSpawnedCreaturePortrait);
		}
		mSpawnedCreaturePortrait = null;
		Transform[] infoRarityStars = InfoRarityStars;
		foreach (Transform transform in infoRarityStars)
		{
			if (transform != null)
			{
				transform.gameObject.SetActive(false);
			}
		}
		EmptyLabel(InfoName);
		EmptyLabel(InfoDetails);
		EmptyLabel(InfoXPCount);
		EmptyLabel(InfoXPPercent);
		EmptyLabel(InfoLevel);
		EmptyLabel(InfoLevelNumber);
		EmptyLabel(InfoType);
		EmptyLabel(InfoPassiveLevel);
		EmptyLabel(InfoPassiveSkill);
		EmptyLabel(TeamCost);
		EmptyLabel(AttackCost);
		UILabel[] infoStats = InfoStats;
		foreach (UILabel labelToEmpty in infoStats)
		{
			EmptyLabel(labelToEmpty);
		}
		if (InfoFactionIcon != null)
		{
			InfoFactionIcon.spriteName = null;
		}
	}

	public void SetLabelText(UILabel labelToSet, string newText)
	{
		if (labelToSet != null)
		{
			labelToSet.text = newText;
		}
	}

	public void EmptyLabel(UILabel labelToEmpty)
	{
		if (labelToEmpty != null)
		{
			labelToEmpty.text = string.Empty;
		}
	}

	public void ToggleMaxLevel()
	{
		Populate(CreatureItem, !mShowMaxLevel);
	}

	public Transform GetHighestStar()
	{
		return InfoRarityStars[CreatureItem.Creature.StarRating - 1];
	}

	public void OnClickStats()
	{
		Singleton<StatsDescriptionPopup>.Instance.ShowPanel();
	}

	public void OnClickWeight()
	{
		Singleton<StatsDescriptionPopup>.Instance.ShowWeightPanel();
	}
}
