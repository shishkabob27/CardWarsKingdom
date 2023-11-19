using System.Collections.Generic;
using UnityEngine;

public class QuestSelectQuest : UIStreamingGridListItem
{
	public UITweenController UnlockTween;

	public UITweenController ResetTween;

	public UILabel Name;

	public UILabel Index;

	public UILabel Stamina;

	public UILabel MapCount;

	public UITexture OpponentPortrait;

	public Transform CreatureNodeParent;

	private Transform[] CreatureNodes;

	public GameObject Lock;

	public GameObject HideWhenLocked;

	public Transform LeaderPortraitParent;

	public GameObject BossBanner;

	public GameObject NewLabel;

	public GameObject[] Stars = new GameObject[3];

	public UILabel RandomRewardType;

	public float DelayToShowUnlockedElements = 2f;

	private List<GameObject> mSpawnedCreatures = new List<GameObject>();

	private bool mInitialzed;

	public QuestData Quest { get; set; }

	private void Initialize()
	{
		CreatureNodes = new Transform[MiscParams.CreaturesOnTeam];
		if (CreatureNodeParent != null)
		{
			for (int i = 0; i < MiscParams.CreaturesOnTeam; i++)
			{
				CreatureNodes[i] = CreatureNodeParent.FindChild("CreatureNode_" + (i + 1).ToString("D2"));
			}
		}
		mInitialzed = true;
	}

	public override void Populate(object dataObj)
	{
		if (!mInitialzed)
		{
			Initialize();
		}
		Quest = dataObj as QuestData;
		if (Quest.League.ID == "DailyRandom")
		{
			Name.text = Quest.LevelName;
			RandomRewardType.text = Quest.RandomDungeonReward.RewardLabel();
			return;
		}
		ResetTween.Play();
		Name.text = Quest.LevelName;
		Stamina.text = string.Empty;
		int num;
		if (Quest.League.QuestLine == QuestLineEnum.Main)
		{
			Index.text = Quest.ID;
			num = Singleton<PlayerInfoScript>.Instance.GetQuestStars(Quest.GetIntQuestId());
		}
		else
		{
			Index.text = string.Empty;
			num = Singleton<PlayerInfoScript>.Instance.GetDungeonStars(Quest.ID);
		}
		for (int i = 0; i < Stars.Length; i++)
		{
			Stars[i].SetActive(i < num);
		}
		LeaderData opponent = Quest.Opponent;
		Singleton<SLOTResourceManager>.Instance.QueueUITextureLoad(opponent.PortraitTexture, "FTUEBundle", "UI/UI/LoadingPlaceholder", OpponentPortrait);
		InventoryTile.ClearDelegates(true);
		InventorySlotItem[] preMatchCreatures = Quest.GetPreMatchCreatures();
		for (int j = 0; j < preMatchCreatures.Length; j++)
		{
			if (preMatchCreatures[j] != null)
			{
				GameObject gameObject = CreatureNodes[j].InstantiateAsChild(Singleton<PrefabReferences>.Instance.InventoryTile);
				gameObject.ChangeLayer(base.gameObject.layer);
				InventoryTile component = gameObject.GetComponent<InventoryTile>();
				component.Populate(preMatchCreatures[j]);
				mSpawnedCreatures.Add(gameObject);
			}
		}
		bool flag = Singleton<PlayerInfoScript>.Instance.IsQuestUnlocked(Quest);
		if (flag)
		{
			SetUnlockedState();
		}
		else
		{
			SetLockedState();
		}
		NewLabel.SetActive(flag && Quest.League.QuestLine == QuestLineEnum.Special && Singleton<PlayerInfoScript>.Instance.GetSpecialQuestStatus(Quest.ID) == SpecialQuestStatus.New);
		if (Singleton<QuestSelectController>.Instance.ShowingDungeonMapsLeague())
		{
			int num2 = Singleton<PlayerInfoScript>.Instance.SaveData.DungeonMaps[Quest.ID];
			MapCount.text = "x" + num2;
		}
		else
		{
			MapCount.text = string.Empty;
		}
	}

	public override void Unload()
	{
		base.Unload();
		foreach (GameObject mSpawnedCreature in mSpawnedCreatures)
		{
			NGUITools.Destroy(mSpawnedCreature);
		}
		if (OpponentPortrait != null)
		{
			OpponentPortrait.UnloadTexture();
		}
	}

	public void OnQuestClicked()
	{
		if (BossBanner != null)
		{
		}
		Singleton<QuestSelectController>.Instance.OnQuestClicked(this);
	}

	public void DelayedShowUnlockedElements()
	{
		SetLockedState();
		UnlockTween.Play();
		Invoke("ShowHideWhenLockedElements", DelayToShowUnlockedElements);
	}

	private void SetUnlockedState()
	{
		Lock.SetActive(false);
		HideWhenLocked.SetActive(true);
		LeaderPortraitParent.localPosition = new Vector3(-318f, -3f, 0f);
		LeaderPortraitParent.localScale = Vector3.one;
		BossBanner.SetActive(Quest.IsBossQuest());
	}

	private void SetLockedState()
	{
		Lock.SetActive(true);
		HideWhenLocked.SetActive(false);
		LeaderPortraitParent.localPosition = new Vector3(-296f, -3f, 0f);
		LeaderPortraitParent.localScale = Vector3.one * 0.75f;
		BossBanner.SetActive(false);
	}

	private void ShowHideWhenLockedElements()
	{
		HideWhenLocked.SetActive(true);
		LeaderPortraitParent.localPosition = new Vector3(-318f, -3f, 0f);
		LeaderPortraitParent.localScale = Vector3.one;
	}

	public void ShowFX()
	{
		if (BossBanner != null)
		{
			BossBanner.SetActive(Quest.IsBossQuest());
		}
	}

	public void SetStamina(bool reducedStamina)
	{
		int num = ((!reducedStamina) ? Quest.StaminaCost : Quest.ReducedStaminaCost);
		Stamina.text = num.ToString();
	}
}
