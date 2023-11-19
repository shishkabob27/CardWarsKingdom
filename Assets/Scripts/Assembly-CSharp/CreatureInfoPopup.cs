using System.Collections.Generic;
using UnityEngine;

public class CreatureInfoPopup : Singleton<CreatureInfoPopup>
{
	public Color StatBuffColor = Color.green;

	public Color StatDebuffColor = Color.red;

	public UITweenController ShowTween;

	public UITweenController HideTween;

	public UITweenController ShowEffectTween;

	public UITweenController HideEffectTween;

	public UILabel Name;

	public UILabel Level;

	public UITexture Image;

	public UIGrid RarityStarsParent;

	private Transform[] RarityStars = new Transform[5];

	public UILabel[] Stats = new UILabel[6];

	public Transform EffectDescPopup;

	public UILabel EffectName;

	public Transform EffectsParent;

	public GameObject EffectIconPrefab;

	public UIGrid EffectIconParentGrid;

	public GameObject StatusEffectDisplay;

	private List<GameObject> EffectIconPrefabList = new List<GameObject>();

	public UILabel Passive;

	public Transform CardsParent;

	private Transform[] CardSpawnPoints = new Transform[5];

	public Transform ExCardsParent;

	private Transform[] ExCardSpawnPoints = new Transform[3];

	public Collider ZoomCollider;

	public Transform ZoomPosition;

	public GameObject CycleArrowsGroup;

	public GameObject HelperLabel;

	public UILabel FactionName;

	public UISprite FactionIcon;

	public UILabel DragAttackCost;

	public int mShowingId = -1;

	private bool mShowing;

	private CreatureState mCreature;

	private CreatureItem mCreatureItem;

	private List<StatusIconItem> mCreatureEffectList;

	private List<CardPrefabScript> mSpawnedCards = new List<CardPrefabScript>();

	private List<CardPrefabScript> mSpawnedExCards = new List<CardPrefabScript>();

	private Color mStatBaseColor;

	private StatusData mShowingEffect;

	public void Awake()
	{
		mStatBaseColor = Stats[0].color;
		for (int i = 0; i < 5; i++)
		{
			RarityStars[i] = RarityStarsParent.transform.FindChild("RarityStar_" + (i + 1));
		}
		for (int j = 0; j < 5; j++)
		{
			CardSpawnPoints[j] = CardsParent.FindChild("ActionCardNode_" + (j + 1).ToString("D2"));
		}
		for (int k = 0; k < 3; k++)
		{
			ExCardSpawnPoints[k] = ExCardsParent.FindChild("ExpansionCardSpawn_" + (k + 1).ToString("D2"));
		}
	}

	private void GridRepos()
	{
	}

	public void Show(CreatureState creature)
	{
		mShowing = true;
		Singleton<HandCardController>.Instance.HideHand();
		Singleton<HandCardController>.Instance.HideOpponentTween.Play();
		Singleton<BattleHudController>.Instance.HideHudForInfoPopupTween.Play();
		ShowTween.Play();
		mCreature = creature;
		mCreatureItem = creature.Data;
		CycleArrowsGroup.SetActive(Singleton<DWGame>.Instance.CurrentBoardState.GetPlayerState(creature.Owner.Type).GetCreatureCount() > 1);
		Populate();
	}

	public void Show(CreatureItem creature)
	{
		mShowing = true;
		Singleton<HandCardController>.Instance.HideHand();
		Singleton<HandCardController>.Instance.HideOpponentTween.Play();
		Singleton<BattleHudController>.Instance.HideHudForInfoPopupTween.Play();
		if (Singleton<DWGame>.Instance.InDeploymentPhase())
		{
			Singleton<BattleHudController>.Instance.HideDeployTween.Play();
		}
		ShowTween.Play();
		mCreature = null;
		mCreatureItem = creature;
		CycleArrowsGroup.SetActive(false);
		Populate();
	}

