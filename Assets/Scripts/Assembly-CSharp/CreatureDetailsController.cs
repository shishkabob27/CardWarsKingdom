using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureDetailsController : Singleton<CreatureDetailsController>
{
	public delegate void ClosedDelegate(InventorySlotItem creature);

	public int NoGemsPanelShrinkage;

	public UITweenController ShowTween;

	public UITweenController HideTween;

	public UITweenController ShowCardsTween;

	public UITweenController ShowCreatureTween;

	public UITweenController HideCreatureTween;

	public UITweenController ShowPassiveTween;

	public UITweenController HidePassiveTween;

	public GameObject MainPanel;

	public CreatureStatsPanel StatsPanel;

	public Transform MainModelParent;

	public Transform SpawnNodeScaler;

	public UIStreamingGrid MainCreatureGrid;

	private UIStreamingGridDataSource<InventorySlotItem> mMainCreatureGridDataSource = new UIStreamingGridDataSource<InventorySlotItem>();

	public Transform SkillCardNodeParent;

	public Collider ZoomCollider;

	public Transform ZoomPosition;

	public GameObject GemsParent;

	public Transform StatsPanelTransform;

	public Transform StatsButtonTransform;

	public Transform WeightButtonTransform;

	public UISprite StatsPanelBackground;

	public GameObject FavoriteParent;

	public GameObject FavoriteCheckmark;

	public GameObject ExCardsParent;

	public Camera CreatureCamera;

	private InventorySlotItem mCreature;

	private GameObject mLoadedModelInstance;

	private bool isFirstTimeShow;

	private GameObject[] SkillCardNodes = new GameObject[5];

	private List<CardPrefabScript> mSpawnedActionCards = new List<CardPrefabScript>();

	public Transform[] ExtraCardSlots = new Transform[3];

	public GameObject[] ExtraCardSlotLocks = new GameObject[3];

	private List<CardPrefabScript> mSpawnedExCards = new List<CardPrefabScript>();

	private ClosedDelegate mClosedDelegate;

	private Vector3 mBaseStatsPanelPos;

	private int mBaseStatsPanelBGHeight;

	private bool mEnableCardEditing;

	private bool isCreatureModelShown;

	private bool isActionCardShown;

	private string shownActionCardName;

	private bool isClosing;

	public void Awake()
	{
		mBaseStatsPanelPos = StatsPanelTransform.localPosition;
		mBaseStatsPanelBGHeight = StatsPanelBackground.height;
		for (int i = 0; i < 5; i++)
		{
			SkillCardNodes[i] = SkillCardNodeParent.FindChild("ActionCardSpawn_0" + (i + 1)).gameObject;
		}
	}

	public void Update()
	{
		if (!isCreatureModelShown)
		{
			bool flag = true;
			if (isActionCardShown)
			{
				flag = false;
			}
			if (Singleton<SimplePopupController>.Instance.MainPanel.activeInHierarchy)
			{
				flag = false;
			}
			if (DetachedSingleton<SceneFlowManager>.Instance.InFrontEnd() && Singleton<CardEquipController>.Instance.MainPanel.activeInHierarchy)
			{
				flag = false;
			}
			if (isClosing)
			{
				flag = false;
			}
			if (flag)
			{
				ShowCreatureModel();
			}
		}
		else
		{
			AdjustCameraDepth();
		}
	}

	public void ShowCreature(InventorySlotItem creature, ClosedDelegate onClosed, bool enableCardEditing)
	{
		ShowTween.Play();
		mCreature = creature;
		mClosedDelegate = onClosed;
		mEnableCardEditing = enableCardEditing;
		if (creature.Creature.Form.ZoomSound != null)
		{
			Singleton<SLOTAudioManager>.Instance.PlaySound(creature.Creature.Form.ZoomSound);
		}
		PopulateCurrentCreature();
	}

	public void ShowCollectionCreature(CreatureData data, ClosedDelegate onClosed)
	{
		CreatureItem creatureItem = new CreatureItem(data);
		creatureItem.IsCollectionDummy = true;
		creatureItem.Xp = 0;
		InventorySlotItem creature = new InventorySlotItem(creatureItem);
		ShowCreature(creature, onClosed, false);
	}

	private void PopulateCurrentCreature()
	{
		isCreatureModelShown = true;
		isActionCardShown = false;
		shownActionCardName = null;
		isClosing = false;
		SpawnNodeScaler.localScale = Vector3.one;
		StatsPanel.Populate(mCreature);
		if (mCreature.Creature.Form.HasPassiveAbility())
		{
			ShowPassiveTween.Play();
		}
		else
		{
			HidePassiveTween.Play();
		}
		ExCardsParent.SetActive(mEnableCardEditing && DetachedSingleton<SceneFlowManager>.Instance.InFrontEnd() && Singleton<PlayerInfoScript>.Instance.CanEquipExCards());
		MainModelParent.transform.localRotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
		for (int i = 0; i < 5; i++)
		{
			if (mCreature.Creature.ActionCards[i] != null)
			{
				GameObject gameObject = SkillCardNodes[i].InstantiateAsChild(Singleton<PrefabReferences>.Instance.Card);
				gameObject.ChangeLayer(SkillCardNodes[i].layer);
				CardPrefabScript cardScript2 = gameObject.GetComponent<CardPrefabScript>();
				cardScript2.Mode = CardPrefabScript.CardMode.GeneralFrontEnd;
				cardScript2.OpponentInfoPopup = mCreature.Creature.EnemyLoadoutCreature;
				cardScript2.Populate(mCreature.Creature.Form.ActionCards[i]);
				cardScript2.AdjustDepth(i + 1);
				cardScript2.SetCardState(CardPrefabScript.HandCardState.InHand);
				mSpawnedActionCards.Add(cardScript2);
				cardScript2.GetComponent<UIEventTrigger>().onClick.Add(new EventDelegate(delegate
				{
					HideCreatureModel(true, cardScript2.Card.Name);
				}));
			}
		}
		for (int j = 0; j < 3; j++)
		{
			if (j < mCreature.Creature.ExCardSlotsUnlocked)
			{
				ExtraCardSlotLocks[j].SetActive(false);
				if (mCreature.Creature.ExCards[j] != null)
				{
					ExtraCardSlots[j].gameObject.SetActive(true);
					GameObject gameObject2 = ExtraCardSlots[j].InstantiateAsChild(Singleton<PrefabReferences>.Instance.Card);
					gameObject2.ChangeLayer(ExtraCardSlots[j].gameObject.layer);
					CardPrefabScript cardScript = gameObject2.GetComponent<CardPrefabScript>();
					cardScript.Mode = CardPrefabScript.CardMode.GeneralFrontEnd;
					cardScript.OpponentInfoPopup = mCreature.Creature.EnemyLoadoutCreature;
					cardScript.Populate(mCreature.Creature.ExCards[j].Card.Form);
					cardScript.AdjustDepth(j + 1);
					cardScript.SetCardState(CardPrefabScript.HandCardState.InHand);
					mSpawnedExCards.Add(cardScript);
					cardScript.GetComponent<UIEventTrigger>().onClick.Add(new EventDelegate(delegate
					{
						HideCreatureModel(true, cardScript.Card.Name);
					}));
				}
			}
			else
			{
				ExtraCardSlotLocks[j].SetActive(true);
			}
		}
		StartCoroutine(LoadModelCo());
		if (Singleton<PlayerInfoScript>.Instance.SaveData.FindCreature((InventorySlotItem m) => m == mCreature) != null)
		{
			FavoriteParent.SetActive(true);
			FavoriteCheckmark.SetActive(mCreature.Creature.Favorite);
		}
		else
		{
			FavoriteParent.SetActive(false);
		}
	}

	private void AdjustCameraDepth()
	{
		if (!CreatureCamera.isActiveAndEnabled || !(mLoadedModelInstance != null))
		{
			return;
		}
		Renderer[] componentsInChildren = mLoadedModelInstance.GetComponentsInChildren<Renderer>();
		bool flag = false;
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i] != null && componentsInChildren[i].isVisible)
			{
				flag = true;
			}
		}
		if (flag)
		{
			CreatureCamera.fieldOfView = 30f;
		}
		else
		{
			CreatureCamera.fieldOfView = 45f;
		}
	}

	private IEnumerator LoadModelCo()
	{
		UnloadModel();
		yield return StartCoroutine(Singleton<SLOTResourceManager>.Instance.LoadCreatureResources(mCreature.Creature.Form, delegate(Object objData, Texture2D texture)
		{
			mLoadedModelInstance = MainModelParent.InstantiateAsChild((GameObject)objData);
			mLoadedModelInstance.ChangeLayerToParent();
			mCreature.Creature.Form.SwapCreatureTexture(mLoadedModelInstance, texture, true);
			float num = Mathf.Max(4f, mCreature.Creature.Form.Height);
			float a = Mathf.Min(1f, 5f / num);
			float num2 = Mathf.Max(4f, mCreature.Creature.Form.Width);
			float b = Mathf.Min(1f, 5f / num2);
			float num3 = Mathf.Min(a, b);
			mLoadedModelInstance.transform.localScale = Vector3.one * num3;
		}));
		if (mCreature.Creature.Form.InitNumOfLoad > 0)
		{
			isFirstTimeShow = mCreature.Creature.Form.InitNumOfLoad <= 1;
			isCreatureModelShown = false;
			ShowCreatureModel();
		}
		else
		{
			isFirstTimeShow = true;
			isCreatureModelShown = true;
		}
		if (DetachedSingleton<SceneFlowManager>.Instance.InBattleScene())
		{
			Camera thisCamera = GetComponentInChildren<Camera>();
			if (thisCamera != null)
			{
				thisCamera.depth = 32f;
			}
		}
	}

	private void UnloadModel()
	{
		if (mLoadedModelInstance != null)
		{
			Object.Destroy(mLoadedModelInstance);
			mLoadedModelInstance = null;
			Resources.UnloadUnusedAssets();
		}
	}

	public void OnClickClose()
	{
		InventorySlotItem creature = mCreature;
		mCreature = null;
		if (mClosedDelegate != null)
		{
			ClosedDelegate closedDelegate = mClosedDelegate;
			mClosedDelegate = null;
			closedDelegate(creature);
		}
		isClosing = true;
		HideCreatureModel();
	}

	public void Unload()
	{
		for (int i = 0; i < mSpawnedActionCards.Count; i++)
		{
			if (mSpawnedActionCards[i] != null)
			{
				NGUITools.Destroy(mSpawnedActionCards[i].gameObject);
				mSpawnedActionCards[i] = null;
			}
		}
		for (int j = 0; j < mSpawnedExCards.Count; j++)
		{
			if (mSpawnedExCards[j] != null)
			{
				NGUITools.Destroy(mSpawnedExCards[j].gameObject);
				mSpawnedExCards[j] = null;
			}
		}
		mMainCreatureGridDataSource.Clear();
		UnloadModel();
		StatsPanel.Unload();
	}

	public void OnClickEquipGems()
	{
	}

	public void OnCreatureClicked(InventorySlotItem creature)
	{
		mCreature = creature;
		PopulateCurrentCreature();
		if (ShowCardsTween != null)
		{
			ShowCardsTween.Play();
		}
	}

	public void UnzoomCard()
	{
		foreach (CardPrefabScript mSpawnedActionCard in mSpawnedActionCards)
		{
			if (mSpawnedActionCard != null)
			{
				mSpawnedActionCard.Unzoom();
			}
		}
		foreach (CardPrefabScript mSpawnedExCard in mSpawnedExCards)
		{
			if (mSpawnedExCard != null)
			{
				mSpawnedExCard.Unzoom();
			}
		}
	}

	public void OnClickFavorite()
	{
		mCreature.Creature.Favorite = !mCreature.Creature.Favorite;
		FavoriteCheckmark.SetActive(mCreature.Creature.Favorite);
	}

	public void OnClickExCards()
	{
		HideTween.Play();
		isClosing = true;
		HideCreatureModel();
		Singleton<CardEquipController>.Instance.Show(mCreature, OnCloseCardEquip);
	}

	private void OnCloseCardEquip()
	{
		ShowTween.Play();
		PopulateCurrentCreature();
	}

	public void OnClickStatsDetails()
	{
		Singleton<StatsDescriptionPopup>.Instance.ShowPanelAtTransformPosition(StatsButtonTransform);
	}

	public void OnClickWeight()
	{
		Singleton<StatsDescriptionPopup>.Instance.ShowWeightPanelAtTransformPosition(WeightButtonTransform);
	}

	public void HideCreatureModel(bool calledByActionCard = false, string actionCardName = null)
	{
		if (calledByActionCard)
		{
			isActionCardShown = true;
			if (shownActionCardName != null && actionCardName != null && shownActionCardName == actionCardName)
			{
				isActionCardShown = false;
				shownActionCardName = null;
				return;
			}
			if (actionCardName != null)
			{
				shownActionCardName = actionCardName;
			}
		}
		if (isCreatureModelShown)
		{
			HideCreatureTween.Play();
			isCreatureModelShown = false;
		}
	}

	public void ShowCreatureModel(bool calledByActionCard = false)
	{
		if (calledByActionCard)
		{
			isActionCardShown = false;
		}
		else if (!isCreatureModelShown)
		{
			ShowCreatureTween.Play();
			isCreatureModelShown = true;
			shownActionCardName = null;
		}
	}

	public void FinishedShowCreatureModel()
	{
		if (mLoadedModelInstance != null && !isFirstTimeShow)
		{
			isFirstTimeShow = true;
			isCreatureModelShown = false;
			ShowCreatureModel();
		}
	}
}
