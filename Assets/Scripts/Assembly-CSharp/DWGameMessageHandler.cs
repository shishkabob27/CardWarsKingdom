using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DWGameMessageHandler : Singleton<DWGameMessageHandler>
{
	private enum HitReactType
	{
		None,
		Small,
		Big
	}

	private class CreatureHit
	{
		public float RawDamage;

		public float CappedDamage;

		public bool Crit;

		public bool Miss;

		public bool Shield;

		public DamageType HitType;

		public HitReactType HeroReactType;

		public bool Killing;

		public CreatureHit(GameMessage attackMessage, GameMessage damageMessage, bool killing)
		{
			Crit = attackMessage.IsCritical;
			Miss = attackMessage.IsMiss;
			Shield = attackMessage.IsShield;
			HeroReactType = HitReactType.Small;
			Killing = false;
			if (damageMessage != null)
			{
				if (damageMessage.Creature != null)
				{
					if (killing)
					{
						HeroReactType = HitReactType.Big;
						Killing = true;
					}
					else
					{
						HeroReactType = HitReactType.Small;
						Killing = false;
					}
				}
				RawDamage = damageMessage.RawAmount;
				CappedDamage = damageMessage.Amount;
				if (damageMessage.MagicDamage && damageMessage.PhysicalDamage)
				{
					HitType = DamageType.Hybrid;
				}
				else if (damageMessage.PhysicalDamage)
				{
					HitType = DamageType.Physical;
				}
				else
				{
					HitType = DamageType.Magic;
				}
				damageMessage.Presented = true;
			}
			else
			{
				RawDamage = (CappedDamage = 0f);
				HitType = DamageType.Physical;
			}
		}
	}

	private class CreatureAttackTarget
	{
		public CreatureState Creature;

		public List<CreatureHit> Hits = new List<CreatureHit>();
	}

	private class CreatureAttacker
	{
		public CreatureState Creature;

		public List<CreatureAttackTarget> Targets = new List<CreatureAttackTarget>();
	}

	private const float Y_POSITION_ADJUST = 2f;

	private const float MELEE_POSITION_OFFSET = 5f;

	public Color StatusTextBuffColor;

	public Color StatusTextDebuffColor;

	public Color PassiveTextColor;

	public float HitFlashMatDuration = 0.2f;

	public float TimeBetweenTeamAttacks = 0.7f;

	public float RangedAttackFinishTime = 0.7f;

	public float RotateToAttackTime = 0.3f;

	public Material HitFlashMaterial;

	public Material FreezeDesatMaterial;

	public Material StealthMaterial;

	public GameObject TransmogrifyCreature;

	public float CritCamYOffset;

	public float CritCamZOffset;

	public float CritCamLookAtYOffset;

	public float NormalCameraShake;

	public float NormalCameraShakeTime;

	public float CritCameraShake;

	public float CritCameraShakeTime;

	public GameObject NonLaneActionStartFXPrefab;

	public GameObject EffectNameDisplayPrefab;

	public GameObject ShieldHitFX;

	public bool PrintDebugActionName;

	public Transform ProjectileFollower;

	public Transform BoardCenterPos;

	public Transform PlayerLaneCenterPos;

	public Transform OpponentLaneCenterPos;

	private bool mProcessingMessages;

	private List<GameMessage> mQueuedMessages = new List<GameMessage>();

	private bool[] mHeroCardAlreadyPlayed = new bool[2];

	private bool mInBattleFinishMoment;

	private int mCreatureAttacksInProgress;

	private List<Transform> mFollowingProjectiles = new List<Transform>();

	private CardPrefabScript mDragDrawCard;

	private float mEffectFinishTimer = -1f;

	public bool IsEffectDone()
	{
		return !mProcessingMessages && mQueuedMessages.Count == 0 && CardProgress.Instance.State == CardState.Idle && !AttackProgress.AttacksInProgress;
	}

	public void ProcessMessages(List<GameMessage> messages)
	{
		if (mQueuedMessages.Count == 0 && !Singleton<PauseController>.Instance.Paused)
		{
			UICamera.LockInput();
		}
		int count = mQueuedMessages.Count;
		mQueuedMessages.AddRange(messages);
		for (int i = count; i < mQueuedMessages.Count; i++)
		{
			mQueuedMessages[i].Index = i;
		}
		CheckMessageQueue();
	}

	private void CheckMessageQueue()
	{
		if (!mProcessingMessages && mQueuedMessages.Count > 0 && CardProgress.Instance.State == CardState.Idle && !AttackProgress.AttacksInProgress)
		{
			StartCoroutine(PlayEffectSequence());
		}
	}

	private void Update()
	{
		if (!Singleton<PauseController>.Instance.Paused)
		{
			CheckMessageQueue();
			UpdateProjectileFollower();
			if (mEffectFinishTimer > 0f)
			{
				mEffectFinishTimer -= Time.deltaTime;
			}
		}
	}

	private GameMessage GetDamageMessage(GameMessage attackMessage)
	{
		for (int i = attackMessage.Index + 1; i < mQueuedMessages.Count; i++)
		{
			if (mQueuedMessages[i].Action == GameEvent.DAMAGE_CREATURE)
			{
				return mQueuedMessages[i];
			}
			if (mQueuedMessages[i].Action == GameEvent.ATTACK_HIT_END)
			{
				break;
			}
		}
		return null;
	}

	private IEnumerator PlayEffectSequence()
	{
		mProcessingMessages = true;
		GameState currentState = Singleton<DWGame>.Instance.GetCurrentGameState();
		Singleton<DWBattleLane>.Instance.HideAvailableActionIndicators();
		bool anyCreaturesDied = false;
		bool anyCreaturesPushed = false;
		bool dragAttackHappened = false;
		bool cardAttackHappened = false;
		bool cardPlayed = false;
		bool turnStarted = false;
		bool gameEnded = false;
		HitReactType hitReactType = HitReactType.None;
		PlayerType playHitReactOnPlayer3 = null;
		bool moreHitsToComeInThisAttack = false;
		bool moreAttacksToCome = false;
		bool inCritAttack = false;
		CreatureState currentAttackTarget = null;
		for (int i = 0; i < mQueuedMessages.Count; i++)
		{
			GameMessage message = mQueuedMessages[i];
			if (message.Presented)
			{
				continue;
			}
			if (!Singleton<PlayerInfoScript>.Instance.StateData.MultiplayerMode)
			{
				while (Singleton<PauseController>.Instance.Paused)
				{
					yield return null;
				}
			}
			if (message.Action == GameEvent.START_TURN)
			{
				if (currentState.IsP1Turn())
				{
					Singleton<BattleHudController>.Instance.OnStartP1Turn();
					Singleton<TutorialController>.Instance.AdvanceIfOnEnemyTurn();
				}
				else
				{
					Singleton<BattleHudController>.Instance.OnStartP2Turn();
				}
				while (Singleton<BattleHudController>.Instance.IsBannerTweenPlaying())
				{
					yield return null;
				}
				turnStarted = true;
			}
			else if (message.Action == GameEvent.GAIN_ACTION_POINTS)
			{
				Singleton<BattleHudController>.Instance.AdjustActionPoints(message.WhichPlayer.Type, (int)message.Amount);
			}
			else if (message.Action == GameEvent.SPEND_ACTION_POINTS || message.Action == GameEvent.LOSE_ACTION_POINTS)
			{
				Singleton<BattleHudController>.Instance.AdjustActionPoints(message.WhichPlayer.Type, -(int)message.Amount);
			}
			else if (message.Action == GameEvent.CARD_PLAYED)
			{
				cardPlayed = true;
				yield return StartCoroutine(ShowCardPlayEffect(message, mQueuedMessages));
			}
			else if (message.Action == GameEvent.CREATURE_DEPLOYED)
			{
				cardPlayed = true;
				if (!message.IsDrag && !Singleton<DWGame>.Instance.IsTutorialSetup)
				{
					if (message.WhichPlayer.Type == PlayerType.User)
					{
						yield return StartCoroutine(Singleton<HandCardController>.Instance.ShowUserCreaturePullAnim(message.Creature.Data, message.Lane.Index));
					}
					else
					{
						yield return StartCoroutine(Singleton<HandCardController>.Instance.ShowOpponentPlayAnim(message.Creature.Data, message.Lane.Index));
					}
					Singleton<DWGame>.Instance.DeployCreatureFromCard(message.WhichPlayer.Type, message.Creature.Data, message.Lane.Index, true);
				}
			}
			else if (message.Action == GameEvent.ATTACK_START)
			{
				currentAttackTarget = null;
				for (int ii4 = i + 1; ii4 < mQueuedMessages.Count; ii4++)
				{
					if (mQueuedMessages[ii4].Action == GameEvent.CREATURE_ATTACKED && mQueuedMessages[ii4].IsCritical && !mQueuedMessages[ii4].IsCounter)
					{
						inCritAttack = true;
						break;
					}
					if (mQueuedMessages[ii4].Action == GameEvent.ATTACK_END)
					{
						inCritAttack = false;
						break;
					}
				}
				moreAttacksToCome = false;
				for (int ii3 = i + 1; ii3 < mQueuedMessages.Count - 1 && (mQueuedMessages[ii3].Action != GameEvent.CREATURE_ATTACKED || !mQueuedMessages[ii3].IsCounter); ii3++)
				{
					if (mQueuedMessages[ii3].Action == GameEvent.ATTACK_START && mQueuedMessages[ii3 + 1].Action != GameEvent.ATTACK_END)
					{
						if (mQueuedMessages[ii3].Creature.Owner == message.Creature.Owner)
						{
							moreAttacksToCome = true;
						}
						break;
					}
				}
			}
			else if (message.Action == GameEvent.ATTACK_HIT_START)
			{
				List<GameMessage> groupedMessages4 = new List<GameMessage>();
				CreatureState creatureTarget = null;
				bool isCounterAttack = false;
				for (int ii2 = i + 1; ii2 < mQueuedMessages.Count; ii2++)
				{
					if (mQueuedMessages[ii2].Action == GameEvent.CREATURE_ATTACKED && creatureTarget == null)
					{
						creatureTarget = mQueuedMessages[ii2].SecondCreature;
					}
					bool counterattackStart = false;
					if (mQueuedMessages[ii2].Action == GameEvent.ATTACK_START && mQueuedMessages[ii2].Creature.Owner != message.Creature.Owner)
					{
						counterattackStart = true;
					}
					if (counterattackStart || mQueuedMessages[ii2].Action == GameEvent.ATTACK_HIT_END)
					{
						if (counterattackStart)
						{
							moreHitsToComeInThisAttack = false;
							break;
						}
						for (int iii = ii2 + 1; iii < mQueuedMessages.Count; iii++)
						{
							if (mQueuedMessages[iii].Action == GameEvent.ATTACK_END)
							{
								moreHitsToComeInThisAttack = false;
								break;
							}
							if (mQueuedMessages[iii].Action != GameEvent.ATTACK_HIT_START)
							{
								continue;
							}
							if (mQueuedMessages[iii + 1].Action == GameEvent.CREATURE_ATTACKED)
							{
								if (mQueuedMessages[iii + 1].SecondCreature == creatureTarget)
								{
									moreHitsToComeInThisAttack = true;
									break;
								}
							}
							else if (mQueuedMessages[iii + 1].Action == GameEvent.AREA_ATTACK_START)
							{
								moreHitsToComeInThisAttack = false;
								break;
							}
						}
						break;
					}
					groupedMessages4.Add(mQueuedMessages[ii2]);
					if (mQueuedMessages[ii2].Action != GameEvent.CREATURE_ATTACKED || !mQueuedMessages[ii2].IsCounter)
					{
						continue;
					}
					isCounterAttack = true;
					for (int iii2 = ii2 + 1; iii2 < mQueuedMessages.Count; iii2++)
					{
						if (mQueuedMessages[iii2].Action == GameEvent.CREATURE_DIED && mQueuedMessages[iii2].Creature == mQueuedMessages[ii2].SecondCreature)
						{
							groupedMessages4.Add(mQueuedMessages[iii2]);
							break;
						}
						if (mQueuedMessages[iii2].Action == GameEvent.ATTACK_HIT_END)
						{
							break;
						}
					}
					break;
				}
				bool isDragAttack = !isCounterAttack && mQueuedMessages.Find((GameMessage m) => m.Action == GameEvent.CARD_PLAYED) == null;
				bool isAiDragAttack = isDragAttack && currentState.IsP2Turn();
				if (isDragAttack)
				{
					dragAttackHappened = true;
				}
				else
				{
					cardAttackHappened = true;
				}
				bool useCritCam = inCritAttack && !moreAttacksToCome;
				if (groupedMessages4.Find((GameMessage m) => m.Action == GameEvent.AREA_ATTACK_START) != null)
				{
					List<GameMessage> attackMessages = new List<GameMessage>();
					foreach (GameMessage j in groupedMessages4)
					{
						if (!j.Presented && j.Action == GameEvent.CREATURE_ATTACKED)
						{
							attackMessages.Add(j);
						}
						if (j.Action == GameEvent.AREA_ATTACK_END)
						{
							break;
						}
					}
					if (attackMessages.Count == 0)
					{
						continue;
					}
					CreatureAttacker attacker2 = new CreatureAttacker
					{
						Creature = attackMessages[0].Creature
					};
					CreatureState thisAttackTarget = null;
					foreach (GameMessage attackMessage3 in attackMessages)
					{
						attackMessage3.Presented = true;
						CreatureAttackTarget target2 = new CreatureAttackTarget
						{
							Creature = attackMessage3.SecondCreature
						};
						attacker2.Targets.Add(target2);
						if (thisAttackTarget == null)
						{
							thisAttackTarget = target2.Creature;
						}
						GameMessage damageMessage2 = GetDamageMessage(attackMessage3);
						if (damageMessage2 != null)
						{
							damageMessage2.Presented = true;
						}
						bool killingAttack2 = groupedMessages4.Find((GameMessage m) => m.Action == GameEvent.CREATURE_DIED && m.Creature == target2.Creature && !m.AutoReviveDeath) != null;
						CreatureHit hit2 = new CreatureHit(attackMessage3, damageMessage2, killingAttack2);
						target2.Hits.Add(hit2);
					}
					if (moreAttacksToCome)
					{
						StartCoroutine(CreatureAttack(attacker2, useCritCam, true, !moreHitsToComeInThisAttack, isAiDragAttack));
						yield return new WaitForSeconds(0.6f);
					}
					else
					{
						yield return StartCoroutine(CreatureAttack(attacker2, useCritCam, true, !moreHitsToComeInThisAttack, isAiDragAttack));
					}
				}
				else
				{
					GameMessage attackMessage2 = groupedMessages4.Find((GameMessage m) => m.Action == GameEvent.CREATURE_ATTACKED);
					if (attackMessage2 == null)
					{
						continue;
					}
					CreatureAttacker attacker = new CreatureAttacker
					{
						Creature = attackMessage2.Creature
					};
					attackMessage2.Presented = true;
					CreatureAttackTarget target = new CreatureAttackTarget
					{
						Creature = attackMessage2.SecondCreature
					};
					attacker.Targets.Add(target);
					GameMessage damageMessage = GetDamageMessage(attackMessage2);
					if (damageMessage != null)
					{
						damageMessage.Presented = true;
					}
					bool killingAttack = groupedMessages4.Find((GameMessage m) => m.Action == GameEvent.CREATURE_DIED && m.Creature == target.Creature && !m.AutoReviveDeath) != null;
					CreatureHit hit = new CreatureHit(attackMessage2, damageMessage, killingAttack);
					target.Hits.Add(hit);
					bool firstAttack = currentAttackTarget != target.Creature;
					currentAttackTarget = target.Creature;
					if (moreAttacksToCome)
					{
						StartCoroutine(CreatureAttack(attacker, useCritCam, firstAttack, !moreHitsToComeInThisAttack, isAiDragAttack));
						yield return new WaitForSeconds(0.6f);
					}
					else
					{
						yield return StartCoroutine(CreatureAttack(attacker, useCritCam, firstAttack, !moreHitsToComeInThisAttack, isAiDragAttack));
					}
				}
			}
			else if (message.Action == GameEvent.CREATURE_DIED)
			{
				if (!message.AutoReviveDeath)
				{
					while (mCreatureAttacksInProgress > 0)
					{
						yield return null;
					}
					anyCreaturesDied = true;
					GameMessage attackMessage = mQueuedMessages.Find((GameMessage m) => m.Action == GameEvent.CREATURE_ATTACKED);
					if (attackMessage == null)
					{
						Singleton<BattleHudController>.Instance.PlayWhiteScreenFlashTween();
					}
					GameMessage passiveTriggered = mQueuedMessages.Find((GameMessage m) => m.Action == GameEvent.PASSIVE_TRIGGERED && m.Creature == message.Creature);
					if (passiveTriggered != null)
					{
						if (passiveTriggered.Presented)
						{
							passiveTriggered = null;
						}
						else
						{
							passiveTriggered.Presented = true;
						}
					}
					OnCreatureDied(message.Creature, passiveTriggered);
					yield return new WaitForSeconds(0.5f);
					if (message.WhichPlayer.Type == PlayerType.User)
					{
						Singleton<SLOTAudioManager>.Instance.TriggerVOEvent(Singleton<DWGame>.Instance.GetCharacter(PlayerType.User), VOEvent.LoseCreature);
					}
					else
					{
						Singleton<SLOTAudioManager>.Instance.TriggerVOEvent(Singleton<DWGame>.Instance.GetCharacter(PlayerType.User), VOEvent.DestroyCreature);
					}
				}
				else
				{
					DWBattleLaneObject laneObj2 = Singleton<DWBattleLane>.Instance.GetLaneObject(message.Creature);
					yield return StartCoroutine(Singleton<DWBattleLane>.Instance.PlayDerez(laneObj2, message.Creature, true, true));
				}
				if (hitReactType < HitReactType.Big)
				{
					hitReactType = HitReactType.Big;
					playHitReactOnPlayer3 = message.Creature.Owner.Type;
				}
			}
			else if (message.Action == GameEvent.DAMAGE_CREATURE)
			{
				if (message.Amount != 0f)
				{
					ShowCreatureDamage(isFatal: mQueuedMessages.Count > i + 1 && mQueuedMessages[i + 1].Action == GameEvent.CREATURE_DIED && !mQueuedMessages[i + 1].AutoReviveDeath && mQueuedMessages[i + 1].Creature == message.Creature, type: (message.MagicDamage && message.PhysicalDamage) ? DamageType.Hybrid : ((!message.PhysicalDamage) ? DamageType.Magic : DamageType.Physical), creature: message.Creature, rawAmount: (int)message.RawAmount, cappedAmount: (int)message.Amount, isCrit: message.IsCritical, isMissOrShield: false);
					if (message.Parent != null && message.Parent.Action == GameEvent.START_TURN)
					{
						mEffectFinishTimer = 1f;
						yield return new WaitForSeconds(0.11f);
					}
					if (hitReactType < HitReactType.Small)
					{
						hitReactType = HitReactType.Small;
						playHitReactOnPlayer3 = message.Creature.Owner.Type;
					}
				}
			}
			else if (message.Action == GameEvent.HEAL_CREATURE)
			{
				ShowCreatureHealing(message.Creature, (int)message.RawAmount, (int)message.Amount);
			}
			else if (message.Action == GameEvent.SET_CREATURE_HP)
			{
				CreatureHPBar hpBar = Singleton<BattleHudController>.Instance.GetHPBar(message.Creature);
				if (hpBar != null)
				{
					hpBar.SetHealth((int)message.Amount);
				}
			}
			else if (message.Action == GameEvent.DRAW_CARD || message.Action == GameEvent.INITIAL_DRAW_CARD)
			{
				List<GameMessage> groupedMessages2 = mQueuedMessages.FindAll((GameMessage m) => !m.Presented && m.Parent == message.Parent && (m.Action == GameEvent.DRAW_CARD || m.Action == GameEvent.INITIAL_DRAW_CARD));
				List<CardData> drawnCards = new List<CardData>();
				foreach (GameMessage groupedMessage2 in groupedMessages2)
				{
					if (groupedMessage2.Action == GameEvent.DRAW_CARD)
					{
						GameMessage failedDrawMessage = mQueuedMessages.Find((GameMessage m) => !m.Presented && m.Parent == message.Parent && m.Action == GameEvent.FAILED_DRAW_HAND_FULL && m.WhichPlayer == groupedMessage2.WhichPlayer && m.Card == groupedMessage2.Card);
						if (failedDrawMessage != null)
						{
							Singleton<HandCardController>.Instance.ShowFailedCardDraw(failedDrawMessage.WhichPlayer.Type, failedDrawMessage.Card, groupedMessage2.Creature);
							failedDrawMessage.Presented = true;
						}
						else
						{
							if (groupedMessage2.Creature == null)
							{
								HeroPortraitScript portrait = Singleton<BattleHudController>.Instance.HeroPortraits[message.WhichPlayer.Type.IntValue];
								yield return StartCoroutine(portrait.OnPowerUpMeterFull());
								yield return StartCoroutine(PlayHeroCardDrawAnim(groupedMessage2.WhichPlayer.Type));
								Singleton<HandCardController>.Instance.ShowCardDraw(groupedMessage2.WhichPlayer.Type, groupedMessage2.Card);
							}
							else
							{
								Singleton<HandCardController>.Instance.ShowCardDraw(groupedMessage2.WhichPlayer.Type, groupedMessage2.Card, groupedMessage2.Creature);
							}
							drawnCards.Add(groupedMessage2.Card);
						}
					}
					else if (groupedMessage2.Action == GameEvent.INITIAL_DRAW_CARD)
					{
						CreatureItem creature = ((groupedMessage2.Creature == null) ? null : groupedMessage2.Creature.Data);
						Singleton<HandCardController>.Instance.ShowCardDraw(groupedMessage2.WhichPlayer.Type, groupedMessage2.Card, creature);
					}
					groupedMessage2.Presented = true;
				}
				yield return null;
				while (Singleton<HandCardController>.Instance.CardEventsInProgress())
				{
					yield return null;
				}
				yield return new WaitForSeconds(0.5f);
				if (message.WhichPlayer.Type == PlayerType.User)
				{
					foreach (CardData card in drawnCards)
					{
						DetachedSingleton<ConditionalTutorialController>.Instance.OnCardDrawn(card);
					}
				}
			}
			else if (message.Action == GameEvent.FORCE_DISCARD_CARD)
			{
				List<GameMessage> groupedMessages3 = mQueuedMessages.FindAll((GameMessage m) => !m.Presented && m.Action == message.Action);
				foreach (GameMessage groupedMessage3 in groupedMessages3)
				{
					Singleton<HandCardController>.Instance.ShowCardDiscard(groupedMessage3.WhichPlayer.Type, groupedMessage3.Card);
					groupedMessage3.Presented = true;
				}
				while (Singleton<HandCardController>.Instance.CardEventsInProgress())
				{
					yield return null;
				}
			}
			else if (message.Action == GameEvent.MANUAL_DISCARD_CARD)
			{
				if (message.WhichPlayer.Type == PlayerType.Opponent)
				{
					Singleton<HandCardController>.Instance.ShowCardDiscard(PlayerType.Opponent, message.Card);
					while (Singleton<HandCardController>.Instance.CardEventsInProgress())
					{
						yield return null;
					}
					yield return new WaitForSeconds(1f);
				}
			}
			else if (message.Action == GameEvent.ENABLE_STEALTH)
			{
				if (Singleton<BattleHudController>.Instance.TargetedCreature == message.Creature)
				{
					Singleton<BattleHudController>.Instance.ClearTargetReticle();
				}
			}
			else if (message.Action == GameEvent.ENABLE_BRAVERY)
			{
				CreatureState currentTarget = Singleton<BattleHudController>.Instance.TargetedCreature;
				if (currentTarget != null && currentTarget != message.Creature && !currentTarget.HasBravery)
				{
					Singleton<BattleHudController>.Instance.ClearTargetReticle();
				}
			}
			else if (message.Action == GameEvent.ADVANCE_APMETER)
			{
				HeroPortraitScript portrait2 = Singleton<BattleHudController>.Instance.HeroPortraits[message.WhichPlayer.Type.IntValue];
				portrait2.FillPowerUpMeter(message.Amount);
			}
			else if (message.Action == GameEvent.ENABLE_LANDCORN || message.Action == GameEvent.ENABLE_LANDNICE || message.Action == GameEvent.ENABLE_LANDPLAINS || message.Action == GameEvent.ENABLE_LANDSAND || message.Action == GameEvent.ENABLE_LANDSWAMP)
			{
				if (message.Creature.Owner.Type == PlayerType.User)
				{
					DetachedSingleton<ConditionalTutorialController>.Instance.OnGainedLandscape(message.Creature, message.Status);
				}
			}
			else if (message.Action == GameEvent.GAIN_BUFF || message.Action == GameEvent.GAIN_DEBUFF)
			{
				if (message.Creature.Owner.Type == PlayerType.User)
				{
					DetachedSingleton<ConditionalTutorialController>.Instance.OnGainedStatusEffect(message.Creature, message.Status);
				}
			}
			else if (message.Action == GameEvent.APMETER_FULL)
			{
				if (message.WhichPlayer.Type == PlayerType.User)
				{
					DetachedSingleton<ConditionalTutorialController>.Instance.OnAPMeterFull();
				}
			}
			else if (message.Action == GameEvent.PASSIVE_TRIGGERED)
			{
				DWBattleLaneObject laneObj = Singleton<DWBattleLane>.Instance.GetLaneObject(message.Creature);
				if (laneObj != null)
				{
					PlayPassiveTriggerEffect(laneObj.CreatureObject.transform.position, laneObj, message);
					yield return new WaitForSeconds(1.2f);
				}
			}
			else if (message.Action == GameEvent.PUSH_CREATURE_TO_HAND)
			{
				Vector3 pos = Singleton<DWBattleLane>.Instance.RemoveCreatureObj(message.Creature, null, false);
				Singleton<HandCardController>.Instance.ShowCreatureCardDraw(message.Creature.Owner.Type, message.Creature.Data, pos);
				anyCreaturesPushed = true;
			}
			GameEventFXData eventFX = GameEventFXDataManager.Instance.GetData(message.Action.ToString());
			if (eventFX != null)
			{
				if (eventFX.CanGroup)
				{
					List<GameMessage> groupedMessages = mQueuedMessages.FindAll((GameMessage m) => !m.Presented && m.Action == message.Action && m.Parent == message.Parent && m.Creature.Owner == message.Creature.Owner && m.CausedByPassive == message.CausedByPassive);
					bool centeredFX = eventFX.ActionFXSpawnPoint == GameEventFXSpawnPoint.BoardCenter || eventFX.ActionFXSpawnPoint == GameEventFXSpawnPoint.BoardSide;
					if (groupedMessages.Count > 0)
					{
						for (int ii = 0; ii < groupedMessages.Count; ii++)
						{
							GameMessage groupedMessage = groupedMessages[ii];
							groupedMessage.Presented = true;
							if (ii == 0 || !centeredFX)
							{
								yield return StartCoroutine(PerformEventFX(groupedMessage, eventFX));
								float waitTime = ((ii >= groupedMessages.Count - 1 || centeredFX) ? eventFX.ActionFXWaitTime : eventFX.ActionFXGroupedWaitTime);
								if (waitTime > 0f)
								{
									yield return new WaitForSeconds(waitTime);
								}
							}
						}
					}
					else
					{
						yield return StartCoroutine(PerformEventFX(message, eventFX));
						if (eventFX.ActionFXWaitTime > 0f)
						{
							yield return new WaitForSeconds(eventFX.ActionFXWaitTime);
						}
					}
				}
				else
				{
					yield return StartCoroutine(PerformEventFX(message, eventFX));
					if (eventFX.ActionFXWaitTime > 0f)
					{
						yield return new WaitForSeconds(eventFX.ActionFXWaitTime);
					}
				}
			}
			else
			{
				yield return StartCoroutine(PerformEventFX(message, null));
			}
			message.Presented = true;
			if (!Singleton<DWGame>.Instance.InDeploymentPhase())
			{
				int playerCreatureCount = Singleton<DWBattleLane>.Instance.BattleLaneObjects[0].Count;
				int opponentCreatureCount = Singleton<DWBattleLane>.Instance.BattleLaneObjects[1].Count;
				if ((playerCreatureCount == 0 && Singleton<PlayerInfoScript>.Instance.StateData.MultiplayerMode) || opponentCreatureCount == 0)
				{
					gameEnded = true;
					Singleton<BattleHudController>.Instance.EnemyThinkingObject.SetActive(false);
					break;
				}
			}
		}
		mQueuedMessages.Clear();
		if (mDragDrawCard != null)
		{
			NGUITools.Destroy(mDragDrawCard.gameObject);
			mDragDrawCard = null;
		}
		while (mEffectFinishTimer > 0f)
		{
			yield return null;
		}
		Singleton<DWBattleLane>.Instance.UpdateHPBarAttackValues();
		UICamera.UnlockInput();
		if (dragAttackHappened)
		{
			Singleton<TutorialController>.Instance.AdvanceIfOnAttack();
		}
		if (cardAttackHappened)
		{
			Singleton<TutorialController>.Instance.AdvanceIfTargetingAttack();
		}
		if (cardPlayed)
		{
			Singleton<TutorialController>.Instance.AdvanceIfOnCardPlay(false);
			if (Singleton<DWGame>.Instance.InDeploymentPhase() && !Singleton<DWGame>.Instance.IsTutorialSetup)
			{
				if (!Singleton<DWGame>.Instance.CurrentBoardState.IsFirstTurn())
				{
					UICamera.LockInput();
					yield return new WaitForSeconds(0.8f);
					UICamera.UnlockInput();
				}
				if (currentState.IsP1Turn())
				{
					Singleton<BattleHudController>.Instance.ClearPvpTimer(false);
					Singleton<DWGame>.Instance.EndPlayerTurn();
				}
			}
		}
		if (Singleton<DWBattleLane>.Instance.LootObjectsToCollect())
		{
			DetachedSingleton<ConditionalTutorialController>.Instance.OnLootDropped();
			Singleton<SLOTAudioManager>.Instance.TriggerVOEvent(Singleton<DWGame>.Instance.GetCharacter(PlayerType.User), VOEvent.LootDrop);
			yield return new WaitForSeconds(0.6f);
			bool inputLocked = UICamera.IsInputLocked() && currentState.IsP2Turn();
			if (inputLocked)
			{
				UICamera.UnlockInput();
			}
			Singleton<HandCardController>.Instance.HideHand();
			Singleton<BattleHudController>.Instance.EnemyThinkingObject.SetActive(false);
			Singleton<BattleHudController>.Instance.ShowTapToCollectBanner();
			Singleton<DWBattleLane>.Instance.TriggerBattleLootCollect();
			bool pauseMenuInputLocked = Singleton<PauseController>.Instance.IsInputBlocked();
			Singleton<PauseController>.Instance.BlockInput(false);
			while (Singleton<DWBattleLane>.Instance.LootObjectsToCollect())
			{
				yield return null;
			}
			Singleton<PauseController>.Instance.BlockInput(pauseMenuInputLocked);
			Singleton<DWBattleLane>.Instance.RepositionLaneObjects();
			Singleton<DWGameCamera>.Instance.MoveCameraToP1Setup();
			Singleton<HandCardController>.Instance.ShowHand();
			if (inputLocked)
			{
				UICamera.LockInput();
			}
		}
		else if (anyCreaturesDied)
		{
			UICamera.LockInput();
			while (Singleton<DWBattleLane>.Instance.CurrencyObjectsToCollect())
			{
				yield return null;
			}
			UICamera.UnlockInput();
			Singleton<DWBattleLane>.Instance.RepositionLaneObjects();
			Singleton<DWGameCamera>.Instance.MoveCameraToP1Setup();
		}
		else if (anyCreaturesPushed)
		{
			Singleton<DWBattleLane>.Instance.RepositionLaneObjects();
		}
		if (turnStarted)
		{
			if (currentState.IsP1Turn())
			{
				DetachedSingleton<ConditionalTutorialController>.Instance.OnTurnStart();
			}
			else if (!gameEnded)
			{
				if (!Singleton<DWGame>.Instance.InDeploymentPhase())
				{
					Singleton<DWGameCamera>.Instance.MoveCameraToP2Setup();
					if (!Singleton<TutorialController>.Instance.IsBlockActive("IntroBattle") && !Singleton<PlayerInfoScript>.Instance.StateData.MultiplayerMode)
					{
						Singleton<BattleHudController>.Instance.EnemyThinkingTween.Play();
						float bubbleOffset = Singleton<DWGame>.Instance.GetCharacter(PlayerType.Opponent).ThoughtBubbleHeightAdjust;
						Singleton<BattleHudController>.Instance.ThoughtBubbleParent.localPosition = new Vector3(0f, bubbleOffset, 0f);
					}
					Singleton<DWBattleLane>.Instance.StartP2ThinkingDelay();
				}
				else
				{
					Singleton<DWBattleLane>.Instance.SkipP2ThinkingDelay();
				}
			}
		}
		Singleton<TutorialController>.Instance.AdvanceIfOnState("Q1_PickUpDrop");
		Singleton<TutorialController>.Instance.AdvanceIfOnState("Q1_BattlePlayCard");
		if (!Singleton<DWGame>.Instance.InDeploymentPhase() && Singleton<DWGame>.Instance.GetCreatureCount(PlayerType.Opponent) <= 0)
		{
			if (!IsTransitioningBetweenWaves())
			{
				if (Singleton<TutorialController>.Instance.IsBlockActive("IntroBattle"))
				{
					Singleton<BattleHudController>.Instance.OnGameEnd();
					Singleton<HandCardController>.Instance.HideHand();
					Singleton<TutorialController>.Instance.AdvanceTutorialState();
					Singleton<DWGameCamera>.Instance.MoveCameraToP2Setup();
					Singleton<DWGame>.Instance.SetGameState(GameState.EndGameWait);
				}
				else
				{
					Singleton<DWGame>.Instance.SetGameState(GameState.EndGameWait);
					yield return new WaitForSeconds(0.7f);
					Singleton<DWGame>.Instance.SetGameState(GameState.P2Defeated);
                    if (Singleton<PlayerInfoScript>.Instance.StateData.MultiplayerMode)
                    {
                        Singleton<DWGame>.Instance.turnNumber = 0;
                        Singleton<DWGame>.Instance.battleDuration = 0f;
                    }
                }
			}
		}
		else if (!Singleton<DWGame>.Instance.InDeploymentPhase() && !Singleton<DWGame>.Instance.WaitingForCreatureDeployAfterRevive && Singleton<DWGame>.Instance.GetCreatureCount(PlayerType.User) <= 0)
		{
			if (!IsTransitioningBetweenWaves() && !Singleton<DWGame>.Instance.IsGameOver())
			{
				Singleton<DWGame>.Instance.LostDuringMyTurn = currentState.IsP1Turn();
				Singleton<DWGame>.Instance.SetGameState(GameState.EndGameWait);
				yield return new WaitForSeconds(0.7f);
				Singleton<DWGame>.Instance.SetGameState(GameState.P1Defeated);
                if (Singleton<PlayerInfoScript>.Instance.StateData.MultiplayerMode)
                {
                    Singleton<DWGame>.Instance.turnNumber = 0;
                    Singleton<DWGame>.Instance.battleDuration = 0f;
                }
            }
		}
		else if (currentState.IsP1Turn())
		{
			Singleton<DWBattleLane>.Instance.UpdateAvailableActionIndicators();
		}
		mProcessingMessages = false;
	}

	public void PlayPassiveTriggerEffect(Vector3 position, DWBattleLaneObject attachToLane, GameMessage message)
	{
		GameObject value = null;
		Singleton<DWBattleLane>.Instance.CreatureVFXPool.TryGetValue(message.Creature.Data.Form.RezInVFX, out value);
		if (value == null)
		{
			value = (GameObject)Singleton<SLOTResourceManager>.Instance.LoadResource("VFX/Creatures/" + message.Creature.Data.Form.RezInVFX);
		}
		if (value != null)
		{
			Transform transform = base.transform.InstantiateAsChild(value).transform;
			transform.position = position;
		}
		Singleton<SLOTAudioManager>.Instance.PlaySound("battle/SFX_Action_TriggerEnchant");
		string str = message.Creature.Data.PassiveNameString();
		Vector3 position2 = ((!(attachToLane != null)) ? position : attachToLane.CreatureObject.transform.position);
		ShowEffectLabelPopup(position2, attachToLane, str, PassiveTextColor, Color.black, 25);
	}

	private IEnumerator PerformEventFX(GameMessage ms, GameEventFXData eventFX)
	{
		PlayerType player = ((ms.Creature == null) ? ms.WhichPlayer.Type : ms.Creature.Owner.Type);
		if (ms.Creature != null)
		{
			CreatureBuffBar buffBar = Singleton<BattleHudController>.Instance.GetBuffBar(ms.Creature);
			if (buffBar != null)
			{
				yield return StartCoroutine(buffBar.SetPersistentVFX(ms));
			}
		}
		if (!PrintDebugActionName || ms.Action.ToString().StartsWith("ENABLE"))
		{
		}
		if (!PrintDebugActionName || ms.Action.ToString().StartsWith("STATUS"))
		{
		}
		if (ms.Action.ToString().StartsWith("STATUS_ACTION"))
		{
			ShowStatusEffectLabelPopup(ms);
		}
		DisableCreaturePersistentFX(ms);
		if (eventFX == null)
		{
			yield break;
		}
		EnableCreaturePersistentFX(ms);
		Transform spawnPoint = GetGameEventEffectTarget(eventFX.ActionFXSpawnPoint, player, ms.Creature, ms.SecondCreature);
		GameObject fxObj = null;
		if (!(spawnPoint != null))
		{
			yield break;
		}
		GameObject eventFXPrefab = Resources.Load("VFX/Actions/" + eventFX.ActionFXPrefab, typeof(GameObject)) as GameObject;
		if (eventFXPrefab != null)
		{
			if (eventFX.RezInOut != "In")
			{
				fxObj = spawnPoint.InstantiateAsChild(eventFXPrefab as GameObject);
				if (eventFX.ActionFXSpawnPoint == GameEventFXSpawnPoint.EnemyPortrait)
				{
					fxObj.ChangeLayerToParent();
				}
				else if ((eventFX.ActionFXSpawnPoint == GameEventFXSpawnPoint.Creature || eventFX.ActionFXSpawnPoint == GameEventFXSpawnPoint.Creature2) && player == PlayerType.User)
				{
					fxObj.transform.eulerAngles = new Vector3(0f, 90f, 0f);
				}
			}
			UIWidget spawnTargetWidget = spawnPoint.GetComponent<UIWidget>();
			if (spawnTargetWidget != null && fxObj != null)
			{
				VFXRenderQueueSorter sorter = fxObj.GetComponent<VFXRenderQueueSorter>();
				if (sorter == null)
				{
					sorter = fxObj.AddComponent<VFXRenderQueueSorter>();
				}
				sorter.mTarget = spawnTargetWidget;
			}
			if (eventFX.AttractPoint != 0)
			{
				ParticleAttractor[] attractors = fxObj.GetComponentsInChildren<ParticleAttractor>();
				ParticleAttractor[] array = attractors;
				foreach (ParticleAttractor attractor in array)
				{
					if (eventFX.AttractPoint == GameEventFXData.ParticleAttractorPoint.MyPortrait)
					{
						attractor.SetTarget(Singleton<BattleHudController>.Instance.HeroPortraits[(int)ms.Creature.Owner.Type].transform);
					}
					else if (eventFX.AttractPoint == GameEventFXData.ParticleAttractorPoint.Creature)
					{
						DWBattleLaneObject lane = Singleton<DWBattleLane>.Instance.GetLaneObject(ms.Creature);
						attractor.SetTarget(lane.transform);
					}
				}
			}
		}
		ShowEffectValuePopup(ms, eventFX.Keyword);
	}

	private bool ExistsInDisableEvents(GameMessage ms)
	{
		List<StatusData> database = StatusDataManager.Instance.GetDatabase();
		foreach (StatusData item in database)
		{
			if (item.DisableMessage == ms.Action)
			{
				return true;
			}
		}
		return false;
	}

	private StatusData GetStatusData(GameMessage ms)
	{
		List<StatusData> database = StatusDataManager.Instance.GetDatabase();
		return database.Find((StatusData match) => match.EnableMessage == ms.Action);
	}

	private IEnumerator ShowCardPlayEffect(GameMessage cardPlayedMessage, List<GameMessage> messages)
	{
		if (cardPlayedMessage.Card.TargetType1 != SelectionType.Lane)
		{
			NoneLaneTriggerEffect(cardPlayedMessage);
		}
		if (cardPlayedMessage.Card.DirectDamageFX != string.Empty)
		{
			Transform spawnPoint = GetGameEventEffectTarget(player: (cardPlayedMessage.Creature == null) ? cardPlayedMessage.WhichPlayer.Type : cardPlayedMessage.Creature.Owner.Type, spawnPointType: cardPlayedMessage.Card.FXSpawnPoint, creature: cardPlayedMessage.Creature);
			if (spawnPoint != null)
			{
				GameObject eventFXPrefab = Resources.Load("VFX/Actions/" + cardPlayedMessage.Card.DirectDamageFX, typeof(GameObject)) as GameObject;
				if (eventFXPrefab != null)
				{
					spawnPoint.transform.InstantiateAsChild(eventFXPrefab as GameObject);
					yield return new WaitForSeconds(0.8f);
				}
			}
		}
		yield return new WaitForSeconds(0.3f);
	}

	private void NoneLaneTriggerEffect(GameMessage ms)
	{
		GameObject gameObject = Singleton<BattleHudController>.Instance.transform.InstantiateAsChild(NonLaneActionStartFXPrefab);
		UILabel componentInChildren = gameObject.GetComponentInChildren<UILabel>();
		if (componentInChildren != null)
		{
			componentInChildren.text = ms.Card.Name + "!";
		}
	}

	private void DisableCreaturePersistentFX(GameMessage ms)
	{
		DWBattleLaneObject laneObject = Singleton<DWBattleLane>.Instance.GetLaneObject(ms.Creature);
		if (!(laneObject == null))
		{
			if (ms.Action == GameEvent.DISABLE_FROZEN)
			{
				laneObject.UnfreezeCreature();
			}
			if (ms.Action == GameEvent.DISABLE_STEALTH)
			{
				laneObject.UnStealthCreature();
			}
			if (ms.Action == GameEvent.DISABLE_TRANSMOGRIFY)
			{
				laneObject.UntransmogrifyCreature();
			}
			laneObject.RemoveStatusEffectObjFromCreature(ms);
		}
	}

	private void EnableCreaturePersistentFX(GameMessage ms)
	{
		DWBattleLaneObject laneObject = Singleton<DWBattleLane>.Instance.GetLaneObject(ms.Creature);
		if (!(laneObject == null))
		{
			if (ms.Action == GameEvent.ENABLE_FROZEN)
			{
				laneObject.FreezeCreature();
			}
			if (ms.Action == GameEvent.ENABLE_STEALTH)
			{
				laneObject.StealthCreature();
			}
			if (ms.Action == GameEvent.ENABLE_TRANSMOGRIFY)
			{
				laneObject.TransmogrifyCreature();
			}
			laneObject.ApplyStatusEffectObjOnCreature(ms);
		}
	}

	private void OnCreatureDied(CreatureState creature, GameMessage passiveTriggered)
	{
		if (creature.Owner.Type == PlayerType.User)
		{
			Singleton<DWBattleLane>.Instance.CurrentBattleTomestoneCreature.Add(creature);
			DetachedSingleton<ConditionalTutorialController>.Instance.OnCreatureDied();
			Singleton<HandCardController>.Instance.ShowCreatureHand();
		}
		Singleton<BattleHudController>.Instance.OnCreatureDied(creature);
		Singleton<DWBattleLane>.Instance.RemoveCreatureObj(creature, passiveTriggered);
	}

	private Transform GetGameEventEffectTarget(GameEventFXSpawnPoint spawnPointType, PlayerType player = null, CreatureState creature = null, CreatureState secondCreature = null)
	{
		switch (spawnPointType)
		{
		case GameEventFXSpawnPoint.UI_Deck:
			return Singleton<BattleHudController>.Instance.UIEffectTargetDeck.transform;
		case GameEventFXSpawnPoint.UI_HeroPortrait:
			return Singleton<BattleHudController>.Instance.UIEffectTargets[player.IntValue].transform;
		case GameEventFXSpawnPoint.UI_DiscardPile:
			return Singleton<BattleHudController>.Instance.UIEffectTargetDiscardPile.transform;
		case GameEventFXSpawnPoint.UI_MagicPoint:
			return Singleton<BattleHudController>.Instance.MagicPoints[player.IntValue].transform;
		case GameEventFXSpawnPoint.Creature:
		{
			GameObject creatureObject2 = Singleton<DWBattleLane>.Instance.GetCreatureObject(creature);
			if (creatureObject2 == null)
			{
				return null;
			}
			return creatureObject2.transform;
		}
		case GameEventFXSpawnPoint.Creature2:
		{
			GameObject creatureObject = Singleton<DWBattleLane>.Instance.GetCreatureObject(secondCreature);
			if (creatureObject == null)
			{
				return null;
			}
			return creatureObject.transform;
		}
		case GameEventFXSpawnPoint.BoardSide:
			if (player == PlayerType.User)
			{
				return PlayerLaneCenterPos;
			}
			return OpponentLaneCenterPos;
		case GameEventFXSpawnPoint.BoardCenter:
			return BoardCenterPos;
		case GameEventFXSpawnPoint.EnemyPortrait:
			return Singleton<BattleHudController>.Instance.HeroPortraits[(int)(!creature.Owner.Type)].transform;
		default:
			return null;
		}
	}

	public void PlayActionTriggerAnim(CreatureState creature)
	{
		GameObject creatureObject = Singleton<DWBattleLane>.Instance.GetCreatureObject(creature);
		DWBattleLaneObject laneObject = Singleton<DWBattleLane>.Instance.GetLaneObject(creature);
		if (!laneObject.IsFrozen)
		{
			creatureObject.GetComponent<Animation>().Play("Floop");
		}
	}

	public void PlayNegativeActionTriggerAnim(CreatureState creature)
	{
		GameObject creatureObject = Singleton<DWBattleLane>.Instance.GetCreatureObject(creature);
		DWBattleLaneObject laneObject = Singleton<DWBattleLane>.Instance.GetLaneObject(creature);
		if (!laneObject.IsFrozen)
		{
			creatureObject.GetComponent<Animation>().Play("Hammer");
		}
	}

	public Transform FindInChildren(Transform tr, string childName)
	{
		Transform[] componentsInChildren = tr.gameObject.GetComponentsInChildren<Transform>();
		Transform[] array = componentsInChildren;
		foreach (Transform transform in array)
		{
			if (transform.name.Contains(childName))
			{
				return transform;
			}
		}
		return null;
	}

	public void FlashDamagedCreature(CreatureState creature)
	{
		GameObject creatureObject = Singleton<DWBattleLane>.Instance.GetCreatureObject(creature);
		BattleCreatureAnimState componentInChildren = creatureObject.GetComponentInChildren<BattleCreatureAnimState>();
		DWBattleLaneObject laneObject = Singleton<DWBattleLane>.Instance.GetLaneObject(creature);
		if (!laneObject.IsFrozen && !(componentInChildren == null))
		{
		}
	}

	private IEnumerator AssignOriginalShaderBack(BattleCreatureAnimState animState)
	{
		yield return new WaitForSeconds(HitFlashMatDuration);
		for (int i = 0; i < animState.orignalMeshes.Count; i++)
		{
			animState.orignalMeshes[i].material = animState.originalMats[i];
		}
	}

	public void PlayCreatureAnim(CreatureState creature, string trigger)
	{
		DWBattleLaneObject laneObject = Singleton<DWBattleLane>.Instance.GetLaneObject(creature);
		if (!laneObject.IsFrozen)
		{
			GameObject creatureObject = Singleton<DWBattleLane>.Instance.GetCreatureObject(creature);
			Animator[] componentsInChildren = creatureObject.GetComponentsInChildren<Animator>(true);
			componentsInChildren[0].SetTrigger(trigger);
		}
	}

	public Vector3 GetAttackPosition(CreatureState creature)
	{
		GameObject creatureObject = Singleton<DWBattleLane>.Instance.GetCreatureObject(creature);
		Vector3 position = creatureObject.transform.position;
		position.y += 2f;
		return position;
	}

	private Vector3 GetMeleeAttackPosition(CreatureState attacker, CreatureState target)
	{
		Vector3 position = Singleton<DWBattleLane>.Instance.GetCreatureObject(attacker).transform.position;
		Vector3 position2 = Singleton<DWBattleLane>.Instance.GetCreatureObject(target).transform.position;
		return position2 - (position2 - position).normalized * 5f;
	}

	private void ShowEffectValuePopup(GameMessage ms, KeyWordData keyword)
	{
		StatusData statusData = GetStatusData(ms);
		if (statusData != null && keyword != null)
		{
			Color outlineColor = ((ms.Status.StatusType != StatusType.Buff) ? StatusTextDebuffColor : StatusTextBuffColor);
			string str = "-" + keyword.DisplayName + "-\n" + keyword.PopupText;
			DWBattleLaneObject laneObject = Singleton<DWBattleLane>.Instance.GetLaneObject(ms.Creature);
			ShowEffectLabelPopup(laneObject.CreatureObject.transform.position, laneObject, str, Color.white, outlineColor);
		}
	}

	private void ShowStatusEffectLabelPopup(GameMessage ms)
	{
		if (ms.Status != null && ms.Status.FXData.Keyword != null)
		{
			Color outlineColor = ((ms.Status.StatusType != StatusType.Buff) ? StatusTextDebuffColor : StatusTextBuffColor);
			DWBattleLaneObject laneObject = Singleton<DWBattleLane>.Instance.GetLaneObject(ms.Creature);
			if (laneObject != null)
			{
				ShowEffectLabelPopup(laneObject.CreatureObject.transform.position, laneObject, ms.Status.FXData.Keyword.DisplayName, Color.white, outlineColor);
			}
		}
	}

	private void ShowEffectLabelPopup(Vector3 position, DWBattleLaneObject attachToLane, string str, Color color, Color outlineColor, int fontSize = 20)
	{
		GameObject gameObject = Singleton<BattleHudController>.Instance.transform.InstantiateAsChild(EffectNameDisplayPrefab);
		ShowEffectDisplayPopup component = gameObject.GetComponent<ShowEffectDisplayPopup>();
		component.Init(attachToLane, str, position, color, outlineColor, fontSize);
	}

	private IEnumerator CreatureAttack(CreatureAttacker attacker, bool useCritCamera, bool firstAttack, bool lastAttack, bool isAiDragAttack)
	{
		mCreatureAttacksInProgress++;
		PlayerType activePlayer = ((!Singleton<DWGame>.Instance.GetCurrentGameState().IsP1Turn()) ? PlayerType.Opponent : PlayerType.User);
		DWBattleLaneObject attackerLane = Singleton<DWBattleLane>.Instance.GetLaneObject(attacker.Creature);
		GameObject attackerObj = Singleton<DWBattleLane>.Instance.GetCreatureObject(attacker.Creature);
		Animator animator = attackerObj.GetComponentInChildren<Animator>();
		Vector3 targetPos = GetMeleeAttackPosition(attacker.Creature, attacker.Targets[0].Creature);
		CreatureData creatureData = attacker.Creature.Data.Form;
		bool isMagicAttack = creatureData.AlwaysMagicVFX || attacker.Targets[0].Hits[0].HitType == DamageType.Magic || attacker.Targets[0].Hits[0].HitType == DamageType.Hybrid;
		if (activePlayer == PlayerType.Opponent)
		{
			attackerLane.HealthBar.SetAttackValues(attacker.Targets[0].Hits[0].HitType);
		}
		if (firstAttack)
		{
			attackerLane.AttackStartPosition = attackerObj.transform.position;
			if (isAiDragAttack)
			{
				GameObject targetCreatureObj = Singleton<DWBattleLane>.Instance.GetCreatureObject(attacker.Targets[0].Creature);
				yield return StartCoroutine(Singleton<DWBattleLane>.Instance.ShowAiAttackArrow(attackerObj, targetCreatureObj));
			}
			if (useCritCamera)
			{
				float time = 0.3f;
				iTween.EaseType ease = iTween.EaseType.easeInOutCubic;
				Vector3 lookAtPos = (attackerLane.AttackStartPosition + targetPos) / 2f;
				Vector3 sourcePos = ((activePlayer != PlayerType.User) ? targetPos : attackerLane.AttackStartPosition);
				lookAtPos.y += CritCamLookAtYOffset;
				iTween.MoveTo(Singleton<DWGameCamera>.Instance.MainCamLookAt.gameObject, iTween.Hash("position", lookAtPos, "time", time, "easetype", ease));
				Vector3 camOffset = new Vector3(0f, CritCamYOffset + attacker.Creature.Data.Form.Height, CritCamZOffset);
				if (lookAtPos.z < sourcePos.z)
				{
					camOffset.z *= -1f;
				}
				Vector3 delta = lookAtPos - sourcePos;
				Vector3 camPos = camOffset + sourcePos - delta.normalized * 15f;
				iTween.MoveTo(Singleton<DWGameCamera>.Instance.MainCam.gameObject, iTween.Hash("position", camPos, "time", time, "easetype", ease));
				yield return new WaitForSeconds(time);
				Time.timeScale = 0.75f;
			}
			iTween.RotateTo(attackerObj, iTween.Hash("rotation", Quaternion.LookRotation(targetPos - attackerLane.AttackStartPosition).eulerAngles, "time", RotateToAttackTime, "easetype", iTween.EaseType.easeInOutCubic));
			if (!isMagicAttack)
			{
				animator.SetTrigger("Lunge");
				if (useCritCamera)
				{
					Singleton<DWGameCamera>.Instance.SetObjectLock(attackerObj.transform);
				}
				iTween.MoveTo(attackerObj, iTween.Hash("position", targetPos, "time", creatureData.AttackMovementTime, "easetype", creatureData.AttackMovementStyle));
				if (creatureData.LungeSound != string.Empty)
				{
					Singleton<SLOTAudioManager>.Instance.PlaySound("creature/" + creatureData.LungeSound);
				}
				else
				{
					Singleton<SLOTAudioManager>.Instance.PlaySound("battle/SFX_Attack_Lunge");
				}
				yield return new WaitForSeconds(creatureData.AttackMovementTime);
			}
			else
			{
				animator.SetTrigger("Charge");
				if (creatureData.ChargeSound != string.Empty)
				{
					Singleton<SLOTAudioManager>.Instance.PlaySound(creatureData.ChargeSound);
				}
				GameObject hitVFX2 = null;
				Singleton<DWBattleLane>.Instance.CreatureVFXPool.TryGetValue(creatureData.AttackChargeVFX, out hitVFX2);
				if (hitVFX2 == null)
				{
					hitVFX2 = Resources.Load("VFX/Creatures/" + creatureData.AttackChargeVFX, typeof(GameObject)) as GameObject;
				}
				if (hitVFX2 != null)
				{
					List<Transform> trs = Singleton<DWGame>.Instance.FindAllInChildren(attackerObj.transform, creatureData.ShootVFXAttachBone);
					foreach (Transform tr in trs)
					{
						tr.InstantiateAsChild(hitVFX2);
					}
				}
				yield return new WaitForSeconds(creatureData.ChargeTime);
			}
		}
		GameObject mTrailObj = null;
		if (!isMagicAttack)
		{
			animator.SetTrigger("Attack");
			if (creatureData.SmashSound != string.Empty)
			{
				Singleton<SLOTAudioManager>.Instance.PlaySound(creatureData.SmashSound);
			}
			float trailFrameDelay = creatureData.WeaponTrailStartTime;
			yield return new WaitForSeconds(trailFrameDelay);
			if (creatureData.WeaponTrailVFX != null)
			{
				Transform attachPoint = FindInChildren(attackerObj.transform, "TrailAttach");
				if (attachPoint != null)
				{
					GameObject trailVFX = null;
					Singleton<DWBattleLane>.Instance.CreatureVFXPool.TryGetValue(creatureData.WeaponTrailVFX, out trailVFX);
					if (trailVFX == null)
					{
						trailVFX = (GameObject)Singleton<SLOTResourceManager>.Instance.LoadResource("VFX/Creatures/" + creatureData.WeaponTrailVFX);
					}
					if (trailVFX != null)
					{
						mTrailObj = SLOTGame.InstantiateFX(trailVFX, attachPoint.position, attachPoint.rotation) as GameObject;
						if (mTrailObj != null)
						{
							mTrailObj.transform.parent = attachPoint;
						}
					}
				}
			}
			float attackAfterTrailTime = creatureData.AttackStartTime - trailFrameDelay;
			if (attackAfterTrailTime > 0f)
			{
				yield return new WaitForSeconds(attackAfterTrailTime);
			}
		}
		else
		{
			animator.SetTrigger("Shoot");
			yield return new WaitForSeconds(creatureData.ShootStartTime);
			if (creatureData.ShootSound != string.Empty)
			{
				Singleton<SLOTAudioManager>.Instance.PlaySound(creatureData.ShootSound);
			}
			List<GameObject> projectiles = new List<GameObject>();
			mFollowingProjectiles.Clear();
			foreach (CreatureAttackTarget target2 in attacker.Targets)
			{
				GameObject shootVFX = null;
				Singleton<DWBattleLane>.Instance.CreatureVFXPool.TryGetValue(creatureData.ShootVFX, out shootVFX);
				if (shootVFX == null)
				{
					shootVFX = Resources.Load("VFX/Creatures/" + creatureData.ShootVFX, typeof(GameObject)) as GameObject;
				}
				if (!(shootVFX != null))
				{
					continue;
				}
				List<Transform> trs2 = Singleton<DWGame>.Instance.FindAllInChildren(attackerObj.transform, creatureData.ShootVFXAttachBone);
				foreach (Transform tr2 in trs2)
				{
					Quaternion rot = Quaternion.LookRotation(targetPos - tr2.position);
					GameObject projectile2 = SLOTGame.InstantiateFX(shootVFX, tr2.position, rot) as GameObject;
					projectiles.Add(projectile2);
					mFollowingProjectiles.Add(projectile2.transform);
					UpdateProjectileFollower();
					VFXRotateParticleScript rotationScript = projectile2.GetComponentInChildren<VFXRotateParticleScript>();
					if (rotationScript != null)
					{
						rotationScript.ShouldRotateParticle = true;
						rotationScript.RotationAxis = rot.eulerAngles;
					}
					bool critCamMovement = useCritCamera && projectiles.Count == 1 && lastAttack;
					Vector3 thisTargetPos = GetAttackPosition(target2.Creature);
					if (creatureData.ShootVFXType != "Dragon")
					{
						if (critCamMovement)
						{
							Singleton<DWGameCamera>.Instance.SetObjectLock(ProjectileFollower);
						}
						iTween.MoveTo(projectile2, iTween.Hash("position", thisTargetPos, "time", creatureData.ShootTravelTime, "easetype", creatureData.ShootTravelStyle));
						continue;
					}
					if (critCamMovement)
					{
						Vector3 camTargetPos = GetMeleeAttackPosition(attacker.Creature, target2.Creature);
						Vector3 delta2 = camTargetPos - Singleton<DWGameCamera>.Instance.MainCamLookAt.transform.position;
						Vector3 newCamPos = Singleton<DWGameCamera>.Instance.MainCam.transform.position + delta2;
						iTween.MoveTo(Singleton<DWGameCamera>.Instance.MainCamLookAt.gameObject, iTween.Hash("position", camTargetPos, "time", creatureData.ShootTravelTime, "easetype", iTween.EaseType.easeInCubic));
						iTween.MoveTo(Singleton<DWGameCamera>.Instance.MainCam.gameObject, iTween.Hash("position", newCamPos, "time", creatureData.ShootTravelTime, "easetype", iTween.EaseType.easeInCubic));
					}
					float distance = Vector3.Distance(tr2.position, thisTargetPos);
					DragonShootModifier(projectile2, distance);
				}
			}
			yield return new WaitForSeconds(creatureData.ShootTravelTime);
			mFollowingProjectiles.Clear();
			foreach (GameObject projectile in projectiles)
			{
				StartCoroutine(KillParticleAfterDeath(projectile));
			}
		}
		foreach (CreatureAttackTarget target in attacker.Targets)
		{
			CreatureHit hit = target.Hits[0];
			GameObject hitVFX3 = null;
			Singleton<DWBattleLane>.Instance.CreatureVFXPool.TryGetValue(creatureData.HitVFX, out hitVFX3);
			if (hitVFX3 == null)
			{
				hitVFX3 = Resources.Load("VFX/Creatures/" + creatureData.HitVFX, typeof(GameObject)) as GameObject;
			}
			Vector3 effectPos = GetAttackPosition(target.Creature);
			if (hit.Shield)
			{
				GameObject sVFX = SLOTGame.InstantiateFX(ShieldHitFX, effectPos, Quaternion.identity) as GameObject;
				if (target.Creature.Owner.Type == PlayerType.User)
				{
					sVFX.transform.position = new Vector3(-13f, sVFX.transform.position.y, sVFX.transform.position.z);
					sVFX.transform.eulerAngles = new Vector3(0f, 180f, 0f);
				}
			}
			else if (hit.Miss)
			{
				Singleton<SLOTAudioManager>.Instance.PlaySound("battle/SFX_Attack_Miss");
				DWBattleLaneObject laneObj = Singleton<DWBattleLane>.Instance.GetLaneObject(target.Creature);
				ShowEffectLabelPopup(laneObj.CreatureObject.transform.position, laneObj, KFFLocalization.Get("!!MISS"), Color.white, Color.black);
			}
			else
			{
				if (hitVFX3 != null)
				{
					SLOTGame.InstantiateFX(hitVFX3, effectPos, Quaternion.identity);
				}
				if (hit.Crit)
				{
					hitVFX3 = null;
					Singleton<DWBattleLane>.Instance.CreatureVFXPool.TryGetValue(creatureData.CritHitVFX, out hitVFX3);
					if (hitVFX3 == null)
					{
						hitVFX3 = Resources.Load("VFX/Creatures/" + creatureData.CritHitVFX, typeof(GameObject)) as GameObject;
					}
					if ((bool)hitVFX3)
					{
						GameObject fxObj = Instantiate(hitVFX3, effectPos, Quaternion.identity) as GameObject;
					}
				}
			}
			if (hit.HeroReactType == HitReactType.Small)
			{
				int rnd = Random.Range(0, 2);
				Singleton<CharacterAnimController>.Instance.PlayRandomHitReaction(!attacker.Creature.Owner.Type, rnd);
			}
			else if (hit.HeroReactType == HitReactType.Big)
			{
				Singleton<CharacterAnimController>.Instance.PlayHeroAnim(!attacker.Creature.Owner.Type, CharAnimType.BigActionReact);
			}
			ShowCreatureDamage(target.Creature, (int)hit.RawDamage, (int)hit.CappedDamage, hit.Crit, hit.HitType, hit.Shield || hit.Miss, hit.Killing);
		}
		if (isAiDragAttack)
		{
			Singleton<BattleLogController>.Instance.CanShowDragAttack = true;
		}
		if (!isMagicAttack)
		{
			yield return new WaitForSeconds(creatureData.MultiAttackInterval - creatureData.AttackStartTime);
		}
		else
		{
			yield return new WaitForSeconds(creatureData.MultiShootInterval - creatureData.ShootTravelTime - creatureData.ShootStartTime);
		}
		if (mTrailObj != null)
		{
			Object.Destroy(mTrailObj);
		}
		if (lastAttack)
		{
			if (!mInBattleFinishMoment)
			{
				Time.timeScale = 1f;
			}
			if (!isMagicAttack)
			{
				if (useCritCamera)
				{
					Singleton<DWGameCamera>.Instance.SetObjectLock(null);
				}
				iTween.MoveTo(attackerObj, iTween.Hash("position", attackerLane.AttackStartPosition, "time", creatureData.AttackMovementTime, "easetype", iTween.EaseType.easeInOutQuad));
				yield return new WaitForSeconds(creatureData.AttackMovementTime);
			}
			else
			{
				yield return new WaitForSeconds(RangedAttackFinishTime);
				if (useCritCamera)
				{
					Singleton<DWGameCamera>.Instance.SetObjectLock(null);
				}
			}
			float rotation = ((attacker.Creature.Owner.Type != PlayerType.User) ? (-90f) : 90f);
			iTween.RotateTo(attackerObj, iTween.Hash("rotation", new Vector3(0f, rotation, 0f), "time", RotateToAttackTime, "easetype", iTween.EaseType.easeInOutCubic));
			if (useCritCamera)
			{
				Singleton<DWGameCamera>.Instance.MoveCameraToP1Setup();
			}
		}
		mCreatureAttacksInProgress--;
	}

	private void ShowCreatureDamage(CreatureState creature, int rawAmount, int cappedAmount, bool isCrit, DamageType type, bool isMissOrShield, bool isFatal)
	{
		Singleton<BattleHudController>.Instance.ShowCreatureDamage(creature, rawAmount, cappedAmount, isCrit, type);
		if (!isMissOrShield)
		{
			FlashDamagedCreature(creature);
			PlayCreatureAnim(creature, "HitReaction");
			if (isCrit)
			{
				Singleton<DWGameCamera>.Instance.CameraShake(CritCameraShake, CritCameraShakeTime);
			}
			else
			{
				Singleton<DWGameCamera>.Instance.CameraShake(NormalCameraShake, NormalCameraShakeTime);
			}
		}
		if (!isFatal)
		{
			return;
		}
		Singleton<BattleHudController>.Instance.PlayWhiteScreenFlashTween();
		int num = 0;
		foreach (DWBattleLaneObject item in Singleton<DWBattleLane>.Instance.BattleLaneObjects[creature.Owner.Type.IntValue])
		{
			if (!item.DyingThisFrame)
			{
				num++;
			}
		}
		Singleton<DWBattleLane>.Instance.GetLaneObject(creature).DyingThisFrame = true;
		if (num == 1)
		{
			if (creature.Owner.Type == PlayerType.Opponent)
			{
				RegisterPlayerWin();
			}
			else
			{
				RegisterPlayerLoss();
			}
			Singleton<PauseController>.Instance.HideIfShowing();
			Singleton<PauseController>.Instance.HideButton();
			Singleton<QuickMessageController>.Instance.HideIfShowing();
			Singleton<QuickMessageController>.Instance.HideButton();
			StartCoroutine(BattleFinishMoment(creature.Owner.Type));
		}
	}

	public void RegisterPlayerWin()
	{
		DetachedSingleton<MissionManager>.Instance.ProcessWin();
		if (Singleton<PlayerInfoScript>.Instance.StateData.MultiplayerMode)
		{
			Singleton<MultiplayerMessageHandler>.Instance.SendLeaveGame("won");
			PvpMatchResultDetails resultDetails = Singleton<PlayerInfoScript>.Instance.RegisterPvpMatchResult(true);
			Singleton<PvpBattleResultsController>.Instance.RegisterResults(resultDetails);
			Singleton<PlayerInfoScript>.Instance.Save();
			return;
		}
		if (Singleton<TutorialController>.Instance.IsBlockActive("Q1"))
		{
			TutorialDataManager.Instance.GetBlock("Q1").Completed = true;
		}
		else if (Singleton<TutorialController>.Instance.IsBlockActive("Q2"))
		{
			TutorialDataManager.Instance.GetBlock("Q2").Completed = true;
		}
		QuestData currentActiveQuest = Singleton<PlayerInfoScript>.Instance.StateData.CurrentActiveQuest;
		if (currentActiveQuest.IsRandomDungeonBattle && Singleton<PlayerInfoScript>.Instance.StateData.NoReviveRandomDungeonRun && (int)Singleton<PlayerInfoScript>.Instance.SaveData.RandomDungeonLevel < RandomDungeonFloorDataManager.Instance.GetDatabase().Count && currentActiveQuest.IsFinalRandomBattle)
		{
			++Singleton<PlayerInfoScript>.Instance.SaveData.RandomDungeonLevel;
		}
		Singleton<BattleResultsController>.Instance.PopulateRewards();
		if (currentActiveQuest.BroadcastWin)
		{
			Singleton<PlayerInfoScript>.Instance.StateData.QuestWinToBroadcast = currentActiveQuest;
		}
	}

	public void RegisterPlayerLoss()
	{
		if (Singleton<PlayerInfoScript>.Instance.StateData.MultiplayerMode)
		{
			Singleton<MultiplayerMessageHandler>.Instance.SendLeaveGame("lost");
			PvpMatchResultDetails resultDetails = Singleton<PlayerInfoScript>.Instance.RegisterPvpMatchResult(false);
			Singleton<PvpBattleResultsController>.Instance.RegisterResults(resultDetails);
			Singleton<PlayerInfoScript>.Instance.Save();
		}
	}

	private IEnumerator BattleFinishMoment(PlayerType loser)
	{
		mInBattleFinishMoment = true;
		if (Singleton<TutorialController>.Instance.IsBlockActive("Q1"))
		{
			UICamera.LockInput();
		}
		if (loser == PlayerType.Opponent)
		{
			Singleton<SLOTMusic>.Instance.PlayVictoryMusic();
		}
		else
		{
			Singleton<SLOTMusic>.Instance.PlayLoserMusic();
		}
		Time.timeScale = 0.2f;
		yield return new WaitForSeconds(2f * Time.timeScale);
		Time.timeScale = 1f;
		mInBattleFinishMoment = false;
	}

	private void ShowCreatureHealing(CreatureState creature, int rawAmount, int cappedAmount)
	{
		Singleton<BattleHudController>.Instance.ShowCreatureHealing(creature, rawAmount, cappedAmount);
	}

	private bool IsTransitioningBetweenWaves()
	{
		GameState currentGameState = Singleton<DWGame>.Instance.GetCurrentGameState();
		return currentGameState == GameState.EndGameWait;
	}

	private IEnumerator PlayHeroCardDrawAnim(PlayerType player)
	{
		if (player == PlayerType.User)
		{
			Singleton<HandCardController>.Instance.HideHand();
			Singleton<HandCardController>.Instance.HideOpponentTween.Play();
			Singleton<BattleHudController>.Instance.HideHudForInfoPopupTween.Play();
			Singleton<BattleHudController>.Instance.PlayFloopBanner();
			Singleton<DWGameCamera>.Instance.RenderP1Character(true);
			Singleton<DWGameCamera>.Instance.MoveCameraToP1Winner();
		}
		else
		{
			Singleton<DWGameCamera>.Instance.MoveCameraToP2Setup();
		}
		yield return new WaitForSeconds(0.5f);
		Singleton<SLOTAudioManager>.Instance.TriggerVOEvent(Singleton<DWGame>.Instance.GetCharacter(player), VOEvent.DrawLeaderCard);
		if (!mHeroCardAlreadyPlayed[player.IntValue])
		{
			mHeroCardAlreadyPlayed[player.IntValue] = true;
			Singleton<CharacterAnimController>.Instance.PlayHeroAnim(player, CharAnimType.HeroCard_01);
		}
		else
		{
			Singleton<CharacterAnimController>.Instance.PlayHeroAnim(player, CharAnimType.HeroCard_00);
		}
		BattleCharacterAnimState animState = Singleton<CharacterAnimController>.Instance.playerAnimState[player];
		bool isDone = false;
		float timePast = 0f;
		while (!isDone)
		{
			timePast += Time.deltaTime;
			if (animState.GetCurrentAnimType() == CharAnimType.HeroCard_00 || animState.GetCurrentAnimType() == CharAnimType.HeroCard_01)
			{
				if (animState.IsCurrentAnimDone())
				{
					isDone = true;
				}
				else
				{
					yield return null;
				}
			}
			else if (timePast >= 2.66f)
			{
				isDone = true;
			}
			else
			{
				yield return null;
			}
		}
		Singleton<DWGameCamera>.Instance.MoveCameraToP1Setup();
		yield return new WaitForSeconds(0.2f);
		if (player == PlayerType.User)
		{
			Singleton<DWGameCamera>.Instance.RenderP1Character(false);
			Singleton<HandCardController>.Instance.ShowHand();
			Singleton<HandCardController>.Instance.ShowOpponentTween.Play();
			Singleton<BattleHudController>.Instance.ShowHudAfterInfoPopupTween.Play();
		}
	}

	private IEnumerator KillParticleAfterDeath(GameObject particleObj)
	{
		ParticleSystem[] pSystems = particleObj.GetComponentsInChildren<ParticleSystem>();
		ParticleSystem[] array = pSystems;
		foreach (ParticleSystem ps in array)
		{
			if (ps != null)
			{
				ps.loop = false;
				ps.enableEmission = false;
				ps.Stop();
			}
		}
		ParticleSystem[] array2 = pSystems;
		foreach (ParticleSystem ps2 in array2)
		{
			while (ps2 != null && ps2.IsAlive(false))
			{
				yield return null;
			}
		}
		Object.Destroy(particleObj);
	}

	private float GetMinMaxDistanceBetweenCreatures(bool min)
	{
		float num = ((!min) ? 0f : 100f);
		foreach (DWBattleLaneObject item in Singleton<DWBattleLane>.Instance.BattleLaneObjects[0])
		{
			foreach (DWBattleLaneObject item2 in Singleton<DWBattleLane>.Instance.BattleLaneObjects[1])
			{
				float b = Vector3.Distance(item.transform.position, item2.transform.position);
				num = ((!min) ? Mathf.Max(num, b) : Mathf.Min(num, b));
			}
		}
		return num;
	}

	private void DragonShootModifier(GameObject obj, float distance)
	{
		float minMaxDistanceBetweenCreatures = GetMinMaxDistanceBetweenCreatures(true);
		ParticleSystem[] componentsInChildren = obj.GetComponentsInChildren<ParticleSystem>();
		ParticleSystem[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].startSpeed *= distance / minMaxDistanceBetweenCreatures;
		}
	}

	private void UpdateProjectileFollower()
	{
		if (mFollowingProjectiles.Count == 0)
		{
			return;
		}
		Vector3 zero = Vector3.zero;
		foreach (Transform mFollowingProjectile in mFollowingProjectiles)
		{
			zero += mFollowingProjectile.position;
		}
		zero /= (float)mFollowingProjectiles.Count;
		ProjectileFollower.position = zero;
	}

	public void SetDragDrawCard(CardPrefabScript card)
	{
		mDragDrawCard = card;
	}

	public CardPrefabScript RetrieveDragDrawCard()
	{
		CardPrefabScript cardPrefabScript = mDragDrawCard;
		mDragDrawCard = null;
		if (cardPrefabScript != null)
		{
			cardPrefabScript.SetTargetAlpha(1f, 20f);
		}
		return cardPrefabScript;
	}
}