	private void Populate()
	{
		if (mCreature != null)
		{
			CreatureData form = mCreatureItem.Form;
			Name.text = form.Name;
			Level.text = KFFLocalization.Get("!!LV") + " " + mCreatureItem.Level;
			for (int i = 0; i < RarityStars.Length; i++)
			{
				RarityStars[i].gameObject.SetActive(i < mCreatureItem.StarRating);
			}
			RarityStarsParent.Reposition();
			Passive.text = mCreatureItem.BuildPassiveDescriptionString(true);
			if (DragAttackCost != null)
			{
				DragAttackCost.text = mCreatureItem.Form.AttackCost.ToString();
			}
			for (int j = 0; j < Stats.Length; j++)
			{
				string result;
				int boost;
				mCreature.GetStatString((CreatureStat)j, out result, out boost);
				Stats[j].text = result;
				if (boost > 0)
				{
					Stats[j].color = StatBuffColor;
				}
				else if (boost < 0)
				{
					Stats[j].color = StatDebuffColor;
				}
				else
				{
					Stats[j].color = mStatBaseColor;
				}
			}
			for (int k = 0; k < CardSpawnPoints.Length; k++)
			{
				GameObject gameObject = CardSpawnPoints[k].InstantiateAsChild(Singleton<PrefabReferences>.Instance.Card);
				gameObject.ChangeLayer(base.gameObject.layer);
				CardPrefabScript component = gameObject.GetComponent<CardPrefabScript>();
				mSpawnedCards.Add(component);
				component.Mode = CardPrefabScript.CardMode.BattlePopup;
				component.OpponentInfoPopup = mCreature.Owner.Type == PlayerType.Opponent;
				component.Populate(mCreatureItem.Form.ActionCards[k]);
				component.AdjustDepth(k + 9);
			}
			for (int l = 0; l < ExCardSpawnPoints.Length; l++)
			{
				if (mCreatureItem.ExCards[l] != null)
				{
					GameObject gameObject2 = ExCardSpawnPoints[l].InstantiateAsChild(Singleton<PrefabReferences>.Instance.Card);
					gameObject2.ChangeLayer(base.gameObject.layer);
					CardPrefabScript component2 = gameObject2.GetComponent<CardPrefabScript>();
					mSpawnedExCards.Add(component2);
					component2.Mode = CardPrefabScript.CardMode.BattlePopup;
					component2.OpponentInfoPopup = mCreature.Owner.Type == PlayerType.Opponent;
					component2.Populate(mCreatureItem.ExCards[l].Card.Form);
					component2.AdjustDepth(l + 9);
				}
			}
			CreatureBuffBar buffBar = Singleton<BattleHudController>.Instance.GetBuffBar(mCreature);
			mCreatureEffectList = buffBar.GetActivePersistentFXList();
			bool flag = mCreatureEffectList.Count > 7;
			int num = 0;
			foreach (StatusIconItem mCreatureEffect in mCreatureEffectList)
			{
				GameObject gameObject3 = EffectIconParentGrid.transform.InstantiateAsChild(EffectIconPrefab);
				EffectIconPrefabList.Add(gameObject3);
				gameObject3.GetComponent<CreatureInfoEffectPrefabScript>().id = num;
				num++;
				StatusIconItem component3 = gameObject3.GetComponent<StatusIconItem>();
				component3.Populate(mCreatureEffect.Status, mCreature);
				gameObject3.GetComponent<Collider>().enabled = true;
				gameObject3.GetComponent<UIDragScrollView>().enabled = flag;
			}
			bool flag2 = false;
			if (Passive != null)
			{
				flag2 = !string.IsNullOrEmpty(Passive.text);
				Passive.gameObject.SetActive(flag2);
			}
			bool active = mCreatureEffectList.Count > 0;
			if (StatusEffectDisplay != null)
			{
				StatusEffectDisplay.SetActive(active);
				StatusEffectDisplay.transform.localPosition = ((!flag2) ? new Vector3(145f, -150f, 0f) : new Vector3(145f, -227f, 0f));
			}
			EffectIconParentGrid.Reposition();
			HelperLabel.SetActive(Singleton<PlayerInfoScript>.Instance.StateData.HelperCreature != null && Singleton<PlayerInfoScript>.Instance.StateData.HelperCreature.Creature == mCreature.Data);
		}
		else
		{
			CreatureData form2 = mCreatureItem.Form;
			Name.text = form2.Name;
			Level.text = KFFLocalization.Get("!!LV") + " " + mCreatureItem.Level;
			for (int m = 0; m < RarityStars.Length; m++)
			{
				RarityStars[m].gameObject.SetActive(m < mCreatureItem.StarRating);
			}
			RarityStarsParent.Reposition();
			bool flag3 = false;
			if (Passive != null)
			{
				Passive.text = mCreatureItem.BuildPassiveDescriptionString(true);
				flag3 = !string.IsNullOrEmpty(Passive.text);
				Passive.gameObject.SetActive(flag3);
			}
			if (StatusEffectDisplay != null)
			{
				StatusEffectDisplay.SetActive(false);
			}
			if (DragAttackCost != null)
			{
				DragAttackCost.text = mCreatureItem.Form.AttackCost.ToString();
			}
			for (int n = 0; n < Stats.Length; n++)
			{
				Stats[n].text = mCreatureItem.GetStat((CreatureStat)n) + ((!((CreatureStat)n).IsPercent()) ? string.Empty : "%");
			}
			for (int num2 = 0; num2 < CardSpawnPoints.Length; num2++)
			{
				GameObject gameObject4 = CardSpawnPoints[num2].InstantiateAsChild(Singleton<PrefabReferences>.Instance.Card);
				gameObject4.ChangeLayer(base.gameObject.layer);
				CardPrefabScript component4 = gameObject4.GetComponent<CardPrefabScript>();
				mSpawnedCards.Add(component4);
				component4.Mode = CardPrefabScript.CardMode.BattlePopup;
				component4.Populate(mCreatureItem.Form.ActionCards[num2]);
				component4.AdjustDepth(num2 + 9);
			}
			for (int num3 = 0; num3 < ExCardSpawnPoints.Length; num3++)
			{
				if (mCreatureItem.ExCards[num3] != null)
				{
					GameObject gameObject5 = ExCardSpawnPoints[num3].InstantiateAsChild(Singleton<PrefabReferences>.Instance.Card);
					gameObject5.ChangeLayer(base.gameObject.layer);
					CardPrefabScript component5 = gameObject5.GetComponent<CardPrefabScript>();
					mSpawnedExCards.Add(component5);
					component5.Mode = CardPrefabScript.CardMode.BattlePopup;
					component5.Populate(mCreatureItem.ExCards[num3].Card.Form);
					component5.AdjustDepth(num3 + 9);
				}
			}
			HelperLabel.SetActive(Singleton<PlayerInfoScript>.Instance.StateData.HelperCreature != null && Singleton<PlayerInfoScript>.Instance.StateData.HelperCreature.Creature == mCreatureItem);
		}
		FactionIcon.spriteName = mCreatureItem.Form.Faction.IconTexture();
		FactionName.text = mCreatureItem.Form.GetClassString();
		if (mCreatureItem.Form.BattleZoomSound != null)
		{
			Singleton<SLOTAudioManager>.Instance.PlaySound(mCreatureItem.Form.BattleZoomSound);
		}
	}

