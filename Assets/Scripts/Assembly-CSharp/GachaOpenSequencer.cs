using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GachaOpenSequencer : Singleton<GachaOpenSequencer>
{
	public float CreatureHeightMod = 1f;

	public int BattleCameraDepth;

	public UITweenController FadeToBlackTween;

	public UITweenController FadeBackInTween;

	public UITweenController CreatureInfoHideTween;

	public UITweenController CreatureChestOpenTween;

	public UITweenController ShowInfoPanelCloseButtonTween;

	public CreatureStatsPanel StatsPanel;

	public Transform ChestNode;

	public Camera ChestCamera;

	public Camera CreatureCamera;

	public List<Transform> CardNodes;

	public GameObject tipsPanel;

	public GameObject VFX_CardSwap_Prefab;

	public GameObject CreatureNode;

	private InventorySlotItem mCreature;

	private GameObject mLoadedModelInstance;

	public GameObject[] RarityToppers;

	public GameObject[] RarityGlows;

	public GameObject[] RarityPlatforms;

	private bool mClicked;

	public bool Showing { get; private set; }

	public void ShowGachaSequence(InventorySlotItem item, Action onOpened = null)
	{
		StartCoroutine(ShowGachaSequenceCo(item, onOpened));
		if (DetachedSingleton<SceneFlowManager>.Instance.InBattleScene())
		{
			Singleton<BattleResultsController>.Instance.HideRewardsDuringGacha.Play();
		}
	}

	private IEnumerator ShowGachaSequenceCo(InventorySlotItem item, Action onOpened)
	{
		Showing = true;
		if (DetachedSingleton<SceneFlowManager>.Instance.InBattleScene())
		{
			ChestCamera.depth = BattleCameraDepth;
			CreatureCamera.depth = BattleCameraDepth;
		}
		ChestCamera.enabled = true;
		CreatureCamera.enabled = true;
		bool isCreature = item.SlotType == InventorySlotType.Creature;
		if (isCreature)
		{
			mCreature = item;
		}
		GameObject chestObj = ChestNode.InstantiateAsChild(Singleton<PrefabReferences>.Instance.GachaChests[item.Rarity - 1]);
		chestObj.ChangeLayerToParent();
		ChestNode.gameObject.SetActive(false);
		GachaChestObject chest = chestObj.GetComponent<GachaChestObject>();
		if (chest == null)
		{
			chest = chestObj.GetComponentInChildren<GachaChestObject>();
		}
		CardPrefabScript spawnedCard = chest.CardNode.InstantiateAsChild(Singleton<PrefabReferences>.Instance.Card).GetComponent<CardPrefabScript>();
		spawnedCard.gameObject.ChangeLayerToParent();
		spawnedCard.transform.localPosition = Vector3.zero;
		spawnedCard.transform.localRotation = Quaternion.identity;
		spawnedCard.transform.localScale = Vector3.one;
		spawnedCard.InGacha = true;
		spawnedCard.Mode = CardPrefabScript.CardMode.GeneralFrontEnd;
		spawnedCard.CardCollider.enabled = false;
		if (item.SlotType == InventorySlotType.Card)
		{
			spawnedCard.Populate(item.Card.Form);
		}
		else
		{
			spawnedCard.Populate(item);
		}
		spawnedCard.gameObject.SetActive(false);
		if (isCreature)
		{
			StatsPanel.Populate(item);
			for (int j = 0; j < item.Creature.Form.ActionCards.Count; j++)
			{
				CardPrefabScript creatureCardObj = CardNodes[j].InstantiateAsChild(Singleton<PrefabReferences>.Instance.Card).GetComponent<CardPrefabScript>();
				creatureCardObj.gameObject.ChangeLayerToParent();
				creatureCardObj.Mode = CardPrefabScript.CardMode.GeneralFrontEnd;
				creatureCardObj.Populate(item.Creature.Form.ActionCards[j]);
				creatureCardObj.AdjustDepth(j + 1);
			}
		}
		FadeToBlackTween.Play();
		yield return new WaitForSeconds(0.3f);
		ChestNode.gameObject.SetActive(true);
		chest.Animator.Play("Intro");
		chest.EnableChestVFXObj(true);
		bool isSoftLocked = true;
		while (isSoftLocked)
		{
			if (UICamera.IsInputLocked())
			{
				UICamera.UnlockInput();
			}
			isSoftLocked = UICamera.IsInputLocked();
		}
		Singleton<SLOTAudioManager>.Instance.PlaySound("SFX_Gacha_ChestShow");
		Singleton<SLOTAudioManager>.Instance.PlaySound("SFX_LootChestBounce");
		Singleton<SLOTAudioManager>.Instance.PlaySound("SFX_LootChestStarLoop");
		mClicked = false;
		while (!mClicked)
		{
			yield return null;
		}
		if (onOpened != null)
		{
			onOpened();
		}
		chest.Animator.Play("Tap");
		Singleton<SLOTAudioManager>.Instance.StopSound("SFX_LootChestBounce");
		Singleton<SLOTAudioManager>.Instance.StopSound("SFX_LootChestStarLoop");
		Singleton<SLOTAudioManager>.Instance.PlaySound("SFX_Gacha_ChestOpen");
		if (tipsPanel != null)
		{
			tipsPanel.SetActive(false);
		}
		chest.EnableChestVFXObj(false);
		spawnedCard.gameObject.SetActive(true);
		yield return new WaitForSeconds(0.4f);
		if (!isCreature)
		{
			chest.EnableCardVFXObj(true);
		}
		yield return new WaitForSeconds(0.5f);
		Singleton<SLOTAudioManager>.Instance.PlaySound("SFX_LootChestShimmer");
		if (Singleton<TutorialController>.Instance.IsBlockActive("Q2"))
		{
			Singleton<TutorialController>.Instance.AdvanceTutorialState();
		}
		if (isCreature)
		{
			CreatureChestOpenTween.Play();
			GameObject thisFX = chest.CardNode.InstantiateAsChild(VFX_CardSwap_Prefab);
			thisFX.transform.SetParent(StatsPanel.transform.parent.transform);
			if (DetachedSingleton<SceneFlowManager>.Instance.InBattleScene())
			{
				Singleton<DWGameCamera>.Instance.Battle3DUICam.gameObject.SetActive(false);
			}
			StartCoroutine(LoadModelCo());
			while (mLoadedModelInstance == null)
			{
				yield return null;
			}
			CardPrefabScript thisActionCard = chest.CardNode.GetComponentInChildren<CardPrefabScript>();
			if (thisActionCard != null)
			{
				UIWidget thisWidget = thisActionCard.GetComponent<UIWidget>();
				if (thisWidget != null)
				{
					thisWidget.alpha = 0f;
				}
			}
			for (int i = 0; i < RarityToppers.Length; i++)
			{
				if (i == mCreature.Creature.Form.Rarity - 1)
				{
					RarityToppers[i].gameObject.SetActive(true);
					RarityGlows[i].gameObject.SetActive(true);
					RarityPlatforms[i].gameObject.SetActive(true);
				}
				else
				{
					RarityToppers[i].gameObject.SetActive(false);
					RarityGlows[i].gameObject.SetActive(false);
					RarityPlatforms[i].gameObject.SetActive(false);
				}
			}
		}
		mClicked = false;
		while (!mClicked)
		{
			yield return null;
		}
		chest.EnableCardVFXObj(false);
		chest.EnableChestVFXObj(false);
		chest.Animator.Play("Outro");
		Singleton<SLOTAudioManager>.Instance.PlaySound("SFX_Gacha_ChestClose");
		Singleton<SLOTAudioManager>.Instance.StopSound("SFX_LootChestShimmer");
		ChestNode.gameObject.SetActive(false);
		if (isCreature)
		{
			CreatureInfoHideTween.Play();
		}
		yield return StartCoroutine(FadeBackInTween.PlayAsCoroutine());
		if (DetachedSingleton<SceneFlowManager>.Instance.InBattleScene())
		{
			Singleton<BattleResultsController>.Instance.ShowRewardsAfterGacha.Play();
		}
		ChestNode.DestroyAllChildren();
		foreach (Transform cardNode in CardNodes)
		{
			cardNode.DestroyAllChildren();
		}
		CreatureCamera.enabled = false;
		Showing = false;
	}

	public void OnClickNextObject()
	{
		mClicked = true;
	}

	public void OutroComplete()
	{
		if (DetachedSingleton<SceneFlowManager>.Instance.InBattleScene())
		{
			Singleton<DWGameCamera>.Instance.Battle3DUICam.gameObject.SetActive(true);
		}
	}

	private IEnumerator LoadModelCo()
	{
		UnloadModel();
		yield return StartCoroutine(Singleton<SLOTResourceManager>.Instance.LoadCreatureResources(mCreature.Creature.Form, delegate(UnityEngine.Object objData, Texture2D texture)
		{
			CreatureNode.transform.localRotation = Quaternion.Euler(Vector3.zero);
			mLoadedModelInstance = CreatureNode.InstantiateAsChild((GameObject)objData);
			mCreature.Creature.Form.SwapCreatureTexture(mLoadedModelInstance, texture, true);
			SetToIdle(mLoadedModelInstance);
			float num = Mathf.Max(6f, mCreature.Creature.Form.Height);
			mLoadedModelInstance.transform.localScale *= 8f / num;
		}));
	}

	private void SetToIdle(GameObject creature)
	{
		BattleCreatureAnimState componentInChildren = creature.GetComponentInChildren<BattleCreatureAnimState>();
		if (componentInChildren != null && componentInChildren.anim != null)
		{
			componentInChildren.anim.Play("Idle");
		}
	}

	private void UnloadModel()
	{
		if (mLoadedModelInstance != null)
		{
			UnityEngine.Object.Destroy(mLoadedModelInstance);
			mLoadedModelInstance = null;
			Resources.UnloadUnusedAssets();
		}
	}
}
