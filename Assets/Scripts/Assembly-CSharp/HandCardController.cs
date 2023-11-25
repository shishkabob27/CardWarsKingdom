using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandCardController : Singleton<HandCardController>
{
	private enum HandCardEventType
	{
		Draw,
		FailedDraw,
		ForceDiscard
	}

	private class HandCardEvent
	{
		public PlayerType Player;

		public HandCardEventType Type;

		public CardData Card;

		public GameObject CreatureSource;

		public CreatureItem CardCreature;

		public CreatureItem CreatureInHandSource;

		public Vector3 Position;

		public int Depth;

		public Vector3 DrawFromPos;

		public HandCardEvent(PlayerType player, HandCardEventType type, CardData card, CreatureState creatureSource, CreatureItem cardCreature, Vector3 drawFromPos)
		{
			Player = player;
			Type = type;
			Card = card;
			CardCreature = cardCreature;
			DrawFromPos = drawFromPos;
			if (creatureSource != null)
			{
				DWBattleLaneObject laneObject = Singleton<DWBattleLane>.Instance.GetLaneObject(creatureSource);
				if (laneObject != null)
				{
					CreatureSource = laneObject.CreatureObject;
				}
			}
		}

		public HandCardEvent(PlayerType player, HandCardEventType type, CardData card, CreatureItem creatureInHandSource)
		{
			Player = player;
			Type = type;
			Card = card;
			CreatureInHandSource = creatureInHandSource;
		}
	}

	public bool Debug_PositionCards;

	public UITweenController ShowHandTween;

	public UITweenController HideHandTween;

	public UITweenController ShowOpponentTween;

	public UITweenController HideOpponentTween;

	public UITweenController ShowDiscardTween;

	public UITweenController HideDiscardTween;

	public UITweenController ShowCreatureHandTween;

	public UITweenController HideCreatureHandTween;

	public UITweenController ShowLogTargetTween;

	public UITweenController HideLogTargetTween;

	public GameObject HandCardsSpawnParent;

	public GameObject HandCreatureCardsSpawnParent;

	public Transform HandCardsTargetingParent;

	public UIWidget HandPosition;

	public UIWidget CreatureHandPosition;

	public Transform ZoomPosition;

	public Transform CreatureZoomPosition;

	public Transform OpponentPlayPosition;

	public GameObject BlockingHandCollider;

	public GameObject BlockingCreatureHandCollider;

	public GameObject OpponentCardsSpawnParent;

	public GameObject OpponentPlaySpawnParent;

	public UIWidget OpponentPosition;

	public UIWidget CreatureOpponentPosition;

	public Collider ZoomCollider;

	public Collider DeckCollider;

	public UILabel PlayedOnText;

	public UILabel PlayedOnTargetText;

	public GameObject DealtCardText;

	public float CardZDepth = -3173f;

	public float ScreenZAdjust = 668f;

	public float BoardHeightAdjust = 28.1f;

	private List<HandCardEvent> mCardEvents = new List<HandCardEvent>();

	private List<CardPrefabScript> mHandCards = new List<CardPrefabScript>();

	private List<CardPrefabScript> mHandCreatureCards = new List<CardPrefabScript>();

	private List<CardPrefabScript> mAllPlayerCards = new List<CardPrefabScript>();

	private List<CardPrefabScript> mOpponentCards = new List<CardPrefabScript>();

	private List<CardPrefabScript> mOpponentCreatureCards = new List<CardPrefabScript>();

	private bool mHandVisible = true;

	private bool mHandShouldBeVisible = true;

	private bool mCreatureHandVisible = true;

	private int mDeckSize;

	private int mPendingDeckSize;

	private CardPrefabScript mLogZoomCard;

	public float FullCardDrawTime = 1f;

	public float CardDrawDelay = 0.5f;

	public float CardDiscardDelay = 0.5f;

	public float OpponentCardDiscardDelay = 0.1f;

	public int CardSpeedupPoint = 4;

	public float DrawCardSpacing;

	public float MaxHandCardSpacing;

	public float OpponentCardSpacing;

	public float CreatureCardSpacing;

	public float OpponentCreatureCardSpacing;

	public float CurveAmount;

	public float AngleAmount;

	public int CurveAtCards;

	public float OpponentCurveAmount;

	public float OpponentAngleAmount;

	public float CreatureCardAngle;

	public float CreatureCardScale;

	private bool mShowingLogTarget;

	public int CardsMovingToDrawPos { get; set; }

	public int CardsArrivedAtDrawPos { get; set; }

	public List<CardPrefabScript> GetHandCards()
	{
		return mAllPlayerCards;
	}

	public List<CardPrefabScript> GetCreatureCards()
	{
		return mHandCreatureCards;
	}

	public List<CardPrefabScript> GetNonCreatureCards()
	{
		return mHandCards;
	}

	public List<CardPrefabScript> GetOpponentCreatureCards()
	{
		return mOpponentCreatureCards;
	}

	public List<CardPrefabScript> GetOpponentNonCreatureCards()
	{
		return mOpponentCards;
	}

	private void Awake()
	{
		ZoomPosition.SetLocalPositionZ(CardZDepth);
		CreatureZoomPosition.SetLocalPositionZ(CardZDepth);
		OpponentPlayPosition.SetLocalPositionZ(CardZDepth);
		BlockingHandCollider.transform.SetLocalPositionZ(CardZDepth);
		BlockingCreatureHandCollider.transform.SetLocalPositionZ(CardZDepth);
		ZoomCollider.transform.SetLocalPositionZ(CardZDepth);
	}

	public void ShowCreatureCardDraw(PlayerType player, CreatureItem creature)
	{
		ShowCardEvents(player, null, HandCardEventType.Draw, null, creature);
	}

	public void ShowCreatureCardDraw(PlayerType player, CreatureItem creature, Vector3 drawFromPos)
	{
		ShowCardEvents(player, null, HandCardEventType.Draw, null, creature, drawFromPos);
	}

	public void ShowCardDraw(PlayerType player, CardData card, CreatureState sourceCreature = null)
	{
		ShowCardEvents(player, card, HandCardEventType.Draw, sourceCreature);
	}

	public void ShowCardDraw(PlayerType player, CardData card, CreatureItem sourceCreature)
	{
		bool flag = mCardEvents.Count == 0;
		mCardEvents.Add(new HandCardEvent(player, HandCardEventType.Draw, card, sourceCreature));
		if (flag)
		{
			StartCoroutine(ProcessCardEventsCo());
		}
	}

	public void ShowFailedCardDraw(PlayerType player, CardData card, CreatureState sourceCreature = null)
	{
		ShowCardEvents(player, card, HandCardEventType.FailedDraw, sourceCreature);
	}

	public void ShowCardDiscard(PlayerType player, CardData card)
	{
		ShowCardEvents(player, card, HandCardEventType.ForceDiscard);
	}

	private void ShowCardEvents(PlayerType player, CardData card, HandCardEventType type, CreatureState creatureSource = null, CreatureItem cardCreature = null)
	{
		ShowCardEvents(player, card, type, creatureSource, cardCreature, new Vector3(float.MinValue, float.MinValue, float.MinValue));
	}

	private void ShowCardEvents(PlayerType player, CardData card, HandCardEventType type, CreatureState creatureSource, CreatureItem cardCreature, Vector3 drawFromPos)
	{
		bool flag = mCardEvents.Count == 0;
		mCardEvents.Add(new HandCardEvent(player, type, card, creatureSource, cardCreature, drawFromPos));
		if (flag)
		{
			StartCoroutine(ProcessCardEventsCo());
		}
	}

	public bool CardEventsInProgress()
	{
		return mCardEvents.Count > 0 || CardsMovingToDrawPos > 0;
	}

	private void Update()
	{
		if (Debug_PositionCards)
		{
			RefreshHandCardPositions();
			RefreshHandCreatureCardPositions();
			RefreshOpponentCardPositions();
			RefreshOpponentCreatureCardPositions();
		}
		if (mHandShouldBeVisible && !mHandVisible)
		{
			if (HideHandTween.AnyTweenPlaying())
			{
				HideHandTween.StopAndReset();
			}
			ShowHandTween.Play();
			mHandVisible = true;
		}
		else if (!mHandShouldBeVisible && mHandVisible)
		{
			if (ShowHandTween.AnyTweenPlaying())
			{
				ShowHandTween.StopAndReset();
			}
			HideHandTween.Play();
			mHandVisible = false;
		}
	}

	private IEnumerator ProcessCardEventsCo()
	{
		yield return null;
		int numDrawEvents = mCardEvents.FindAll((HandCardEvent m) => m.Type == HandCardEventType.Draw).Count;
		float screenWidth = 768f * Singleton<DWGameCamera>.Instance.MainCam.aspect;
		float spacing = DrawCardSpacing;
		float scale = screenWidth / (spacing * (float)numDrawEvents);
		if (scale > 1f)
		{
			scale = 1f;
		}
		spacing *= scale;
		int i = 0;
		foreach (HandCardEvent cardEvent2 in mCardEvents)
		{
			if (cardEvent2.Type == HandCardEventType.Draw)
			{
				float offset = (float)i * spacing - (float)(numDrawEvents - 1) * spacing / 2f;
				cardEvent2.Position = Singleton<HandCardController>.Instance.ZoomPosition.localPosition + new Vector3(offset, 0f, 0f);
				cardEvent2.Depth = i + 1;
				i++;
			}
		}
		float slowSpeed = 1f;
		float fastSpeed = 5f;
		bool speedUp = false;
		int layer = LayerMask.NameToLayer("3DGUI");
		int totalCount = 0;
		while (mCardEvents.Count != 0)
		{
			HandCardEvent cardEvent = mCardEvents[0];
			while (!Singleton<BattleHudController>.Instance.IsTurnIntroFinished() || (!Singleton<DWGame>.Instance.IsTutorialSetup && !mHandVisible && cardEvent.Player == PlayerType.User && Singleton<DWGame>.Instance.GetCurrentGameState().IsP1Turn()))
			{
				yield return null;
			}
			if (mCardEvents.Count >= CardSpeedupPoint)
			{
				speedUp = true;
			}
			float speedFactor = ((!speedUp) ? slowSpeed : fastSpeed);
			if (cardEvent.Type == HandCardEventType.Draw)
			{
				GameObject spawnParent2 = ((cardEvent.Player != PlayerType.User) ? OpponentCardsSpawnParent : ((cardEvent.Card == null) ? HandCreatureCardsSpawnParent : HandCardsSpawnParent));
				CardPrefabScript dragDrawCard2 = Singleton<DWGameMessageHandler>.Instance.RetrieveDragDrawCard();
				CardPrefabScript spawnedScript2;
				if (dragDrawCard2 != null)
				{
					spawnedScript2 = dragDrawCard2;
					dragDrawCard2.CanAffordIndicator.SetActive(false);
				}
				else
				{
					GameObject spawnedCard2 = spawnParent2.InstantiateAsChild(Singleton<PrefabReferences>.Instance.Card);
					spawnedCard2.ChangeLayer(layer);
					spawnedScript2 = spawnedCard2.GetComponent<CardPrefabScript>();
				}
				if (cardEvent.Player == PlayerType.User)
				{
					mAllPlayerCards.Add(spawnedScript2);
					spawnedScript2.Mode = CardPrefabScript.CardMode.Hand;
					spawnedScript2.SetPlayableIndicator(false);
					if (cardEvent.Card != null)
					{
						spawnedScript2.Populate(cardEvent.Card);
					}
					else
					{
						spawnedScript2.Populate(cardEvent.CardCreature);
					}
				}
				else
				{
					spawnedScript2.Mode = CardPrefabScript.CardMode.Opponent;
					if (cardEvent.Card != null)
					{
						spawnedScript2.Populate(cardEvent.Card);
					}
					else
					{
						spawnedScript2.Populate(cardEvent.CardCreature);
					}
				}
				if (cardEvent.Card != null)
				{
					GameObject sourceObject = null;
					if (cardEvent.CreatureSource != null)
					{
						sourceObject = cardEvent.CreatureSource;
					}
					else if (cardEvent.CreatureInHandSource != null)
					{
						CardPrefabScript sourceCard = ((cardEvent.Player != PlayerType.User) ? mOpponentCreatureCards.Find((CardPrefabScript m) => m.Creature == cardEvent.CreatureInHandSource) : mHandCreatureCards.Find((CardPrefabScript m) => m.Creature == cardEvent.CreatureInHandSource));
						if (sourceCard != null)
						{
							sourceObject = sourceCard.gameObject;
						}
					}
					StartCoroutine(spawnedScript2.PlayFastDrawAnim(sourceObject, cardEvent.Position, scale, cardEvent.Depth));
				}
				else
				{
					StartCoroutine(spawnedScript2.PlayCreatureDrawAnim(cardEvent.DrawFromPos));
				}
				if (!Singleton<DWGame>.Instance.IsTutorialSetup)
				{
					yield return new WaitForSeconds(CardDrawDelay / speedFactor);
				}
			}
			else if (cardEvent.Type == HandCardEventType.FailedDraw)
			{
				CardPrefabScript dragDrawCard = Singleton<DWGameMessageHandler>.Instance.RetrieveDragDrawCard();
				CardPrefabScript spawnedScript;
				if (dragDrawCard != null)
				{
					spawnedScript = dragDrawCard;
					dragDrawCard.CanAffordIndicator.SetActive(false);
				}
				else
				{
					GameObject spawnParent = ((cardEvent.Player != PlayerType.User) ? OpponentCardsSpawnParent : HandCardsSpawnParent);
					GameObject spawnedCard = spawnParent.InstantiateAsChild(Singleton<PrefabReferences>.Instance.Card);
					spawnedCard.ChangeLayer(layer);
					spawnedScript = spawnedCard.GetComponent<CardPrefabScript>();
				}
				if (cardEvent.Player == PlayerType.User)
				{
					spawnedScript.Mode = CardPrefabScript.CardMode.Hand;
					spawnedScript.Populate(cardEvent.Card);
				}
				else
				{
					spawnedScript.Mode = CardPrefabScript.CardMode.Opponent;
					spawnedScript.Populate(cardEvent.Card);
				}
				StartCoroutine(spawnedScript.PlayFailedDrawAnim(totalCount, cardEvent.CreatureSource));
				yield return new WaitForSeconds(CardDrawDelay / speedFactor);
			}
			else if (cardEvent.Type == HandCardEventType.ForceDiscard)
			{
				if (cardEvent.Player == PlayerType.User)
				{
					CardPrefabScript prefab2 = mHandCards.Find((CardPrefabScript match) => match.Card == cardEvent.Card && !match.IsLeavingHand());
					if (prefab2 != null)
					{
						StartCoroutine(prefab2.Discard());
						yield return new WaitForSeconds(CardDiscardDelay / speedFactor);
					}
				}
				else
				{
					CardPrefabScript prefab = mOpponentCards.Find((CardPrefabScript match) => match.Card == cardEvent.Card && !match.IsLeavingHand());
					if (prefab != null)
					{
						StartCoroutine(prefab.Discard());
						yield return new WaitForSeconds(OpponentCardDiscardDelay / speedFactor);
					}
				}
			}
			mCardEvents.RemoveAt(0);
			totalCount++;
		}
	}

	public void AddHandCard(CardPrefabScript card)
	{
		if (card.Creature != null)
		{
			mHandCreatureCards.Add(card);
			RefreshHandCreatureCardPositions();
		}
		else
		{
			mHandCards.Add(card);
			RefreshHandCardPositions();
			Singleton<CharacterAnimController>.Instance.Add3DHoldCard(PlayerType.User);
		}
	}

	public void RemoveHandCard(CardPrefabScript card)
	{
		if (card.Creature != null)
		{
			mHandCreatureCards.Remove(card);
			mAllPlayerCards.Remove(card);
			RefreshHandCreatureCardPositions();
		}
		else
		{
			mAllPlayerCards.Remove(card);
			mHandCards.Remove(card);
			RefreshHandCardPositions();
			Singleton<CharacterAnimController>.Instance.Remove3DHoldCard(PlayerType.User);
		}
	}

	public void AddOpponentCard(CardPrefabScript card)
	{
		if (card.Creature != null)
		{
			mOpponentCreatureCards.Add(card);
			RefreshOpponentCreatureCardPositions();
		}
		else
		{
			mOpponentCards.Add(card);
			RefreshOpponentCardPositions();
			Singleton<CharacterAnimController>.Instance.Add3DHoldCard(PlayerType.Opponent);
		}
	}

	public void RemoveOpponentCard(CardPrefabScript card)
	{
		if (card.Creature != null)
		{
			mOpponentCreatureCards.Remove(card);
			RefreshOpponentCreatureCardPositions();
			return;
		}
		if (!mOpponentCards.Contains(card))
		{
			string text = ((card.Card == null) ? card.Creature.Form.ID : card.Card.ID);
		}
		mOpponentCards.Remove(card);
		RefreshOpponentCardPositions();
	}

	public CardPrefabScript GetCardPrefab(string cardId)
	{
		return mAllPlayerCards.Find((CardPrefabScript match) => match.Card.ID == cardId);
	}

	public IEnumerator ShowUserCardPlayAnimation(CreatureItem creature)
	{
		while (Singleton<PauseController>.Instance.Paused)
		{
			yield return null;
		}
		CreatureItem creature2 = default(CreatureItem);
		CardPrefabScript cardPrefab = mAllPlayerCards.Find((CardPrefabScript m) => m.Creature == creature2);
		if (cardPrefab != null)
		{
			yield return StartCoroutine(cardPrefab.ShowPlayAnim());
		}
	}

	public IEnumerator ShowUserCardPlayAnimation(CardData card)
	{
		while (Singleton<PauseController>.Instance.Paused)
		{
			yield return null;
		}
		CardData card2 = default(CardData);
		CardPrefabScript cardPrefab = mAllPlayerCards.Find((CardPrefabScript m) => m.Card == card2);
		if (cardPrefab != null)
		{
			yield return StartCoroutine(cardPrefab.ShowPlayAnim());
		}
	}

	private void RefreshHandCardPositions()
	{
		if (mHandCards.Count != 0)
		{
			Vector3 localScale = base.transform.localScale;
			float num = HandPosition.width;
			int num2 = (int)(num / MaxHandCardSpacing);
			float num3 = MaxHandCardSpacing;
			if (mHandCards.Count > num2 && mHandCards.Count > 1)
			{
				num3 = num / (float)mHandCards.Count;
			}
			float num4 = 0f;
			if (mHandCards.Count >= CurveAtCards)
			{
				int num5 = 1 + mHandCards.Count - CurveAtCards;
				num4 = CurveAmount * (float)num5;
			}
			float num6 = num3 * (float)mHandCards.Count / 2f;
			for (int i = 0; i < mHandCards.Count; i++)
			{
				float num7 = num3 * ((float)i + 0.5f);
				float num8 = (num7 - num6) / num6;
				float num9 = (0f - num4) * num8 * num8;
				num7 = (num7 + HandPosition.transform.localPosition.x - num6) * localScale.x;
				num9 = (num9 + HandPosition.transform.localPosition.y) * localScale.y;
				Vector3 nguiPos = new Vector3(num7, num9);
				float num10 = -2f * num4 * num8 / num6;
				float angle = AngleAmount * (float)(Math.Atan(num10) * 180.0 / Math.PI);
				mHandCards[i].SetHandLocation(nguiPos, angle, i + 1);
			}
		}
	}

	private void RefreshHandCreatureCardPositions()
	{
		if (mHandCreatureCards.Count != 0)
		{
			Vector3 localScale = base.transform.localScale;
			float num = CreatureCardSpacing;
			if (num > (float)(CreatureHandPosition.height / mHandCreatureCards.Count))
			{
				num = CreatureHandPosition.height / mHandCreatureCards.Count;
			}
			for (int i = 0; i < mHandCreatureCards.Count; i++)
			{
				float num2 = 0f;
				float num3 = num * (float)i;
				num2 = (num2 + CreatureHandPosition.transform.localPosition.x) * localScale.x;
				num3 = (num3 + CreatureHandPosition.transform.localPosition.y) * localScale.y;
				Vector3 nguiPos = new Vector3(num2, num3);
				mHandCreatureCards[i].SetHandLocation(nguiPos, CreatureCardAngle, mHandCreatureCards.Count - i);
			}
		}
	}

	private void RefreshOpponentCardPositions()
	{
		if (mOpponentCards.Count != 0)
		{
			Vector3 localScale = base.transform.localScale;
			float num = OpponentPosition.transform.localScale.x * (float)OpponentPosition.width / (float)mOpponentCards.Count;
			if (num > OpponentCardSpacing)
			{
				num = OpponentCardSpacing;
			}
			float num2 = 0f;
			if (mOpponentCards.Count >= CurveAtCards)
			{
				int num3 = 1 + mOpponentCards.Count - CurveAtCards;
				num2 = OpponentCurveAmount * (float)num3;
			}
			float num4 = num * (float)mOpponentCards.Count / 2f;
			for (int i = 0; i < mOpponentCards.Count; i++)
			{
				float num5 = num * ((float)i + 0.5f);
				float num6 = (num5 - num4) / num4;
				float num7 = (0f - num2) * num6 * num6;
				num5 = (num5 + OpponentPosition.transform.localPosition.x - num4) * localScale.x;
				num7 = (num7 + OpponentPosition.transform.localPosition.y) * localScale.y;
				Vector3 nguiPos = new Vector3(num5, num7);
				float num8 = -2f * num2 * num6 / num4;
				float angle = (0f - OpponentAngleAmount) * (float)(Math.Atan(num8) * 180.0 / Math.PI);
				mOpponentCards[i].SetHandLocation(nguiPos, angle, i + 1);
			}
		}
	}

	private void RefreshOpponentCreatureCardPositions()
	{
		if (mOpponentCreatureCards.Count != 0)
		{
			Vector3 localScale = base.transform.localScale;
			float num = (float)CreatureOpponentPosition.height * CreatureOpponentPosition.transform.localScale.y;
			float num2 = OpponentCreatureCardSpacing;
			if (num2 * (float)mOpponentCreatureCards.Count > num)
			{
				num2 = num / (float)mOpponentCreatureCards.Count;
			}
			for (int i = 0; i < mOpponentCreatureCards.Count; i++)
			{
				float num3 = 0f;
				float num4 = (0f - num2) * (float)i;
				num3 = (num3 + CreatureOpponentPosition.transform.localPosition.x) * localScale.x;
				num4 = (num4 + CreatureOpponentPosition.transform.localPosition.y) * localScale.y;
				Vector3 nguiPos = new Vector3(num3, num4);
				mOpponentCreatureCards[i].SetHandLocation(nguiPos, CreatureCardAngle, i + 1);
			}
		}
	}

	public void ShowHand()
	{
		mHandShouldBeVisible = true;
	}

	public void HideHand()
	{
		mHandShouldBeVisible = false;
	}

	public void ShowCreatureHand()
	{
		if (!mCreatureHandVisible)
		{
			mCreatureHandVisible = true;
			ShowCreatureHandTween.Play();
		}
	}

	public void HideCreatureHand()
	{
		if (mCreatureHandVisible)
		{
			mCreatureHandVisible = false;
			HideCreatureHandTween.Play();
		}
	}

	public void OnShowTweenFinished()
	{
	}

	public IEnumerator ShowOpponentPlayAnim(CardData card, GameObject creatureTarget)
	{
		VOEvent voEvent = ((!card.IsLeaderCard) ? VOEvent.PlayCard : VOEvent.PlayLeaderCard);
		Singleton<SLOTAudioManager>.Instance.TriggerVOEvent(Singleton<DWGame>.Instance.GetCharacter(PlayerType.Opponent), voEvent);
		CardData card2 = default(CardData);
		CardPrefabScript prefab = mOpponentCards.Find((CardPrefabScript match) => match.Card == card2 && !match.IsLeavingHand());
		if (prefab != null)
		{
			yield return StartCoroutine(prefab.ShowOpponentPlayAnim(creatureTarget, 0f));
		}
	}

	public IEnumerator ShowOpponentPlayAnim(CreatureItem creature, int laneIndex)
	{
		while (Singleton<PauseController>.Instance.Paused)
		{
			yield return null;
		}
		if (!Singleton<DWGame>.Instance.IsTutorialSetup)
		{
			Singleton<SLOTAudioManager>.Instance.TriggerVOEvent(Singleton<DWGame>.Instance.GetCharacter(PlayerType.Opponent), VOEvent.PlayCreature, creature.Faction);
		}
		CreatureItem creature2 = default(CreatureItem);
		CardPrefabScript prefab = mOpponentCreatureCards.Find((CardPrefabScript match) => match.Creature == creature2 && !match.IsLeavingHand());
		if (!(prefab != null))
		{
			yield break;
		}
		List<DWBattleLaneObject> lanes = Singleton<DWBattleLane>.Instance.BattleLaneObjects[1];
		if (lanes.Count == 0)
		{
			yield return StartCoroutine(prefab.ShowOpponentPlayAnim(new Vector3(0f - Singleton<DWBattleLane>.Instance.CreatureOffsetX, 0f, 0f)));
			yield break;
		}
		float laneSpacing2 = ((lanes.Count != 1) ? (lanes[0].transform.position.z - lanes[1].transform.position.z) : (Singleton<DWBattleLane>.Instance.MinimumCreatureWidth + Singleton<DWBattleLane>.Instance.CreatureSpacing));
		laneSpacing2 /= 2f;
		if (laneIndex == 0)
		{
			yield return StartCoroutine(prefab.ShowOpponentPlayAnim(lanes[0].CreatureObject, laneSpacing2));
		}
		else
		{
			yield return StartCoroutine(prefab.ShowOpponentPlayAnim(lanes[laneIndex - 1].CreatureObject, 0f - laneSpacing2));
		}
	}

	public IEnumerator ShowUserCreaturePullAnim(CreatureItem creature, int laneIndex)
	{
		CreatureItem creature2 = default(CreatureItem);
		CardPrefabScript prefab = mHandCreatureCards.Find((CardPrefabScript match) => match.Creature == creature2 && !match.IsLeavingHand());
		if (!(prefab != null))
		{
			yield break;
		}
		List<DWBattleLaneObject> lanes = Singleton<DWBattleLane>.Instance.BattleLaneObjects[0];
		if (lanes.Count == 0)
		{
			yield return StartCoroutine(prefab.ShowUserCreaturePullAnim(new Vector3(0f - Singleton<DWBattleLane>.Instance.CreatureOffsetX, 0f, 0f)));
			yield break;
		}
		float laneSpacing2 = ((lanes.Count != 1) ? (lanes[0].transform.position.z - lanes[1].transform.position.z) : (Singleton<DWBattleLane>.Instance.MinimumCreatureWidth + Singleton<DWBattleLane>.Instance.CreatureSpacing));
		laneSpacing2 /= 2f;
		if (laneIndex == 0)
		{
			yield return StartCoroutine(prefab.ShowUserCreaturePullAnim(lanes[0].CreatureObject, laneSpacing2));
		}
		else
		{
			yield return StartCoroutine(prefab.ShowUserCreaturePullAnim(lanes[laneIndex - 1].CreatureObject, 0f - laneSpacing2));
		}
	}

	public void UnzoomCard()
	{
		foreach (CardPrefabScript mHandCard in mHandCards)
		{
			mHandCard.Unzoom();
		}
		foreach (CardPrefabScript mHandCreatureCard in mHandCreatureCards)
		{
			mHandCreatureCard.Unzoom();
		}
		if (mLogZoomCard != null)
		{
			StartCoroutine(mLogZoomCard.PlayLogZoomCloseAnim());
		}
	}

	public void CancelCardDrag()
	{
		CardPrefabScript.CancelCardDrag();
	}

	public void ClearPlayerHand()
	{
		foreach (CardPrefabScript mHandCard in mHandCards)
		{
			NGUITools.Destroy(mHandCard.gameObject);
		}
		mHandCards.Clear();
		mAllPlayerCards.Clear();
		foreach (CardPrefabScript mHandCreatureCard in mHandCreatureCards)
		{
			NGUITools.Destroy(mHandCreatureCard.gameObject);
		}
		mHandCreatureCards.Clear();
		Singleton<CharacterAnimController>.Instance.Reset3DHoldCards(PlayerType.User);
		mPendingDeckSize = 0;
		mDeckSize = 0;
	}

	public void ClearOpponentHand()
	{
		foreach (CardPrefabScript mOpponentCard in mOpponentCards)
		{
			NGUITools.Destroy(mOpponentCard.gameObject);
		}
		mOpponentCards.Clear();
		Singleton<CharacterAnimController>.Instance.Reset3DHoldCards(PlayerType.Opponent);
	}

	public void OnLogCardClicked(CardData card, CreatureItem creature, CreatureItem creatureTarget)
	{
		GameObject gameObject = HandCardsSpawnParent.InstantiateAsChild(Singleton<PrefabReferences>.Instance.Card);
		gameObject.ChangeLayer(base.gameObject.layer);
		mLogZoomCard = gameObject.GetComponent<CardPrefabScript>();
		mLogZoomCard.Mode = CardPrefabScript.CardMode.Hand;
		mLogZoomCard.AdjustDepth(11);
		if (card != null)
		{
			mLogZoomCard.Populate(card);
		}
		else
		{
			mLogZoomCard.Populate(creature);
		}
		mShowingLogTarget = creatureTarget != null;
		if (mShowingLogTarget)
		{
			ShowLogTargetTween.Play();
			if (creature == creatureTarget)
			{
				DealtCardText.SetActive(true);
				PlayedOnText.text = string.Empty;
				PlayedOnTargetText.text = string.Empty;
			}
			else
			{
				DealtCardText.SetActive(false);
				if (card != null)
				{
					PlayedOnText.text = KFFLocalization.Get("!!PLAYED_ON");
				}
				else
				{
					PlayedOnText.text = KFFLocalization.Get("!!ATTACKED");
				}
				PlayedOnTargetText.text = creatureTarget.Form.Name;
			}
		}
		mLogZoomCard.PlayLogZoomAnim();
	}

	public void ClearLogZoomCard()
	{
		mLogZoomCard = null;
		if (mShowingLogTarget)
		{
			HideLogTargetTween.Play();
		}
	}

	public CardPrefabScript GetZoomedCard()
	{
		foreach (CardPrefabScript mHandCard in mHandCards)
		{
			if (mHandCard.ZoomedIn())
			{
				return mHandCard;
			}
		}
		return null;
	}

	public CardPrefabScript GetZoomedCreatureCard()
	{
		foreach (CardPrefabScript mHandCreatureCard in mHandCreatureCards)
		{
			if (mHandCreatureCard.ZoomedIn())
			{
				return mHandCreatureCard;
			}
		}
		return null;
	}

	public void OnCardDragStart()
	{
		BlockingHandCollider.SetActive(true);
		if (GetCreatureCards().Count > 0 && mCreatureHandVisible)
		{
			BlockingCreatureHandCollider.SetActive(true);
		}
	}

	public void OnCardDragFinish()
	{
		BlockingHandCollider.SetActive(false);
		BlockingCreatureHandCollider.SetActive(false);
	}

	public CardPrefabScript CreateDragDrawCard()
	{
		Singleton<SLOTAudioManager>.Instance.PlaySound("card/SFX_Card_Choose1");
		CardPrefabScript component = HandCardsSpawnParent.InstantiateAsChild(Singleton<PrefabReferences>.Instance.Card).GetComponent<CardPrefabScript>();
		component.gameObject.ChangeLayer(base.gameObject.layer);
		component.DragDrawCard = true;
		component.Mode = CardPrefabScript.CardMode.Hand;
		component.SetCardState(CardPrefabScript.HandCardState.DraggingInHand);
		component.ShowBack();
		component.CanAffordIndicator.SetActive(true);
		component.SetScale(Vector3.one * 0.5f);
		component.SetAlpha(0f);
		return component;
	}
}
