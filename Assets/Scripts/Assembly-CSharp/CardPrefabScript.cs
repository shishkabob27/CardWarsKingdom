using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPrefabScript : UIStreamingGridListItem
{
	public enum CardMode
	{
		Hand,
		GeneralFrontEnd,
		BattlePopup,
		Opponent,
		Loot
	}

	public enum HandCardState
	{
		FastDrawing,
		FailedDrawing,
		InHand,
		ZoomingOutToGrid,
		ZoomedIn,
		DraggingInHand,
		DraggingOnBoard,
		Discarding,
		ManualDiscarding,
		Playing,
		OpponentPlaying,
		CreatureDeployAnim,
		LogZoom,
		ClosingLogZoom
	}

	public CardMode PrintMode;

	public HandCardState PrintState;

	public float KeywordZoomOffset = -318f;

	public float DrawSpinRate;

	public float UnplayableOffset;

	public float DragOffset;

	public UITexture Art;

	public UITexture Frame;

	public UITexture CakeFrame;

	public UISprite CardBackSprite;

	public UILabel Name;

	public UILabel Text;

	public UILabel FlavorText;

	public UILabel Cost;

	public UILabel ActionType;

	private UIWidget MyWidget;

	public Collider CardCollider;

	public UILabel KeywordsDescription;

	public Collider CardTextCollider;

	public GameObject CanAffordIndicator;

	public Transform PulseTweener;

	public Collider ZoomCollider;

	public UIWidget ZoomColliderWidget;

	public UILabel GroupNameLabel;

	public GameObject HelperLabel;

	public GameObject Blackout;

	public GameObject SelectionVFX;

	public GameObject CostGlowVFX;

	public Vector3 Velocity = Vector3.zero;

	public int DepthAdjust = 20;

	private int mAdjustedDepth;

	public static int HighestDepth;

	public GameObject InfoParentObject;

	private CardData mCard;

	private CreatureItem mCreature;

	private EvoMaterialData mEvoMaterial;

	private XPMaterialData mXPMaterial;

	private DWBattleLaneObject mPrevHoveredLane;

	private Vector3 mHandPosition;

	private Quaternion mHandRotation;

	private Vector3 mHandScale = Vector3.one;

	private Vector3 mStoredHandPosition;

	private Quaternion mStoredHandRotation;

	private Vector3 mTargetPosition;

	private Quaternion mTargetRotation;

	private Vector3 mTargetScale;

	private Vector3 mCurrentScale = Vector3.one;

	private float mTargetAlpha = 1f;

	private float mForceSpinDegrees;

	private float mSnapSpeed = 10f;

	private float mAlphaSnapSpeed = -1f;

	private int mBaseDepth = -1;

	private bool mShowingBack;

	private bool mShowingKeywords;

	private bool mOnDiscard;

	private bool mSelectingTarget;

	private Vector3 mCardPosInGrid;

	private Vector3 mCardSizeInGrid;

	private bool mZooming;

	private bool mUnplayableOffset;

	private List<DWBattleLaneObject> mDamageTargets = new List<DWBattleLaneObject>();

	private int mShowingDamageTargetIndex;

	private List<CreatureHPBar> mActiveDamagePredictions = new List<CreatureHPBar>();

	[Header("Tweens")]
	public UITweenController ShowKeywordsTween;

	public UITweenController HideKeywordsTween;

	public UITweenController ShowBlackoutTween;

	public UITweenController HideBlackoutTween;

	public UITweenController ShowDiscardTween;

	public UITweenController HandFullTween;

	private static CardPrefabScript mZoomedCard;

	private static CardPrefabScript mDraggingCard;

	private HandCardState mState = HandCardState.InHand;

	private static string mBaseColorString;

	public float TargetSelectionPosOffset;

	public CardMode Mode { get; set; }

	public bool DestroyOnUnzoom { get; set; }

	public bool InGacha { get; set; }

	public bool InGachaDisplay { get; set; }

	public CardData Card
	{
		get
		{
			return mCard;
		}
	}

	public CreatureItem Creature
	{
		get
		{
			return mCreature;
		}
	}

	public bool SpawnedFromTile { get; set; }

	public bool OpponentInfoPopup { get; set; }

	public bool DragDrawCard { get; set; }

	public HandCardState GetState()
	{
		return mState;
	}

	private void Awake()
	{
		MyWidget = base.transform.GetComponent<UIWidget>();
		mCurrentScale = base.transform.localScale;
		if (mBaseColorString == null)
		{
			Color color = Text.color;
			mBaseColorString = "[" + color.ToHexString() + "]";
		}
	}

	public override void Populate(object dataObj)
	{
		if (dataObj is CardData)
		{
			mCard = dataObj as CardData;
			if (Mode == CardMode.Opponent)
			{
				mShowingBack = true;
				ShowBack();
			}
			else if (OpponentInfoPopup)
			{
				mShowingBack = true;
				ShowBack();
				base.gameObject.GetComponent<BoxCollider>().enabled = false;
			}
			else if (!DragDrawCard)
			{
				mShowingBack = false;
				PopulateFront();
			}
		}
		else if (dataObj is CreatureItem)
		{
			mCreature = dataObj as CreatureItem;
			mShowingBack = false;
			PopulateFront();
		}
		else if (dataObj is EvoMaterialData)
		{
			mEvoMaterial = dataObj as EvoMaterialData;
			PopulateFront();
		}
		else if (dataObj is XPMaterialData)
		{
			mXPMaterial = dataObj as XPMaterialData;
			PopulateFront();
		}
		else if (dataObj is InventorySlotItem)
		{
			InventorySlotItem inventorySlotItem = dataObj as InventorySlotItem;
			if (inventorySlotItem.SlotType == InventorySlotType.Creature)
			{
				Populate(inventorySlotItem.Creature);
			}
			else if (inventorySlotItem.SlotType == InventorySlotType.Card)
			{
				Populate(inventorySlotItem.DisplayCard);
			}
			else if (inventorySlotItem.SlotType == InventorySlotType.EvoMaterial)
			{
				Populate(inventorySlotItem.EvoMaterial);
			}
			else if (inventorySlotItem.SlotType == InventorySlotType.XPMaterial)
			{
				Populate(inventorySlotItem.XPMaterial);
			}
		}
	}

	private void PopulateFront()
	{
		CardBackSprite.gameObject.SetActive(false);
		Frame.gameObject.SetActive(true);
		CakeFrame.gameObject.SetActive(false);
		if (mCard != null)
		{
			Art.gameObject.SetActive(true);
			Singleton<SLOTResourceManager>.Instance.QueueUITextureLoad(mCard.UITexture, mCard.AssetBundle, "UI/UI/LoadingPlaceholder", Art);
			Frame.ReplaceTexture(mCard.CardFrame);
			Name.text = mCard.Name;
			ActionType.text = mCard.TypeText;
			mCard.BuildDescriptionString(Text, mBaseColorString);
			FlavorText.text = string.Empty;
			Cost.text = mCard.Cost.ToString();
		}
		else if (mCreature != null)
		{
			Art.gameObject.SetActive(true);
			Art.ReplaceTexture(mCreature.Form.PortraitTexture);
			Frame.ReplaceTexture(mCreature.Form.Faction.CreatureFrameTexture());
			Name.text = mCreature.Form.Name;
			ActionType.text = mCreature.Form.GetClassString();
			Text.text = string.Empty;
			Cost.text = mCreature.Form.DeployCost.ToString();
			if (Singleton<PlayerInfoScript>.Instance.StateData.HelperCreature != null && Singleton<PlayerInfoScript>.Instance.StateData.HelperCreature.Creature == mCreature)
			{
				HelperLabel.SetActive(true);
			}
		}
		else if (mEvoMaterial != null)
		{
			CardBackSprite.gameObject.SetActive(false);
			Frame.gameObject.SetActive(true);
			Frame.ReplaceTexture(mEvoMaterial.CardFrame);
			Art.gameObject.SetActive(true);
			Art.ReplaceTexture(mEvoMaterial.UITexture);
			Name.text = mEvoMaterial.Name;
			ActionType.text = string.Empty;
			Text.text = string.Empty;
			Cost.text = string.Empty;
		}
		else if (mXPMaterial != null)
		{
			CardBackSprite.gameObject.SetActive(false);
			Frame.gameObject.SetActive(false);
			CakeFrame.gameObject.SetActive(true);
			CakeFrame.ReplaceTexture(mXPMaterial.UICardFrame);
			Art.gameObject.SetActive(true);
			Art.ReplaceTexture(mXPMaterial.UITexture);
			Name.text = mXPMaterial.Name;
			ActionType.text = string.Empty;
			Text.text = string.Empty;
			Cost.text = string.Empty;
		}
	}

	public void ShowBack()
	{
		Art.gameObject.SetActive(false);
		if (Mode == CardMode.Opponent && Creature != null)
		{
			Frame.gameObject.SetActive(false);
			CardBackSprite.gameObject.SetActive(true);
			CardBackSprite.spriteName = Creature.Form.Faction.GetIcon();
		}
		else
		{
			Frame.ReplaceTexture(CardBackTexture());
		}
		Name.text = string.Empty;
		Cost.text = string.Empty;
		ActionType.text = string.Empty;
		Text.text = string.Empty;
		FlavorText.text = string.Empty;
		GroupNameLabel.text = string.Empty;
	}

	private string CardBackTexture()
	{
		if (Mode == CardMode.Opponent || OpponentInfoPopup)
		{
			if (Singleton<PlayerInfoScript>.Instance.StateData.MultiplayerMode)
			{
				return Singleton<PlayerInfoScript>.Instance.PvPData.OpponentCardBack.TextureUI;
			}
			return CardBackDataManager.DefaultData.TextureUI;
		}
		return Singleton<PlayerInfoScript>.Instance.SaveData.SelectedCardBack.TextureUI;
	}

	public override void Unload()
	{
		Art.UnloadTexture();
		Frame.UnloadTexture();
	}

	public void AdjustDepth(int depth)
	{
		UIWidget[] componentsInChildren = base.gameObject.GetComponentsInChildren<UIWidget>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].depth += depth * 1000;
		}
		ZoomColliderWidget.depth = 999;
	}

	public void ResetDepth()
	{
		AdjustDepth(-10);
	}

	public bool IsDraggable()
	{
		if (Mode != 0)
		{
			return false;
		}
		if (mState != HandCardState.InHand && mState != HandCardState.ZoomedIn)
		{
			return false;
		}
		if (!Singleton<DWGame>.Instance.GetCurrentGameState().IsP1Turn())
		{
			return false;
		}
		if (Singleton<TutorialController>.Instance.BlockCardDragging())
		{
			return false;
		}
		if (mCreature != null && mState == HandCardState.ZoomedIn)
		{
			return false;
		}
		if (Singleton<DWBattleLane>.Instance.LootObjectsToCollect())
		{
			return false;
		}
		return true;
	}

	public void OnCardDragStart()
	{
		if (mDraggingCard != null)
		{
			mDraggingCard.OnCardDropped();
		}
		if (mSelectingTarget)
		{
			Singleton<DWBattleLane>.Instance.CancelTargetSelection();
			Singleton<DWBattleLane>.Instance.UpdateAvailableActionIndicators();
		}
		mDraggingCard = this;
		Singleton<HandCardController>.Instance.UnzoomCard();
		SelectionVFX.SetActive(true);
		if (StillInHand())
		{
			SetCardState(HandCardState.DraggingInHand);
		}
		else
		{
			SetCardState(HandCardState.DraggingOnBoard);
		}
		Singleton<HandCardController>.Instance.OnCardDragStart();
		AdjustDepth(10);
		CardCollider.enabled = false;
		Singleton<SLOTAudioManager>.Instance.PlaySound("SFX_Card_Choose");
		Singleton<HandCardController>.Instance.ShowDiscardTween.Play();
		if (mCard != null)
		{
			Singleton<BattleHudController>.Instance.ShowCost(mCard.Cost);
		}
		else if (mCreature != null && !Singleton<DWGame>.Instance.InDeploymentPhase())
		{
			Singleton<BattleHudController>.Instance.ShowCost(mCreature.Form.DeployCost);
		}
	}

	public void OnCardMoved()
	{
		if (StillInHand())
		{
			SetCardState(HandCardState.DraggingInHand);
		}
		else
		{
			SetCardState(HandCardState.DraggingOnBoard);
		}
		if (mCard == null)
		{
			return;
		}
		if (mCard.TargetType1 == SelectionType.Lane)
		{
			DWBattleLaneObject dWBattleLaneObject = ((mState != HandCardState.DraggingOnBoard) ? null : BattleHudController.GetHoveredLane(mCard));
			if (dWBattleLaneObject != mPrevHoveredLane)
			{
				if (dWBattleLaneObject != null)
				{
					Singleton<DWBattleLane>.Instance.SetTargetIndicators(Card, dWBattleLaneObject);
					mPrevHoveredLane = dWBattleLaneObject;
					DetermineDamageTargets();
				}
				else
				{
					mPrevHoveredLane = dWBattleLaneObject;
					Singleton<DWBattleLane>.Instance.HideTargetIndicators();
					StopDraggingDamagePredictions();
				}
			}
		}
		bool flag = HoveringOnDiscard();
		if (flag && !mOnDiscard)
		{
			ShowDiscardTween.Play();
		}
		else if (!flag && mOnDiscard)
		{
			ShowDiscardTween.End();
		}
		mOnDiscard = flag;
	}

	public void OnCardDropped()
	{
		if (mDraggingCard == null)
		{
			return;
		}
		mDraggingCard = null;
		CardCollider.enabled = true;
		Singleton<HandCardController>.Instance.OnCardDragFinish();
		AdjustDepth(-10);
		HandCardState handCardState = mState;
		SetCardState(HandCardState.InHand);
		Singleton<BattleHudController>.Instance.HideCost();
		Singleton<DWBattleLane>.Instance.HideDamagePredictions();
		bool flag = true;
		if (handCardState == HandCardState.DraggingOnBoard)
		{
			if (mCard != null)
			{
				DWBattleLaneObject hoveredLane = BattleHudController.GetHoveredLane(mCard);
				if (!Singleton<DWGame>.Instance.InDeploymentPhase())
				{
					switch ((!(hoveredLane != null)) ? Singleton<DWGame>.Instance.CanPlay(PlayerType.User, Card) : Singleton<DWGame>.Instance.CanPlay(PlayerType.User, Card, hoveredLane.Creature.Owner.Type, hoveredLane.Creature.Lane.Index))
					{
					case PlayerState.CanPlayResult.CanPlay:
					{
						Singleton<DWBattleLane>.Instance.PlayUserActionCard(this, hoveredLane);
						Singleton<SLOTAudioManager>.Instance.SetVOEventCooldown(VOEvent.Idle);
						VOEvent voEvent = ((!Card.IsLeaderCard) ? VOEvent.PlayCard : VOEvent.PlayLeaderCard);
						Singleton<SLOTAudioManager>.Instance.TriggerVOEvent(Singleton<DWGame>.Instance.GetCharacter(PlayerType.User), voEvent);
						if (Card.RequiresTargetSelection() && Singleton<DWBattleLane>.Instance.GetAutoTargetForCard(Card) == null)
						{
							flag = false;
						}
						else
						{
							StartCoroutine(ShowPlayAnim());
						}
						break;
					}
					case PlayerState.CanPlayResult.NotEnoughAP:
						Singleton<BattleHudController>.Instance.ShowErrorReason(KFFLocalization.Get("!!NOT_ENOUGH_ENERGY"));
						Singleton<SLOTAudioManager>.Instance.PlaySound("SFX_Announcer_OutofEnergy");
						break;
					case PlayerState.CanPlayResult.NoTarget:
						Singleton<BattleHudController>.Instance.ShowErrorReason(KFFLocalization.Get("!!NO_CARD_TARGET"));
						break;
					case PlayerState.CanPlayResult.MustPlayCreature:
						Singleton<BattleHudController>.Instance.ShowErrorReason(KFFLocalization.Get("!!MUST_PLAY_CREATURE"));
						break;
					}
				}
			}
			else
			{
				switch (Singleton<DWGame>.Instance.CanPlay(PlayerType.User, mCreature))
				{
				case PlayerState.CanPlayResult.CanPlay:
				{
					Vector3 worldPos = BattleHudController.GetWorldPos(Input.mousePosition, 0f);
					List<DWBattleLaneObject> list = Singleton<DWBattleLane>.Instance.BattleLaneObjects[0];
					int laneIndex = list.Count;
					for (int i = 0; i < list.Count; i++)
					{
						if (0f - worldPos.z < 0f - list[i].transform.position.z)
						{
							laneIndex = i;
							break;
						}
					}
					Singleton<SLOTAudioManager>.Instance.SetVOEventCooldown(VOEvent.Idle);
					Singleton<SLOTAudioManager>.Instance.TriggerVOEvent(Singleton<DWGame>.Instance.GetCharacter(PlayerType.User), VOEvent.PlayCreature, mCreature.Faction);
					Singleton<DWGame>.Instance.DeployCreatureFromCard(PlayerType.User, mCreature, laneIndex);
					if (Singleton<DWGame>.Instance.CurrentBoardState.GetPlayerState(0).GetCreatureCount() >= MiscParams.CreaturesOnBoard)
					{
						Singleton<HandCardController>.Instance.HideCreatureHand();
					}
					StartCoroutine(ShowPlayAnim());
					break;
				}
				case PlayerState.CanPlayResult.NotEnoughAP:
					Singleton<BattleHudController>.Instance.ShowErrorReason(KFFLocalization.Get("!!NOT_ENOUGH_ENERGY"));
					Singleton<SLOTAudioManager>.Instance.PlaySound("SFX_Announcer_OutofEnergy");
					break;
				}
			}
		}
		else if (HoveringOnDiscard())
		{
			ShowDiscardTween.End();
			StartCoroutine(ManualDiscard());
		}
		if (flag)
		{
			SelectionVFX.SetActive(false);
		}
		Singleton<DWBattleLane>.Instance.HideTargetIndicators();
		StopDraggingDamagePredictions();
		Singleton<HandCardController>.Instance.HideDiscardTween.Play();
	}

	public static void CancelCardDrag()
	{
		if (mDraggingCard != null)
		{
			mDraggingCard.mState = HandCardState.DraggingInHand;
			mDraggingCard.GetComponent<HandCardDragDropItem>().ForceDrop();
		}
	}

	public void OnCardPressed()
	{
	}

	public void OnCardClicked()
	{
		if (mState != HandCardState.FailedDrawing && mState != 0)
		{
			if (DetachedSingleton<SceneFlowManager>.Instance.InBattleScene())
			{
				Singleton<CreatureInfoPopup>.Instance.OnEffectReleased();
			}
			if (mState == HandCardState.LogZoom)
			{
				StartCoroutine(PlayLogZoomCloseAnim());
			}
			else if (mCreature != null)
			{
				StartCoroutine(ToggleZoom());
			}
			else if ((Mode == CardMode.Hand || Mode == CardMode.BattlePopup || Mode == CardMode.GeneralFrontEnd) && !OpponentInfoPopup)
			{
				StartCoroutine(ToggleZoom());
			}
		}
	}

	public void OnClickDescription()
	{
		ShowKeywordsTween.Play();
		string text = "[FBE58A]";
		KeywordsDescription.color = Color.white;
		string text2 = string.Empty;
		foreach (KeyWordData descriptionKeyword in Card.DescriptionKeywords)
		{
			if (text2 != string.Empty)
			{
				text2 += "\n\n";
			}
			string text3 = text2;
			text2 = text3 + "[" + descriptionKeyword.TextColor + "]" + descriptionKeyword.DisplayName + text + " : " + descriptionKeyword.Description;
		}
		KeywordsDescription.text = text2;
		mShowingKeywords = true;
		CardTextCollider.enabled = false;
		if (Mode == CardMode.GeneralFrontEnd)
		{
			mTargetPosition = Vector3.zero;
		}
		else
		{
			mTargetPosition = GetZoomPosition().localPosition;
		}
		mTargetPosition.x += KeywordZoomOffset;
	}

	public IEnumerator ToggleZoom()
	{
		if (Mode == CardMode.Hand)
		{
			if (mState == HandCardState.ZoomedIn)
			{
				if (mCreature != null)
				{
					yield return new WaitForSeconds(0.3f);
				}
				if (!mSelectingTarget)
				{
					SelectionVFX.SetActive(false);
					if (mCard != null)
					{
						base.transform.parent = Singleton<HandCardController>.Instance.HandCardsSpawnParent.transform;
					}
					else
					{
						base.transform.parent = Singleton<HandCardController>.Instance.HandCreatureCardsSpawnParent.transform;
					}
				}
				else
				{
					base.transform.parent = Singleton<HandCardController>.Instance.HandCardsTargetingParent.transform;
				}
				AdjustDepth(-10);
				if (mCard != null)
				{
					Singleton<HandCardController>.Instance.ZoomCollider.gameObject.SetActive(false);
				}
				else if (mCreature != null)
				{
					CardCollider.enabled = true;
				}
				SetCardState(HandCardState.InHand);
			}
			else
			{
				if (!Singleton<DWGame>.Instance.GetCurrentGameState().IsP1Turn() || Singleton<DWBattleLane>.Instance.LootObjectsToCollect())
				{
					yield break;
				}
				base.transform.parent = Singleton<HandCardController>.Instance.transform;
				Singleton<HandCardController>.Instance.UnzoomCard();
				AdjustDepth(10);
				if (mCard != null)
				{
					Singleton<HandCardController>.Instance.ZoomCollider.gameObject.SetActive(true);
				}
				else if (mCreature != null)
				{
					CardCollider.enabled = false;
				}
				SetCardState(HandCardState.ZoomedIn);
				SelectionVFX.SetActive(true);
				if (mCreature != null)
				{
					Singleton<CreatureInfoPopup>.Instance.Show(mCreature);
				}
			}
		}
		else if (Mode == CardMode.GeneralFrontEnd)
		{
			if (mState == HandCardState.ZoomedIn)
			{
				if (Singleton<CreatureDetailsController>.Instance != null && Singleton<CreatureDetailsController>.Instance.MainPanel.activeInHierarchy)
				{
					Singleton<CreatureDetailsController>.Instance.ShowCreatureModel(true);
				}
				if (Blackout.activeSelf)
				{
					HideBlackoutTween.Play();
				}
				SelectionVFX.SetActive(false);
				CostGlowVFX.SetActive(false);
				if (DestroyOnUnzoom)
				{
					mTargetAlpha = 0f;
					mSnapSpeed = 15f;
					UICamera.LockInput();
					yield return new WaitForSeconds(0.15f);
					UICamera.UnlockInput();
					NGUITools.Destroy(base.gameObject);
					yield break;
				}
				if (SpawnedFromTile)
				{
					CardCollider.enabled = false;
				}
				ZoomCollider.gameObject.SetActive(false);
				SetCardState(HandCardState.InHand);
				AdjustDepth(-10);
				mZoomedCard = null;
			}
			else
			{
				if (mZoomedCard != null)
				{
					mZoomedCard.Unzoom();
				}
				if (SpawnedFromTile)
				{
					CardCollider.enabled = true;
				}
				ZoomCollider.gameObject.SetActive(true);
				SetCardState(HandCardState.ZoomedIn);
				AdjustDepth(10);
				ShowBlackoutTween.Play();
				UIWidget[] componentsInChildren = Blackout.GetComponentsInChildren<UIWidget>(true);
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].depth--;
				}
				mZoomedCard = this;
				if (mXPMaterial == null)
				{
					SelectionVFX.SetActive(true);
					CostGlowVFX.SetActive(true);
				}
			}
		}
		else if (Mode == CardMode.BattlePopup)
		{
			if (mState == HandCardState.ZoomedIn)
			{
				Singleton<CreatureInfoPopup>.Instance.ZoomCollider.gameObject.SetActive(false);
				SetCardState(HandCardState.InHand);
				AdjustDepth(-10);
				CardPrefabScript zoomedCreatureCard2 = Singleton<HandCardController>.Instance.GetZoomedCreatureCard();
				if (zoomedCreatureCard2 != null)
				{
					zoomedCreatureCard2.mTargetAlpha = 1f;
					zoomedCreatureCard2.SelectionVFX.SetActive(true);
					if (zoomedCreatureCard2.CanPlay())
					{
						zoomedCreatureCard2.SetPlayableIndicator(true);
					}
				}
			}
			else
			{
				Singleton<CreatureInfoPopup>.Instance.UnzoomCard();
				Singleton<CreatureInfoPopup>.Instance.ZoomCollider.gameObject.SetActive(true);
				SetCardState(HandCardState.ZoomedIn);
				AdjustDepth(10);
				CardPrefabScript zoomedCreatureCard = Singleton<HandCardController>.Instance.GetZoomedCreatureCard();
				if (zoomedCreatureCard != null)
				{
					zoomedCreatureCard.mTargetAlpha = 0f;
					zoomedCreatureCard.SelectionVFX.SetActive(false);
					zoomedCreatureCard.SetPlayableIndicator(false);
				}
			}
		}
		Singleton<SLOTAudioManager>.Instance.PlaySound("SFX_Card_Whoosh");
	}

	public void Unzoom()
	{
		if (mState == HandCardState.ZoomedIn)
		{
			StartCoroutine(ToggleZoom());
		}
	}

	public void SetHandLocation(Vector3 nguiPos, float angle, int depth)
	{
		mHandPosition = NguiPosToCardPos(nguiPos);
		if (Mode == CardMode.Opponent)
		{
			mHandRotation = Quaternion.Euler(new Vector3(0f, 180f, angle));
			mHandScale = Singleton<HandCardController>.Instance.OpponentPosition.transform.localScale;
		}
		else
		{
			mHandRotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
			if (Creature != null)
			{
				float creatureCardScale = Singleton<HandCardController>.Instance.CreatureCardScale;
				mHandScale = new Vector3(creatureCardScale, creatureCardScale, creatureCardScale);
			}
			else
			{
				mHandScale = Vector3.one;
			}
		}
		if (mBaseDepth == -1)
		{
			AdjustDepth(depth);
		}
		else
		{
			int depth2 = depth - mBaseDepth;
			AdjustDepth(depth2);
		}
		mBaseDepth = depth;
	}

	private Vector3 NguiPosToCardPos(Vector3 nguiPos)
	{
		nguiPos.z = 0f;
		Vector3 screenPos = Singleton<DWGameCamera>.Instance.BattleUICam.WorldToScreenPoint(nguiPos);
		return ScreenPosToCardPos(screenPos);
	}

	private Vector3 ScreenPosToCardPos(Vector3 screenPos)
	{
		screenPos.z = Singleton<HandCardController>.Instance.ScreenZAdjust;
		Vector3 result = Singleton<DWGameCamera>.Instance.Battle3DUICam.ScreenToWorldPoint(screenPos);
		result.z = Singleton<HandCardController>.Instance.CardZDepth;
		return result;
	}

	private void Update()
	{
		PrintMode = Mode;
		PrintState = mState;
		if (Mode == CardMode.Loot)
		{
			base.transform.localScale = Vector3.one;
			base.transform.localPosition = Vector3.zero;
			return;
		}
		float deltaTime = Time.deltaTime;
		if (Mode == CardMode.Hand || Mode == CardMode.Opponent)
		{
			UpdateTransformTargets();
			if (mState == HandCardState.DraggingInHand || mState == HandCardState.DraggingOnBoard)
			{
				base.transform.localPosition = mTargetPosition - base.transform.parent.localPosition;
			}
			else
			{
				base.transform.localPosition = NGUIMath.SpringLerp(base.transform.localPosition, mTargetPosition, mSnapSpeed, deltaTime);
			}
			if (mForceSpinDegrees > 0f)
			{
				float num = DrawSpinRate * Time.deltaTime;
				base.transform.localRotation = Quaternion.AngleAxis(num, Vector3.up) * base.transform.localRotation;
				mForceSpinDegrees -= num;
				if (mForceSpinDegrees < 0f)
				{
					mForceSpinDegrees = 0f;
				}
			}
			else
			{
				base.transform.localRotation = NGUIMath.SpringLerp(base.transform.localRotation, mTargetRotation, mSnapSpeed, deltaTime);
			}
			Vector3 to = mTargetScale;
			if (mSelectingTarget && mState != HandCardState.ZoomedIn)
			{
				to.x *= PulseTweener.localScale.x;
				to.y *= PulseTweener.localScale.y;
			}
			mCurrentScale = NGUIMath.SpringLerp(mCurrentScale, to, mSnapSpeed, deltaTime);
			float strength = ((!(mAlphaSnapSpeed > 0f)) ? mSnapSpeed : mAlphaSnapSpeed);
			MyWidget.alpha = NGUIMath.SpringLerp(MyWidget.alpha, mTargetAlpha, strength, deltaTime);
			CheckFrontBackStatus();
			if (Mode == CardMode.Hand)
			{
				UpdateDraggingDamagePredictions();
			}
		}
		else
		{
			if (Mode != CardMode.BattlePopup && Mode != CardMode.GeneralFrontEnd)
			{
				return;
			}
			UpdateTransformTargets();
			if (mState == HandCardState.ZoomedIn)
			{
				Vector3 to3;
				Vector3 to2;
				if (Mode == CardMode.GeneralFrontEnd)
				{
					to2 = Vector3.zero;
					if (mShowingKeywords)
					{
						UIPanel uIPanel = MyWidget.panel;
						while (uIPanel.parentPanel != null)
						{
							uIPanel = uIPanel.parentPanel;
						}
						to2.x += KeywordZoomOffset * uIPanel.transform.localScale.x;
					}
					to3 = new Vector3(2.5f, 2.5f, 1f);
					Transform parent = base.transform.parent;
					while (parent != MyWidget.panel.transform)
					{
						to3.x /= parent.localScale.x;
						to3.y /= parent.localScale.y;
						parent = parent.parent;
					}
				}
				else
				{
					Transform zoomPosition = GetZoomPosition();
					to2 = zoomPosition.localPosition;
					if (mShowingKeywords)
					{
						to2.x += KeywordZoomOffset;
					}
					to2 = zoomPosition.parent.TransformPoint(to2);
					to3 = zoomPosition.localScale;
				}
				base.transform.position = NGUIMath.SpringLerp(base.transform.position, to2, mSnapSpeed, deltaTime);
				base.transform.rotation = NGUIMath.SpringLerp(base.transform.rotation, Quaternion.Euler(Vector3.zero), mSnapSpeed, deltaTime);
				base.transform.localScale = NGUIMath.SpringLerp(base.transform.localScale, to3, mSnapSpeed, deltaTime);
			}
			else if (!InGachaDisplay)
			{
				base.transform.localPosition = NGUIMath.SpringLerp(base.transform.localPosition, Vector3.zero, mSnapSpeed, deltaTime);
				base.transform.localRotation = NGUIMath.SpringLerp(base.transform.localRotation, Quaternion.Euler(Vector3.zero), mSnapSpeed, deltaTime);
				base.transform.localScale = NGUIMath.SpringLerp(base.transform.localScale, Vector3.one, mSnapSpeed, deltaTime);
			}
			float strength2 = ((!(mAlphaSnapSpeed > 0f)) ? mSnapSpeed : mAlphaSnapSpeed);
			MyWidget.alpha = NGUIMath.SpringLerp(MyWidget.alpha, mTargetAlpha, strength2, deltaTime);
			if (InGacha)
			{
				CheckFrontBackStatus();
			}
			else if (!InGachaDisplay)
			{
				base.transform.SetLocalPositionZ(-2300f);
			}
		}
	}

	private void CheckFrontBackStatus()
	{
		bool flag = Vector3.Dot(rhs: (InGacha ? Singleton<GachaOpenSequencer>.Instance.CreatureCamera : ((!InGachaDisplay) ? Singleton<DWGameCamera>.Instance.Battle3DUICam : Singleton<GachaScreenController>.Instance.CreatureCamera)).transform.position - base.transform.position, lhs: base.transform.forward) > 0f;
		if (Singleton<GachaOpenSequencer>.Instance.Showing)
		{
			flag = false;
		}
		if (!mShowingBack && flag)
		{
			mShowingBack = true;
			ShowBack();
		}
		else if (mShowingBack && !flag)
		{
			mShowingBack = false;
			PopulateFront();
		}
		Vector3 localScale = mCurrentScale;
		if (mShowingBack)
		{
			localScale.x *= -1f;
		}
		base.transform.localScale = localScale;
	}

	public void SetCardState(HandCardState state)
	{
		if (mState == state)
		{
			return;
		}
		mState = state;
		if (Mode == CardMode.Hand && mCard != null)
		{
			if (mState == HandCardState.DraggingInHand || mState == HandCardState.DraggingOnBoard)
			{
				Singleton<BattleHudController>.Instance.DisplayPredictedValueOnHover(mCard);
			}
			else
			{
				Singleton<BattleHudController>.Instance.StopDisplayPredictedValue();
			}
		}
		if ((mState == HandCardState.ZoomedIn || mState == HandCardState.LogZoom) && Card != null && Card.DescriptionKeywords.Count > 0)
		{
			CardTextCollider.enabled = true;
		}
		else
		{
			CardTextCollider.enabled = false;
			if (mShowingKeywords)
			{
				HideKeywordsTween.Play();
				mShowingKeywords = false;
			}
		}
		if (mState != 0)
		{
			if (mState == HandCardState.FailedDrawing)
			{
				if (Mode == CardMode.Hand)
				{
					mTargetPosition = base.transform.localPosition;
					mTargetRotation = Quaternion.Euler(Vector3.zero);
				}
				else
				{
					mTargetPosition = base.transform.localPosition;
					mTargetPosition.y += 100f;
					mTargetRotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
				}
				mSnapSpeed = 15f;
			}
			else if (mState == HandCardState.InHand)
			{
				mSnapSpeed = 10f;
			}
			else if (mState == HandCardState.ZoomedIn || mState == HandCardState.LogZoom)
			{
				if (Mode == CardMode.GeneralFrontEnd)
				{
					mTargetPosition = Vector3.zero;
				}
				else
				{
					mTargetPosition = GetZoomPosition().localPosition;
				}
				mTargetRotation = Quaternion.Euler(Vector3.zero);
				mSnapSpeed = 15f;
			}
			else if (mState == HandCardState.DraggingInHand)
			{
				mTargetRotation = Quaternion.Euler(Vector3.zero);
				if (DragDrawCard)
				{
					mSnapSpeed = 15f;
				}
				else
				{
					mSnapSpeed = 8f;
				}
			}
			else if (mState == HandCardState.DraggingOnBoard)
			{
				mSnapSpeed = 8f;
			}
			else if (mState == HandCardState.Discarding)
			{
				mTargetPosition = mHandPosition;
				if (Mode == CardMode.Opponent)
				{
					mTargetPosition.y -= 30f;
				}
				else
				{
					mTargetPosition.y -= 100f;
				}
				mTargetRotation = mHandRotation;
				mTargetAlpha = 0f;
				mSnapSpeed = 6f;
			}
			else if (mState == HandCardState.ManualDiscarding)
			{
				mTargetPosition = base.transform.localPosition;
				mTargetRotation = base.transform.localRotation;
				mTargetAlpha = 0f;
				mSnapSpeed = 6f;
			}
			else if (mState == HandCardState.Playing)
			{
				mTargetPosition = base.transform.localPosition;
				mTargetRotation = base.transform.localRotation;
				mTargetAlpha = 0f;
				mSnapSpeed = 7f;
			}
			else if (mState == HandCardState.ClosingLogZoom)
			{
				mTargetAlpha = 0f;
				mSnapSpeed = 10f;
			}
			else if (mState == HandCardState.OpponentPlaying)
			{
				mTargetPosition = Singleton<HandCardController>.Instance.OpponentPlayPosition.localPosition;
				mTargetRotation = Quaternion.Euler(Vector3.zero);
				mSnapSpeed = 5f;
			}
		}
		if (Mode == CardMode.Hand)
		{
			if (mState == HandCardState.ZoomedIn || mState == HandCardState.LogZoom || mState == HandCardState.ClosingLogZoom)
			{
				mTargetScale = GetZoomPosition().localScale;
			}
			else if (mState == HandCardState.DraggingOnBoard)
			{
				mTargetScale = Vector3.one;
			}
			else
			{
				mTargetScale = mHandScale;
			}
		}
		else if (Mode == CardMode.Opponent)
		{
			if (state == HandCardState.OpponentPlaying)
			{
				mTargetScale = GetZoomPosition().localScale;
			}
			else
			{
				mTargetScale = Singleton<HandCardController>.Instance.OpponentPosition.transform.localScale;
			}
		}
		UpdateTransformTargets();
	}

	private void UpdateTransformTargets()
	{
		if (mState == HandCardState.InHand)
		{
			mTargetPosition = mHandPosition;
			if (mUnplayableOffset)
			{
				if (mCard != null)
				{
					mTargetPosition.y -= UnplayableOffset;
				}
				else
				{
					mTargetPosition.x -= UnplayableOffset;
				}
			}
			mTargetRotation = mHandRotation;
		}
		else if (mState == HandCardState.DraggingInHand)
		{
			mTargetPosition = ScreenPosToCardPos(Input.mousePosition);
			if (mCard != null)
			{
				mTargetPosition.y -= DragOffset;
			}
			else
			{
				mTargetPosition.y += DragOffset;
			}
		}
		else if (mState == HandCardState.DraggingOnBoard)
		{
			Vector3 worldPos = BattleHudController.GetWorldPos(Input.mousePosition, Singleton<HandCardController>.Instance.BoardHeightAdjust);
			Vector3 screenPos = Singleton<DWGameCamera>.Instance.MainCam.WorldToScreenPoint(worldPos);
			mTargetPosition = ScreenPosToCardPos(screenPos);
			if (mCard != null)
			{
				mTargetPosition.y -= DragOffset;
			}
			else
			{
				mTargetPosition.y += DragOffset;
			}
			mTargetRotation = Quaternion.Euler(90f - Singleton<DWGameCamera>.Instance.MainCam.transform.rotation.eulerAngles.x, 0f, 0f);
		}
	}

	private Transform GetZoomPosition()
	{
		if (Mode == CardMode.BattlePopup)
		{
			return Singleton<CreatureInfoPopup>.Instance.ZoomPosition;
		}
		if (mCreature != null && mState != HandCardState.LogZoom)
		{
			return Singleton<HandCardController>.Instance.CreatureZoomPosition;
		}
		return Singleton<HandCardController>.Instance.ZoomPosition;
	}

	private bool StillInHand()
	{
		if (UICamera.hoveredObject == null)
		{
			return false;
		}
		if (NGUITools.FindInParents<HandCardController>(UICamera.hoveredObject) != null)
		{
			return true;
		}
		return false;
	}

	private bool HoveringOnDiscard()
	{
		return UICamera.hoveredObject == Singleton<HandCardController>.Instance.DeckCollider.gameObject;
	}

	public IEnumerator PlayFastDrawAnim(GameObject sourceCreature, Vector3 drawToPosition, float scale, int depth)
	{
		SetCardState(HandCardState.FastDrawing);
		bool skipZoom = false;
		if (sourceCreature != null)
		{
			if (sourceCreature.layer == base.gameObject.layer)
			{
				base.transform.localPosition = sourceCreature.transform.localPosition;
				skipZoom = true;
				Singleton<SLOTAudioManager>.Instance.PlaySound("SFX_Card_Flip");
			}
			else
			{
				if (!DragDrawCard)
				{
					Vector3 laneWorldPos = sourceCreature.transform.position;
					Vector3 screenPos2 = Singleton<DWGameCamera>.Instance.MainCam.WorldToScreenPoint(laneWorldPos);
					base.transform.localPosition = ScreenPosToCardPos(screenPos2);
					mBaseDepth = 10 + depth;
					AdjustDepth(mBaseDepth);
				}
				Singleton<SLOTAudioManager>.Instance.PlaySound("SFX_Card_Flip");
			}
		}
		else
		{
			Vector3 portraitPos = ((Mode != 0) ? Singleton<BattleHudController>.Instance.HeroPortraits[1].transform.position : Singleton<BattleHudController>.Instance.HeroPortraits[0].transform.position);
			Vector3 screenPos = Singleton<DWGameCamera>.Instance.BattleUICam.WorldToScreenPoint(portraitPos);
			base.transform.localPosition = ScreenPosToCardPos(screenPos);
			Singleton<SLOTAudioManager>.Instance.PlaySound("SFX_Card_Flip");
		}
		if (Mode == CardMode.Hand)
		{
			base.transform.localRotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
			mTargetRotation = Quaternion.Euler(Vector3.zero);
			mCurrentScale = new Vector3(0.5f, 0.5f, 1f);
			if (Singleton<TutorialController>.Instance.PauseToShowCard())
			{
				mTargetPosition = Vector3.zero;
				mTargetScale = Singleton<HandCardController>.Instance.ZoomPosition.localScale * scale * 4f;
			}
			else
			{
				mTargetPosition = drawToPosition;
				mTargetScale = Singleton<HandCardController>.Instance.ZoomPosition.localScale * scale;
			}
			mSnapSpeed = 8f;
			mForceSpinDegrees = 360f;
			if (!Singleton<DWGame>.Instance.IsTutorialSetup)
			{
				GameObject drawFX2 = (GameObject)SLOTGame.InstantiateFX(Singleton<BattleHudController>.Instance.CardDrawFX, base.transform.position, base.transform.rotation);
				drawFX2.transform.parent = base.transform.parent;
				drawFX2.transform.localScale = mCurrentScale;
				if (!DragDrawCard)
				{
					base.gameObject.SetActive(false);
					yield return new WaitForSeconds(0.25f);
					base.gameObject.SetActive(true);
				}
			}
			int cardDrawIndex = 0;
			if (!skipZoom)
			{
				cardDrawIndex = Singleton<HandCardController>.Instance.CardsMovingToDrawPos;
				Singleton<HandCardController>.Instance.CardsMovingToDrawPos++;
				yield return new WaitForSeconds(0.7f);
				Singleton<HandCardController>.Instance.CardsArrivedAtDrawPos++;
			}
			while (Singleton<HandCardController>.Instance.CardsArrivedAtDrawPos < Singleton<HandCardController>.Instance.CardsMovingToDrawPos)
			{
				yield return null;
			}
			if (cardDrawIndex > 0)
			{
				yield return new WaitForSeconds((float)cardDrawIndex * 0.1f);
			}
			if (Singleton<TutorialController>.Instance.PauseToShowCard())
			{
				while (Singleton<TutorialController>.Instance.PauseToShowCard())
				{
					yield return null;
				}
				mTargetPosition = drawToPosition;
				mTargetScale = Singleton<HandCardController>.Instance.ZoomPosition.localScale * scale;
			}
			Singleton<HandCardController>.Instance.AddHandCard(this);
			SetCardState(HandCardState.InHand);
			Singleton<HandCardController>.Instance.CardsMovingToDrawPos = 0;
			Singleton<HandCardController>.Instance.CardsArrivedAtDrawPos = 0;
		}
		else
		{
			Singleton<HandCardController>.Instance.AddOpponentCard(this);
			mTargetPosition = mHandPosition;
			mTargetRotation = mHandRotation;
			base.transform.localRotation = mTargetRotation;
			mCurrentScale = mTargetScale;
			mSnapSpeed = 8f;
			if (!Singleton<DWGame>.Instance.IsTutorialSetup)
			{
				GameObject drawFX = (GameObject)SLOTGame.InstantiateFX(Singleton<BattleHudController>.Instance.CardDrawFX, base.transform.position, base.transform.rotation);
				drawFX.transform.parent = base.transform.parent;
				drawFX.transform.localScale = mCurrentScale;
				base.gameObject.SetActive(false);
				yield return new WaitForSeconds(0.25f);
				base.gameObject.SetActive(true);
			}
			SetCardState(HandCardState.InHand);
		}
	}

	public IEnumerator PlayFailedDrawAnim(int depth, GameObject sourceCreature)
	{
		if (!DragDrawCard)
		{
			Vector3 laneWorldPos = sourceCreature.transform.position;
			Vector3 screenPos = Singleton<DWGameCamera>.Instance.MainCam.WorldToScreenPoint(laneWorldPos);
			base.transform.localPosition = ScreenPosToCardPos(screenPos);
		}
		SetCardState(HandCardState.FailedDrawing);
		AdjustDepth(depth);
		if (Mode == CardMode.Hand)
		{
			base.transform.localRotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
			HandFullTween.Play();
		}
		else
		{
			base.transform.localRotation = mTargetRotation;
			mCurrentScale = mTargetScale;
		}
		Singleton<SLOTAudioManager>.Instance.PlaySound("SFX_Card_Flip");
		yield return new WaitForSeconds(0.7f);
		mTargetAlpha = 0f;
		mSnapSpeed = 7f;
		yield return new WaitForSeconds(0.4f);
		NGUITools.Destroy(base.gameObject);
	}

	public IEnumerator PlayCreatureDrawAnim(Vector3 drawFromPos)
	{
		if (Mode == CardMode.Hand)
		{
			Singleton<HandCardController>.Instance.AddHandCard(this);
		}
		else
		{
			Singleton<HandCardController>.Instance.AddOpponentCard(this);
		}
		if (drawFromPos.x == float.MinValue)
		{
			base.transform.localPosition = mHandPosition;
			if (Mode == CardMode.Hand)
			{
				base.transform.AddLocalPositionY(-100f);
			}
			else
			{
				base.transform.AddLocalPositionX(200f);
			}
			mCurrentScale = (mTargetScale = mHandScale);
		}
		else
		{
			Vector3 screenPos = Singleton<DWGameCamera>.Instance.MainCam.WorldToScreenPoint(drawFromPos);
			base.transform.localPosition = ScreenPosToCardPos(screenPos);
			mCurrentScale = new Vector3(0.5f, 0.5f, 1f);
			mTargetScale = mHandScale;
		}
		SetCardState(HandCardState.InHand);
		Singleton<SLOTAudioManager>.Instance.PlaySound("SFX_Card_Flip");
		yield return null;
	}

	public IEnumerator Discard()
	{
		SetCardState(HandCardState.Discarding);
		Singleton<SLOTAudioManager>.Instance.PlaySound("SFX_Card_Whoosh");
		yield return new WaitForSeconds(0.5f);
		if (Mode == CardMode.Hand)
		{
			Singleton<HandCardController>.Instance.RemoveHandCard(this);
		}
		else
		{
			Singleton<HandCardController>.Instance.RemoveOpponentCard(this);
			Singleton<CharacterAnimController>.Instance.Remove3DHoldCard(PlayerType.Opponent);
		}
		NGUITools.Destroy(base.gameObject);
	}

	public IEnumerator ManualDiscard()
	{
		SetCardState(HandCardState.ManualDiscarding);
		Singleton<DWGame>.Instance.CurrentBoardState.GetPlayerState(PlayerType.User).ManualDiscard(Card);
		Singleton<HandCardController>.Instance.RemoveHandCard(this);
		Singleton<SLOTAudioManager>.Instance.PlaySound("SFX_Card_Whoosh");
		yield return new WaitForSeconds(0.5f);
		NGUITools.Destroy(base.gameObject);
	}

	public IEnumerator ShowPlayAnim()
	{
		UICamera.LockInput();
		SetCardState(HandCardState.Playing);
		PlayCardFadeFX();
		Singleton<HandCardController>.Instance.RemoveHandCard(this);
		Singleton<SLOTAudioManager>.Instance.PlaySound("SFX_Card_Play");
		yield return new WaitForSeconds(0.25f);
		UICamera.UnlockInput();
		NGUITools.Destroy(base.gameObject);
	}

	public IEnumerator ShowOpponentPlayAnim(GameObject creatureTarget, float zOffset = 0f)
	{
		if (creatureTarget != null)
		{
			yield return StartCoroutine(ShowOpponentPlayAnim(creatureTarget.transform.position + new Vector3(0f, 0f, zOffset)));
		}
		else
		{
			yield return StartCoroutine(ShowOpponentPlayAnim(new Vector3(float.MinValue, float.MinValue, float.MinValue)));
		}
	}

	public IEnumerator ShowOpponentPlayAnim(Vector3 worldPos)
	{
		base.transform.parent = Singleton<HandCardController>.Instance.OpponentPlaySpawnParent.transform;
		SetCardState(HandCardState.OpponentPlaying);
		Singleton<HandCardController>.Instance.RemoveOpponentCard(this);
		mCurrentScale = new Vector3(0.15f, 0.15f, 1f);
		if (!Singleton<DWGame>.Instance.IsTutorialSetup)
		{
			Singleton<SLOTAudioManager>.Instance.PlaySound("SFX_Card_Flip");
		}
		if (Singleton<TutorialController>.Instance.IsStateActive("IN_AITurn2a"))
		{
			Singleton<TutorialController>.Instance.AdvanceTutorialState();
			while (Singleton<TutorialController>.Instance.IsStateActive("IN_MysticCards"))
			{
				yield return null;
			}
		}
		if (worldPos.x == float.MinValue)
		{
			yield return new WaitForSeconds(0.5f);
		}
		else
		{
			Vector3 screenPos = Singleton<DWGameCamera>.Instance.MainCam.WorldToScreenPoint(worldPos);
			mTargetPosition = ScreenPosToCardPos(screenPos);
			mTargetRotation = Quaternion.Euler(90f - Singleton<DWGameCamera>.Instance.MainCam.transform.rotation.eulerAngles.x, 0f, 0f);
			mTargetScale = new Vector3(0.65f, 0.65f, 1f);
			mSnapSpeed = 10f;
			Singleton<SLOTAudioManager>.Instance.PlaySound("SFX_Card_Whoosh");
			if (!Singleton<DWGame>.Instance.IsTutorialSetup)
			{
				yield return new WaitForSeconds(0.5f);
				PlayCardFadeFX();
			}
		}
		mTargetAlpha = 0f;
		mSnapSpeed = 10f;
		if (!Singleton<DWGame>.Instance.IsTutorialSetup)
		{
			yield return new WaitForSeconds(0.2f);
		}
		NGUITools.Destroy(base.gameObject);
	}

	public IEnumerator ShowUserCreaturePullAnim(GameObject creatureTarget, float zOffset = 0f)
	{
		if (creatureTarget != null)
		{
			yield return StartCoroutine(ShowUserCreaturePullAnim(creatureTarget.transform.position + new Vector3(0f, 0f, zOffset)));
		}
		else
		{
			yield return StartCoroutine(ShowUserCreaturePullAnim(new Vector3(float.MinValue, float.MinValue, float.MinValue)));
		}
	}

	public IEnumerator ShowUserCreaturePullAnim(Vector3 worldPos)
	{
		base.transform.parent = Singleton<HandCardController>.Instance.HandCreatureCardsSpawnParent.transform;
		SetCardState(HandCardState.Playing);
		mTargetAlpha = 1f;
		Singleton<HandCardController>.Instance.RemoveHandCard(this);
		if (!Singleton<DWGame>.Instance.IsTutorialSetup)
		{
			Singleton<SLOTAudioManager>.Instance.PlaySound("SFX_Card_Flip");
		}
		if (worldPos.x == float.MinValue)
		{
			yield return new WaitForSeconds(0.5f);
		}
		else
		{
			Vector3 screenPos = Singleton<DWGameCamera>.Instance.MainCam.WorldToScreenPoint(worldPos);
			mTargetPosition = ScreenPosToCardPos(screenPos);
			mTargetRotation = Quaternion.Euler(90f - Singleton<DWGameCamera>.Instance.MainCam.transform.rotation.eulerAngles.x, 0f, 0f);
			mTargetScale = new Vector3(0.65f, 0.65f, 1f);
			mSnapSpeed = 10f;
			Singleton<SLOTAudioManager>.Instance.PlaySound("SFX_Card_Whoosh");
			if (!Singleton<DWGame>.Instance.IsTutorialSetup)
			{
				yield return new WaitForSeconds(0.5f);
				PlayCardFadeFX();
			}
		}
		mTargetAlpha = 0f;
		mSnapSpeed = 10f;
		if (!Singleton<DWGame>.Instance.IsTutorialSetup)
		{
			yield return new WaitForSeconds(0.2f);
		}
		NGUITools.Destroy(base.gameObject);
	}

	private void PlayCardFadeFX()
	{
		GameObject gameObject = (GameObject)SLOTGame.InstantiateFX(Singleton<BattleHudController>.Instance.CardReleaseFadeFXs[0], base.transform.position, base.transform.rotation);
		gameObject.transform.parent = base.transform.parent;
		gameObject.transform.localScale = base.transform.localScale;
		Singleton<BattleHudController>.Instance.CardDroppedPos = base.transform.position;
	}

	public void PlayLogZoomAnim()
	{
		SetCardState(HandCardState.LogZoom);
		Singleton<HandCardController>.Instance.ZoomCollider.gameObject.SetActive(true);
		Vector3 worldPos = BattleHudController.GetWorldPos(Input.mousePosition, Singleton<HandCardController>.Instance.BoardHeightAdjust);
		Vector3 screenPos = Singleton<DWGameCamera>.Instance.MainCam.WorldToScreenPoint(worldPos);
		base.transform.localPosition = ScreenPosToCardPos(screenPos);
		mCurrentScale = new Vector3(0f, 0f, 1f);
		base.transform.localScale = mCurrentScale;
		mSnapSpeed = 15f;
		Singleton<SLOTAudioManager>.Instance.PlaySound("SFX_Card_Whoosh");
	}

	public IEnumerator PlayLogZoomCloseAnim()
	{
		SetCardState(HandCardState.ClosingLogZoom);
		Singleton<HandCardController>.Instance.ClearLogZoomCard();
		Singleton<HandCardController>.Instance.ZoomCollider.gameObject.SetActive(false);
		yield return new WaitForSeconds(0.25f);
		NGUITools.Destroy(base.gameObject);
	}

	public void SetTargetSelection(GameObject creature)
	{
		mStoredHandPosition = mHandPosition;
		mStoredHandRotation = mHandRotation;
		mSelectingTarget = true;
		base.transform.parent = Singleton<HandCardController>.Instance.HandCardsTargetingParent;
		Vector3 position = creature.transform.position;
		position.x -= TargetSelectionPosOffset;
		Vector3 screenPos = Singleton<DWGameCamera>.Instance.MainCam.WorldToScreenPoint(position);
		mHandPosition = ScreenPosToCardPos(screenPos);
		mHandRotation = Quaternion.Euler(90f - Singleton<DWGameCamera>.Instance.MainCam.transform.rotation.eulerAngles.x, 0f, 0f);
		mHandScale = new Vector3(0.7f, 0.7f, 1f);
		mTargetScale = mHandScale;
		mSnapSpeed = 10f;
		Singleton<SLOTAudioManager>.Instance.PlaySound("SFX_Card_Whoosh");
	}

	public void UndoTargetSelection()
	{
		mSelectingTarget = false;
		SelectionVFX.SetActive(false);
		base.transform.parent = Singleton<HandCardController>.Instance.HandCardsSpawnParent.transform;
		mHandPosition = mStoredHandPosition;
		mHandRotation = mStoredHandRotation;
		mHandScale = Vector3.one;
		mTargetScale = mHandScale;
		mSnapSpeed = 8f;
	}

	public bool IsLeavingHand()
	{
		if (mState == HandCardState.Discarding || mState == HandCardState.ManualDiscarding)
		{
			return true;
		}
		if (mState == HandCardState.Playing || mState == HandCardState.OpponentPlaying)
		{
			return true;
		}
		return false;
	}

	public bool ZoomedIn()
	{
		return mState == HandCardState.ZoomedIn;
	}

	public bool CanPlay()
	{
		if (Singleton<DWGame>.Instance.InDeploymentPhase())
		{
			return Creature != null;
		}
		PlayerState playerState = Singleton<DWGame>.Instance.CurrentBoardState.GetPlayerState(PlayerType.User);
		int actionPoints = playerState.ActionPoints;
		if (Card != null)
		{
			if (playerState.GetCreatureCount() == 0)
			{
				return false;
			}
			return (int)Card.Cost <= actionPoints;
		}
		if (playerState.GetCreatureCount() >= MiscParams.CreaturesOnBoard)
		{
			return false;
		}
		return (int)mCreature.Form.DeployCost <= actionPoints;
	}

	public void SetPlayableIndicator(bool visible)
	{
		mUnplayableOffset = !visible;
		CanAffordIndicator.SetActive(visible);
	}

	private void DetermineDamageTargets()
	{
		StopDraggingDamagePredictions();
		if (mCard.AttackBase == AttackBase.None)
		{
			return;
		}
		List<DWBattleLaneObject> list = Singleton<DWBattleLane>.Instance.BattleLaneObjects[1];
		if (list.Count == 0)
		{
			return;
		}
		DWBattleLaneObject autoTargetForCard = Singleton<DWBattleLane>.Instance.GetAutoTargetForCard(mCard);
		if (autoTargetForCard != null)
		{
			mDamageTargets.Add(autoTargetForCard);
		}
		else if (mCard.Target2Group == AttackRange.All)
		{
			mDamageTargets.Add(list[0]);
		}
		else if (mCard.Target2Group == AttackRange.Triple)
		{
			if (list.Count > 2)
			{
				for (int i = 1; i < list.Count - 1; i++)
				{
					mDamageTargets.Add(list[i]);
				}
			}
			else
			{
				mDamageTargets.Add(list[0]);
			}
		}
		else
		{
			mDamageTargets.AddRange(list);
		}
	}

	private void StopDraggingDamagePredictions()
	{
		mDamageTargets.Clear();
		foreach (CreatureHPBar mActiveDamagePrediction in mActiveDamagePredictions)
		{
			mActiveDamagePrediction.SetPredictedDamage(0f);
		}
		mActiveDamagePredictions.Clear();
		mShowingDamageTargetIndex = 0;
	}

	private void UpdateDraggingDamagePredictions()
	{
		if (mState != HandCardState.DraggingOnBoard || mPrevHoveredLane == null)
		{
			StopDraggingDamagePredictions();
		}
		else
		{
			if (mDamageTargets.Count == 0)
			{
				return;
			}
			if (mActiveDamagePredictions.Count > 0)
			{
				if (mActiveDamagePredictions[0].FlashDamagePredictionOnceTween.AnyTweenPlaying())
				{
					return;
				}
				mShowingDamageTargetIndex++;
				if (mShowingDamageTargetIndex >= mDamageTargets.Count)
				{
					mShowingDamageTargetIndex = 0;
				}
				foreach (CreatureHPBar mActiveDamagePrediction in mActiveDamagePredictions)
				{
					mActiveDamagePrediction.SetPredictedDamage(0f);
				}
				mActiveDamagePredictions.Clear();
				return;
			}
			mActiveDamagePredictions.Clear();
			List<DWBattleLaneObject> list = Singleton<DWBattleLane>.Instance.BattleLaneObjects[1];
			Singleton<DWGame>.Instance.SetTarget(PlayerType.User, mDamageTargets[mShowingDamageTargetIndex].Creature);
			List<int> list2 = mPrevHoveredLane.Creature.PredictCardDamage(mCard);
			for (int i = 0; i < list2.Count; i++)
			{
				int num = list2[i];
				if (num > 0)
				{
					mActiveDamagePredictions.Add(list[i].HealthBar.FlashPredictedDamageOnce(num));
				}
			}
		}
	}

	public void SetScale(Vector3 scale)
	{
		mCurrentScale = scale;
		base.transform.localScale = scale;
	}

	public void SetAlpha(float alpha)
	{
		MyWidget.alpha = alpha;
	}

	public void SetTargetAlpha(float alpha, float snapSpeed)
	{
		mTargetAlpha = alpha;
		mAlphaSnapSpeed = snapSpeed;
	}

	public void SetTargetPosition(Vector3 pos)
	{
		mTargetPosition = pos;
	}
}