	public void Unload()
	{
		foreach (CardPrefabScript mSpawnedCard in mSpawnedCards)
		{
			NGUITools.Destroy(mSpawnedCard.gameObject);
		}
		mSpawnedCards.Clear();
		foreach (CardPrefabScript mSpawnedExCard in mSpawnedExCards)
		{
			NGUITools.Destroy(mSpawnedExCard.gameObject);
		}
		mSpawnedExCards.Clear();
		EffectIconParentGrid.transform.DestroyAllChildren();
		EffectIconPrefabList.Clear();
	}

	public void HideIfShowing()
	{
		if (mShowing)
		{
			ExitCreatureDetail();
		}
	}

	public void ExitCreatureDetail()
	{
		HideTween.Play();
		Singleton<HandCardController>.Instance.UnzoomCard();
		OnEffectReleased();
		Singleton<HandCardController>.Instance.ShowHand();
		Singleton<HandCardController>.Instance.ShowOpponentTween.Play();
		Singleton<BattleHudController>.Instance.ShowHudAfterInfoPopupTween.Play();
		Singleton<DWGameCamera>.Instance.RenderP1Character(false);
		if (Singleton<DWGameCamera>.Instance.UseDetailCam)
		{
			Singleton<DWGameCamera>.Instance.SetCreatureDetailCam(false);
		}
		else
		{
			Singleton<DWGameCamera>.Instance.MoveCameraToP1Setup(true);
		}
		mShowing = false;
		if (!Singleton<TutorialController>.Instance.IsBlockActive("IntroBattle") && Singleton<DWGame>.Instance.InDeploymentPhase())
		{
			Singleton<BattleHudController>.Instance.ShowDeployTween.Play();
		}
	}

