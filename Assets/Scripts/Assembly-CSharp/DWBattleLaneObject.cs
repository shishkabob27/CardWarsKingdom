using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DWBattleLaneObject : MonoBehaviour
{
	public float LandscapeLiftLerpSpeed;

	public CreatureState Creature;

	public ShadowBlob ShadowBlob;

	public GameObject CreatureObject;

	public GameObject SwappedCreatureObject;

	public CreatureHPBar HealthBar;

	public BoxCollider LaneCollider;

	public bool IsFrozen;

	public bool IsBunny;

	private bool mHolding;

	private float mHoldTime;

	private Vector3 mHoldStartPos;

	private DWBattleLaneObject mPrevHoveredLane;

	private CardPrefabScript mDraggingToDrawCard;

	private bool mHasLandscapeEffect;

	private static CreatureState mSelectedCardTarget;

	public bool IgnoreBoardTint;

	public bool IsTransfmogrified;

	private List<GameObject> CreatureStatusFXObjects = new List<GameObject>();

	private List<GameEvent> ActiveCreaturePersistentFXList = new List<GameEvent>();

	private GameObject mFXObjToFadeOut;

	private bool mFadeOutStatus;

	private float mFadeOutTime = 2f;

	public bool InitialPositionSet { get; set; }

	public Vector3 AttackStartPosition { get; set; }

	public bool DyingThisFrame { get; set; }

	public ShowEffectDisplayPopup AttachedEffectPopup { get; set; }

	private void Awake()
	{
		Transform[] componentsInChildren = base.transform.GetComponentsInChildren<Transform>(true);
		Transform[] array = componentsInChildren;
		foreach (Transform transform in array)
		{
			if (transform.name == "ShadowBlob")
			{
				ShadowBlob = transform.GetComponent<ShadowBlob>();
			}
		}
		LaneCollider = GetComponent<BoxCollider>();
	}

	private Vector3 GetPosOnCircle(float angle, float radius)
	{
		float z = Mathf.Cos(angle * ((float)Math.PI / 180f)) * radius;
		float x = Mathf.Sin(angle * ((float)Math.PI / 180f)) * radius;
		return new Vector3(x, 0f, z);
	}

	private List<Color> GetColorArray()
	{
		List<Color> list = new List<Color>();
		list.Add(Color.Lerp(Color.yellow, Color.green, 0.5f));
		list.Add(Color.yellow);
		list.Add(Color.Lerp(Color.yellow, Color.red, 0.3f));
		list.Add(Color.Lerp(Color.yellow, Color.red, 0.7f));
		list.Add(Color.Lerp(Color.grey, Color.red, 0.5f));
		list.Add(Color.Lerp(Color.red, Color.white, 0.5f));
		list.Add(Color.magenta);
		list.Add(Color.blue);
		list.Add(Color.cyan);
		list.Add(Color.green);
		return list;
	}

	private void OnClick()
	{
		if (CanInteract() && !Singleton<DWGame>.Instance.SelectingLane && Creature != null && !Singleton<TutorialController>.Instance.IsStateActive("Q1_BattlePlayCard"))
		{
			ShowCreatureInfoPopup();
		}
	}

	private void OnDragOver(GameObject go)
	{
		if (Singleton<DWGame>.Instance.SelectingLane)
		{
			if (Creature.Owner.Type == PlayerType.Opponent)
			{
				mSelectedCardTarget = Creature;
				Singleton<DWGame>.Instance.SetTarget(PlayerType.User, Creature);
				Singleton<DWBattleLane>.Instance.ShowDamagePredictions(Singleton<DWBattleLane>.Instance.CurrentActionTarget.Creature.PredictCardDamage(Singleton<DWBattleLane>.Instance.CurrentCard.Card));
				Singleton<DWBattleLane>.Instance.SetTargetIndicators(Singleton<DWBattleLane>.Instance.CurrentCard.Card, this, true);
			}
			else
			{
				mSelectedCardTarget = null;
				Singleton<DWBattleLane>.Instance.HideDamagePredictions();
				Singleton<DWBattleLane>.Instance.HideTargetIndicators();
			}
		}
	}

	private void OnPress(bool pressed)
	{
		if (pressed && CanInteract())
		{
			mHolding = true;
			mHoldTime = 0f;
			mHoldStartPos = Input.mousePosition;
			if (Singleton<DWGame>.Instance.SelectingLane)
			{
				OnDragOver(null);
			}
			return;
		}
		mHolding = false;
		if (CanInteract())
		{
			if (Singleton<DWGame>.Instance.SelectingLane)
			{
				if (mSelectedCardTarget != null)
				{
					if (Singleton<DWGame>.Instance.IsCreatureTargetRestricted(mSelectedCardTarget))
					{
						Singleton<BattleHudController>.Instance.ShowErrorReason(KFFLocalization.Get("!!MUST_TARGET_BRAVERY"));
						Singleton<SLOTAudioManager>.Instance.PlaySound("battle/SFX_Announcer_TryAgain");
					}
					else
					{
						StartCoroutine(Singleton<DWBattleLane>.Instance.CurrentCard.ShowPlayAnim());
						Singleton<DWBattleLane>.Instance.EndTargetSelection();
						CreatureState creature = Singleton<DWBattleLane>.Instance.CurrentActionTarget.Creature;
						Singleton<DWGame>.Instance.PlayActionCard(PlayerType.User, Singleton<DWBattleLane>.Instance.CurrentCard.Card, creature.Owner.Type, creature.Lane.Index);
						Singleton<DWGame>.Instance.SelectingLane = false;
					}
				}
			}
			else if ((Input.mousePosition - mHoldStartPos).y > Singleton<DWBattleLane>.Instance.AttackDragStartDistance)
			{
				if (mPrevHoveredLane != null && mPrevHoveredLane.Creature.Owner.Type == PlayerType.Opponent)
				{
					Singleton<BattleHudController>.Instance.HideDragActionCost();
					Singleton<DWBattleLane>.Instance.HideDragArrow();
					HealthBar.FlashAttackCostTween.StopAndReset();
					HealthBar.FlashAttackValueTween.StopAndReset();
					if (!Singleton<TutorialController>.Instance.IsStateActive("Q3C_LeaderBar4"))
					{
						if (Singleton<DWGame>.Instance.IsCreatureTargetRestricted(mPrevHoveredLane.Creature))
						{
							Singleton<BattleHudController>.Instance.ShowErrorReason(KFFLocalization.Get("!!MUST_TARGET_BRAVERY"));
							Singleton<SLOTAudioManager>.Instance.PlaySound("battle/SFX_Announcer_TryAgain");
						}
						else if (Singleton<DWGame>.Instance.GetActionPoints(PlayerType.User) < Creature.AttackCost && !Singleton<TutorialController>.Instance.IgnoreEnergyCosts())
						{
							Singleton<BattleHudController>.Instance.ShowErrorReason(KFFLocalization.Get("!!NOT_ENOUGH_ENERGY"));
							Singleton<SLOTAudioManager>.Instance.PlaySound("battle/SFX_Announcer_OutofEnergy");
						}
						else
						{
							if (Singleton<PlayerInfoScript>.Instance.StateData.MultiplayerMode)
							{
								Singleton<MultiplayerMessageHandler>.Instance.SendDragAttack(Creature.Lane.Index, mPrevHoveredLane.Creature.Lane.Index);
							}
							DetachedSingleton<ConditionalTutorialController>.Instance.OnCreatureAttack(Creature);
							Singleton<TutorialController>.Instance.CheckTutorialForceDraw(Creature);
							Singleton<DWGame>.Instance.DragAttack(PlayerType.User, Creature.Lane.Index, mPrevHoveredLane.Creature.Lane.Index);
							Singleton<TutorialController>.Instance.PassIfOnAttack();
							Singleton<SLOTAudioManager>.Instance.SetVOEventCooldown(VOEvent.Idle);
						}
					}
				}
				else if (mDraggingToDrawCard != null)
				{
					if (Singleton<DWGame>.Instance.GetActionPoints(PlayerType.User) < Creature.AttackCost && !Singleton<TutorialController>.Instance.IgnoreEnergyCosts())
					{
						Singleton<BattleHudController>.Instance.ShowErrorReason(KFFLocalization.Get("!!NOT_ENOUGH_ENERGY"));
						Singleton<SLOTAudioManager>.Instance.PlaySound("battle/SFX_Announcer_OutofEnergy");
						NGUITools.Destroy(mDraggingToDrawCard.gameObject);
					}
					else
					{
						if (Singleton<PlayerInfoScript>.Instance.StateData.MultiplayerMode)
						{
							Singleton<MultiplayerMessageHandler>.Instance.SendDragAttack(Creature.Lane.Index, -1);
						}
						Singleton<DWGameMessageHandler>.Instance.SetDragDrawCard(mDraggingToDrawCard);
						mDraggingToDrawCard.SetTargetAlpha(0f, 8f);
						Singleton<DWGame>.Instance.DragAttack(PlayerType.User, Creature.Lane.Index, -1);
						Singleton<SLOTAudioManager>.Instance.SetVOEventCooldown(VOEvent.Idle);
					}
					mDraggingToDrawCard = null;
				}
			}
		}
		Singleton<DWBattleLane>.Instance.HideTargetIndicators();
		Singleton<DWBattleLane>.Instance.HideDamagePredictions();
		mPrevHoveredLane = null;
	}

	public void CancelPress()
	{
		if (mDraggingToDrawCard != null)
		{
			NGUITools.Destroy(mDraggingToDrawCard.gameObject);
			mDraggingToDrawCard = null;
		}
		mPrevHoveredLane = null;
		HideDragAttackInfo();
		mHolding = false;
	}

	private bool CanInteract()
	{
		return Singleton<DWGame>.Instance.GetCurrentGameState().IsP1Turn() && !Singleton<DWBattleLane>.Instance.LootObjectsToCollect();
	}

	private void Update()
	{
		if (mHolding && CanInteract() && !Singleton<DWGame>.Instance.SelectingLane && Creature != null && Creature.Owner.Type == PlayerType.User && !IsFrozen)
		{
			if ((Input.mousePosition - mHoldStartPos).y > Singleton<DWBattleLane>.Instance.AttackDragStartDistance)
			{
				if (Singleton<DWGame>.Instance.CurrentBoardState.GetCreatureCount(PlayerType.Opponent) == 0)
				{
					if (mDraggingToDrawCard == null)
					{
						mDraggingToDrawCard = Singleton<HandCardController>.Instance.CreateDragDrawCard();
					}
				}
				else
				{
					DWBattleLaneObject hoveredLane = BattleHudController.GetHoveredLane();
					if (hoveredLane != mPrevHoveredLane)
					{
						if (hoveredLane != null && hoveredLane.Creature.Owner.Type == PlayerType.Opponent)
						{
							Singleton<BattleHudController>.Instance.ShowDragActionCost(Creature.AttackCost);
							Singleton<DWBattleLane>.Instance.ShowDragArrow(CreatureObject);
							HealthBar.FlashAttackCostTween.Play();
							HealthBar.FlashAttackValueTween.Play();
							bool validTarget;
							if (Singleton<DWGame>.Instance.IsCreatureTargetRestricted(hoveredLane.Creature))
							{
								validTarget = false;
								Singleton<DWBattleLane>.Instance.HideDamagePredictions();
							}
							else
							{
								validTarget = true;
								Singleton<DWBattleLane>.Instance.ShowDamagePredictions(Creature.PredictDragAttackDamage(hoveredLane.Creature));
							}
							Singleton<DWBattleLane>.Instance.SetTargetIndicator(hoveredLane.transform.position, validTarget);
						}
						else
						{
							HideDragAttackInfo();
						}
					}
					mPrevHoveredLane = hoveredLane;
				}
			}
			else
			{
				mPrevHoveredLane = null;
				HideDragAttackInfo();
				if (mDraggingToDrawCard != null)
				{
					NGUITools.Destroy(mDraggingToDrawCard.gameObject);
					mDraggingToDrawCard = null;
				}
			}
		}
		float num = ((!mHasLandscapeEffect) ? 0f : 1f);
		if (base.transform.position.y != num)
		{
			Vector3 position = base.transform.position;
			position.y = num;
			base.transform.position = Vector3.Lerp(base.transform.position, position, LandscapeLiftLerpSpeed * Time.deltaTime);
			if (Mathf.Abs(base.transform.position.y - num) < 0.01f)
			{
				base.transform.position = position;
			}
		}
	}

	public void HideDragAttackInfo(bool atTimeUp = false)
	{
		if (mHolding)
		{
			Singleton<BattleHudController>.Instance.HideDragActionCost();
			Singleton<DWBattleLane>.Instance.HideTargetIndicators();
			Singleton<DWBattleLane>.Instance.HideDragArrow();
			Singleton<DWBattleLane>.Instance.HideDamagePredictions();
			if (HealthBar != null && HealthBar.FlashAttackCostTween != null)
			{
				HealthBar.FlashAttackCostTween.StopAndReset();
			}
			if (HealthBar != null && HealthBar.FlashAttackValueTween != null)
			{
				HealthBar.FlashAttackValueTween.StopAndReset();
			}
			if (atTimeUp)
			{
				mHolding = false;
				mPrevHoveredLane = null;
			}
		}
	}

	private void ShowCreatureInfoPopup()
	{
		Singleton<CreatureInfoPopup>.Instance.Show(Creature);
		Singleton<DWGameCamera>.Instance.RenderP1Character(true);
		if (!Singleton<DWGameCamera>.Instance.UseDetailCam)
		{
			Singleton<DWGameCamera>.Instance.MoveCameraToCreatureDetail(Creature, true);
		}
		else
		{
			StartCoroutine(SetCreatureDetailCam());
		}
	}

	private IEnumerator SetCreatureDetailCam()
	{
		yield return new WaitForSeconds(0.3f);
		Singleton<DWGameCamera>.Instance.SetCreatureDetailCam(true, Creature);
	}

	private void OnDragStart()
	{
	}

	private void OnDrag()
	{
	}

	private void OnDragEnd()
	{
	}

	public void SetColliders(bool enable)
	{
		Collider[] componentsInChildren = GetComponentsInChildren<Collider>();
		Collider[] array = componentsInChildren;
		foreach (Collider collider in array)
		{
			collider.enabled = enable;
		}
	}

	public void FreezeCreature()
	{
		IsFrozen = true;
		StartCoroutine(StopCreatureAnimAfterDelay(0.1f));
		SwapMaterials(Singleton<DWGameMessageHandler>.Instance.FreezeDesatMaterial);
	}

	public void AssignFadeMatToCreature()
	{
		SwapMaterials(Singleton<DWBattleLane>.Instance.CreatureDeathMaterial);
	}

	private void SwapMaterials(Material matToSwap)
	{
		IgnoreBoardTint = true;
		BattleCreatureAnimState[] componentsInChildren = CreatureObject.GetComponentsInChildren<BattleCreatureAnimState>(true);
		if (componentsInChildren == null)
		{
			return;
		}
		List<BattleCreatureAnimState> list = new List<BattleCreatureAnimState>();
		list.Add(componentsInChildren[0]);
		if (SwappedCreatureObject != null)
		{
			BattleCreatureAnimState componentInChildren = SwappedCreatureObject.GetComponentInChildren<BattleCreatureAnimState>();
			list.Add(componentInChildren);
		}
		BattleCreatureAnimState[] array = componentsInChildren;
		foreach (BattleCreatureAnimState battleCreatureAnimState in array)
		{
			for (int j = 0; j < battleCreatureAnimState.orignalMeshes.Count; j++)
			{
				SkinnedMeshRenderer skinnedMeshRenderer = battleCreatureAnimState.orignalMeshes[j];
				Material material = UnityEngine.Object.Instantiate(matToSwap);
				material.mainTexture = skinnedMeshRenderer.material.mainTexture;
				skinnedMeshRenderer.material = material;
			}
		}
	}

	public void RevertMaterials()
	{
		IgnoreBoardTint = false;
		BattleCreatureAnimState[] componentsInChildren = CreatureObject.GetComponentsInChildren<BattleCreatureAnimState>(true);
		if (componentsInChildren != null)
		{
			BattleCreatureAnimState battleCreatureAnimState = componentsInChildren[0];
			for (int i = 0; i < battleCreatureAnimState.orignalMeshes.Count; i++)
			{
				battleCreatureAnimState.orignalMeshes[i].material = battleCreatureAnimState.originalMats[i];
			}
		}
	}

	public void UnfreezeCreature()
	{
		IsFrozen = false;
		RestartCreatureIdle();
		RevertMaterials();
	}

	public void StealthCreature()
	{
		SwapMaterials(Singleton<DWGameMessageHandler>.Instance.StealthMaterial);
	}

	public void UnStealthCreature()
	{
		RevertMaterials();
	}

	public void TransmogrifyCreature()
	{
		IsBunny = true;
		Transform transform = CreatureObject.transform;
		if (SwappedCreatureObject != null)
		{
			return;
		}
		SwappedCreatureObject = base.transform.InstantiateAsChild(Singleton<DWGameMessageHandler>.Instance.TransmogrifyCreature);
		if (IsFrozen)
		{
			Animator componentInChildren = SwappedCreatureObject.GetComponentInChildren<Animator>();
			if (componentInChildren != null)
			{
				componentInChildren.speed = 0f;
			}
		}
		SwappedCreatureObject.transform.rotation = transform.rotation;
		BattleCreatureAnimState componentInChildren2 = SwappedCreatureObject.GetComponentInChildren<BattleCreatureAnimState>();
		for (int i = 0; i < ActiveCreaturePersistentFXList.Count; i++)
		{
			if (ActiveCreaturePersistentFXList[i] == GameEvent.ENABLE_BLIND)
			{
				CreatureStatusFXObjects[i].transform.parent = componentInChildren2.AttachBoneBlindEffect;
			}
			else if (CreatureStatusFXObjects[i].transform.parent == CreatureObject.transform)
			{
				CreatureStatusFXObjects[i].transform.parent = SwappedCreatureObject.transform;
			}
		}
		CreatureObject.SetActive(false);
	}

	public void UntransmogrifyCreature()
	{
		IsBunny = false;
		CreatureObject.SetActive(true);
		BattleCreatureAnimState componentInChildren = CreatureObject.GetComponentInChildren<BattleCreatureAnimState>();
		for (int i = 0; i < ActiveCreaturePersistentFXList.Count; i++)
		{
			if (ActiveCreaturePersistentFXList[i] == GameEvent.ENABLE_BLIND)
			{
				CreatureStatusFXObjects[i].transform.parent = componentInChildren.AttachBoneBlindEffect;
			}
			else if (CreatureStatusFXObjects[i].transform.parent == SwappedCreatureObject.transform)
			{
				CreatureStatusFXObjects[i].transform.parent = CreatureObject.transform;
			}
		}
		UnityEngine.Object.Destroy(SwappedCreatureObject);
		SwappedCreatureObject = null;
	}

	public void ApplyStatusEffectObjOnCreature(GameMessage ms)
	{
		if (ActiveCreaturePersistentFXList.Contains(ms.Action))
		{
			return;
		}
		GameObject creatureObject = Singleton<DWBattleLane>.Instance.GetCreatureObject(Creature);
		Transform transform = creatureObject.transform;
		Vector3 localPosition = Vector3.zero;
		if (ms.Action == GameEvent.ENABLE_BLIND)
		{
			Transform transform2 = Singleton<DWGameMessageHandler>.Instance.FindInChildren(creatureObject.transform, "Puppet_Head");
			BattleCreatureAnimState componentInChildren = creatureObject.GetComponentInChildren<BattleCreatureAnimState>();
			if (componentInChildren.AttachBoneBlindEffect != null)
			{
				transform2 = componentInChildren.AttachBoneBlindEffect;
			}
			localPosition = componentInChildren.BlindEffectLocalOffset;
			if (transform2 != null)
			{
				transform = transform2;
			}
		}
		GameEventFXData data = GameEventFXDataManager.Instance.GetData(ms.Action.ToString());
		GameObject resource = Resources.Load("VFX/Actions/" + data.CreatureStatusFXPrefab, typeof(GameObject)) as GameObject;
		if (resource != null)
		{
			if (data.DetachStatusFXFromCreature)
			{
				transform = transform.transform.parent;
			}
			GameObject gameObject = transform.InstantiateAsChild(resource);
			gameObject.transform.localPosition = localPosition;
			ActiveCreaturePersistentFXList.Add(ms.Action);
			CreatureStatusFXObjects.Add(gameObject);
		}
		if (ms.Status != null && ms.Status.IsLandscape)
		{
			mHasLandscapeEffect = true;
		}
	}

	public void RemoveStatusEffectObjFromCreature(GameMessage ms)
	{
		GameEvent disableEventFX = GetDisableEventFX(ms);
		if (disableEventFX == GameEvent.NONE)
		{
			return;
		}
		for (int i = 0; i < ActiveCreaturePersistentFXList.Count; i++)
		{
			GameEvent gameEvent = ActiveCreaturePersistentFXList[i];
			if (gameEvent == disableEventFX)
			{
				ActiveCreaturePersistentFXList.RemoveAt(i);
				GameObject gameObject = CreatureStatusFXObjects[i];
				CreatureStatusFXObjects.RemoveAt(i);
				if (gameObject != null)
				{
					StartCoroutine(FadeOutStatusEffectFromCreature(gameObject));
				}
				break;
			}
		}
		if (ms.Status != null && ms.Status.IsLandscape)
		{
			mHasLandscapeEffect = false;
		}
	}

	public void RemoveAllStatusEffectObjFromCreature()
	{
		for (int i = 0; i < CreatureStatusFXObjects.Count; i++)
		{
			GameObject gameObject = CreatureStatusFXObjects[i];
			if (gameObject != null)
			{
				UnityEngine.Object.Destroy(gameObject);
			}
		}
	}

	private IEnumerator FadeOutStatusEffectFromCreature(GameObject obj)
	{
		Renderer[] mats = obj.GetComponentsInChildren<Renderer>(true);
		Renderer[] array = mats;
		foreach (Renderer mat in array)
		{
			if (mat.material.HasProperty("_Color"))
			{
				iTween.FadeTo(mat.gameObject, iTween.Hash("alpha", 0f, "time", 2f));
			}
		}
		ParticleSystem[] pSystems = obj.GetComponentsInChildren<ParticleSystem>(true);
		ParticleSystem[] array2 = pSystems;
		foreach (ParticleSystem ps in array2)
		{
			ps.loop = false;
		}
		UITexture[] uiTextures = obj.GetComponentsInChildren<UITexture>(true);
		UITexture[] array3 = uiTextures;
		foreach (UITexture tx in array3)
		{
			TweenAlpha tw = tx.gameObject.AddComponent<TweenAlpha>();
			tw.to = 0f;
			tw.duration = 2f;
			tw.PlayForward();
		}
		yield return new WaitForSeconds(2f);
		UnityEngine.Object.Destroy(obj);
	}

	private IEnumerator StopCreatureAnimAfterDelay(float delay)
	{
		Singleton<DWGameMessageHandler>.Instance.PlayCreatureAnim(Creature, "HitReaction");
		yield return new WaitForSeconds(delay);
		List<GameObject> creatures = Singleton<DWBattleLane>.Instance.GetCreatureObjects(Creature);
		foreach (GameObject obj in creatures)
		{
			if (obj != null)
			{
				Animator[] anims = obj.GetComponentsInChildren<Animator>(true);
				anims[0].speed = 0f;
			}
		}
	}

	private void RestartCreatureIdle()
	{
		GameObject creatureObject = Singleton<DWBattleLane>.Instance.GetCreatureObject(Creature);
		Animator[] componentsInChildren = creatureObject.GetComponentsInChildren<Animator>(true);
		componentsInChildren[0].speed = 1f;
		Singleton<DWGameMessageHandler>.Instance.PlayCreatureAnim(Creature, "ToIdle");
	}

	private GameEvent GetDisableEventFX(GameMessage ms)
	{
		GameEvent result = GameEvent.NONE;
		List<StatusData> database = StatusDataManager.Instance.GetDatabase();
		foreach (StatusData item in database)
		{
			if (item.DisableMessage == ms.Action)
			{
				return result = item.EnableMessage;
			}
		}
		return result;
	}
}