	public void OnGem1Pressed()
	{
		OnGemPressed(0);
	}

	public void OnGem2Pressed()
	{
		OnGemPressed(1);
	}

	public void OnGem3Pressed()
	{
		OnGemPressed(2);
	}

	private void OnGemPressed(int index)
	{
	}

	public void OnGemReleased()
	{
	}

	private void Update()
	{
		if (mShowing && (Singleton<TutorialController>.Instance.IsStateActive("Q3_TapUtil") || Singleton<TutorialController>.Instance.IsStateActive("Q3_ComboDesc")))
		{
			UnzoomCard();
			ExitCreatureDetail();
		}
	}

	public void OnEffectPressed()
	{
		UnzoomCard();
		OnEffectIconPressed(UIEventTrigger.current.gameObject);
	}

	private void OnEffectIconPressed(GameObject clickedObject)
	{
		StatusIconItem component = clickedObject.GetComponent<StatusIconItem>();
		if (component.Status == mShowingEffect)
		{
			return;
		}
		Singleton<CreatureInfoPopup>.Instance.ZoomCollider.gameObject.SetActive(true);
		mShowingEffect = component.Status;
		ShowEffectTween.Play();
		KeyWordData keyword = component.Status.FXData.Keyword;
		if (keyword != null)
		{
			string text = component.Status.GetValueString(mCreature, true);
			if (text != string.Empty)
			{
				text = " " + text;
			}
			EffectName.text = keyword.DisplayName + text + "\n" + keyword.Description;
		}
		else
		{
			EffectName.text = "Missing keyword for Status ID " + component.Status.ID;
		}
	}

	public void OnEffectReleased()
	{
		if (mShowingEffect != null)
		{
			Singleton<CreatureInfoPopup>.Instance.ZoomCollider.gameObject.SetActive(false);
			mShowingId = -1;
			mShowingEffect = null;
			HideEffectTween.Play();
		}
	}

	public void OnEnchantIconReleased()
	{
	}

	public void UnzoomCard()
	{
		foreach (CardPrefabScript mSpawnedCard in mSpawnedCards)
		{
			mSpawnedCard.Unzoom();
		}
		foreach (CardPrefabScript mSpawnedExCard in mSpawnedExCards)
		{
			mSpawnedExCard.Unzoom();
		}
		OnEffectReleased();
	}

	public List<CardPrefabScript> GetCards()
	{
		return mSpawnedCards;
	}

	public CardPrefabScript GetZoomedCard()
	{
		foreach (CardPrefabScript mSpawnedCard in mSpawnedCards)
		{
			if (mSpawnedCard.ZoomedIn())
			{
				return mSpawnedCard;
			}
		}
		return null;
	}

	public void OnClickCycleLeft()
	{
		CycleCreature(-1);
	}

	public void OnClickCycleRight()
	{
		CycleCreature(1);
	}

	private void CycleCreature(int direction)
	{
		UnzoomCard();
		Unload();
		int creatureCount = Singleton<DWGame>.Instance.CurrentBoardState.GetPlayerState(mCreature.Owner.Type).GetCreatureCount();
		int num = mCreature.Lane.Index + direction;
		if (num < 0)
		{
			num = creatureCount - 1;
		}
		else if (num >= creatureCount)
		{
			num = 0;
		}
		mCreature = Singleton<DWGame>.Instance.CurrentBoardState.GetPlayerState(mCreature.Owner.Type).GetCreature(num);
		mCreatureItem = mCreature.Data;
		Singleton<DWGameCamera>.Instance.MoveCameraToCreatureDetail(mCreature, true);
		Populate();
	}
}
