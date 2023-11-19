using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KFFUpsightVGController : Singleton<KFFUpsightVGController>
{
	public enum PlacementStates
	{
		PlacementPending,
		PlacementStarted,
		PlacementFinished,
		PlacementFailed
	}

	public enum LaboratoryTrackEvent
	{
		PowerUp,
		Evolve,
		Awaken
	}

	public enum InventoryTrackEvent
	{
		Full,
		FullModal
	}

	public enum InventoryModalAction
	{
		Purchased,
		Closed
	}

	public enum BattleTrackProgress
	{
		QuestStart,
		QuestResult
	}

	public enum BattleTrackEvent
	{
		BattleStart,
		BattleRetry,
		BattleWon,
		BattleLost,
		BattleSuspended,
		BattleQuit,
		BattleContinue
	}

	public enum PVPWinCondition
	{
		Victory = 0,
		Concede = 0,
		Disconnect = 0,
		TimeOut = 0
	}

	private string purchaseID;

	private KFFNetwork.WWWInfo wwwVerifyPurchase;

	private string placementID;

	private PlacementStates _PlacementState;

	private string battleID = string.Empty;

	private static int mDebugPurchaseIdIndex;

	private PlacementStates PlacementState
	{
		get
		{
			return _PlacementState;
		}
		set
		{
			if (_PlacementState != value)
			{
				_PlacementState = value;
			}
		}
	}

	public bool IsPlacementInProgress
	{
		get
		{
			return PlacementState == PlacementStates.PlacementStarted;
		}
	}

	public bool IsPlacementDone
	{
		get
		{
			return PlacementState == PlacementStates.PlacementFinished || PlacementState == PlacementStates.PlacementFailed;
		}
	}

	public void PlacementPending()
	{
		PlacementState = PlacementStates.PlacementPending;
	}

	public void PlacementStarted(string id)
	{
		PlacementState = PlacementStates.PlacementStarted;
		placementID = id;
	}

	public void PlacementFinished()
	{
		PlacementState = PlacementStates.PlacementFinished;
	}

	public void PlacementFailed()
	{
		PlacementState = PlacementStates.PlacementFailed;
	}

	private void Start()
	{
		PlacementPending();
	}

	private void Update()
	{
		if (IsPlacementInProgress && Upsight.isContentReadyForBillboardWithScope(placementID))
		{
			Upsight.prepareBillboard(placementID);
			PlacementFinished();
		}
	}

	public void RecordCustomEvent(string upsightEvent, Dictionary<string, object> properties = null)
	{
		Upsight.recordCustomEvent(upsightEvent, properties);
	}

	public void KPILaboratoryTracking(LaboratoryTrackEvent laboratoryEvent, CreatureItem creature)
	{
		string upsightEvent = "Laboratory." + laboratoryEvent;
		string value = Singleton<PlayerInfoScript>.Instance.SaveData.InventorySpace.ToString();
		string value2 = Singleton<PlayerInfoScript>.Instance.SaveData.FilledInventoryCount.ToString();
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("creatureID", creature.ToString());
		dictionary.Add("level", creature.Level.ToString());
		dictionary.Add("stars", creature.StarRating.ToString());
		dictionary.Add("count", value2);
		dictionary.Add("maxSize", value);
		RecordCustomEvent(upsightEvent, dictionary);
	}

	public void KPIInventoryTrack(InventoryTrackEvent inventoryEvent, string sourceID, string numberItems, InventoryModalAction action = InventoryModalAction.Closed)
	{
		string upsightEvent = "Inventory." + inventoryEvent;
		string value = Singleton<PlayerInfoScript>.Instance.SaveData.InventorySpace.ToString();
		string value2 = Singleton<PlayerInfoScript>.Instance.SaveData.FilledInventoryCount.ToString();
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("maxSize", value);
		dictionary.Add("currentSize", value2);
		switch (inventoryEvent)
		{
		case InventoryTrackEvent.Full:
			dictionary.Add("numberOfItems", numberItems);
			break;
		case InventoryTrackEvent.FullModal:
		{
			string value3 = MiscParams.InventorySpacePurchaseCost.ToString();
			string value4 = Singleton<PlayerInfoScript>.Instance.SaveData.HardCurrency.ToString();
			dictionary.Add("modalAction", action);
			dictionary.Add("expansionCost", value3);
			dictionary.Add("balance", value4);
			break;
		}
		}
		dictionary.Add("sourceID", sourceID);
		RecordCustomEvent(upsightEvent, dictionary);
	}

	public void KPIBattleTrack(BattleTrackProgress progress, BattleTrackEvent trackEvent)
	{
		if (Singleton<PlayerInfoScript>.Instance.StateData.MultiplayerMode)
		{
			return;
		}
		string upsightEvent = "Battle." + progress.ToString() + "." + trackEvent;
		if (progress == BattleTrackProgress.QuestStart)
		{
			GenerateBattleID();
		}
		string iD = Singleton<PlayerInfoScript>.Instance.StateData.CurrentActiveQuest.SetLoadout.ID;
		string value = Singleton<PlayerInfoScript>.Instance.StateData.CurrentActiveQuest.StaminaCost.ToString();
		string value2 = Singleton<PlayerInfoScript>.Instance.RankXpLevelData.mCurrentLevel.ToString();
		string value3 = DetachedSingleton<StaminaManager>.Instance.GetStamina(StaminaType.Quests).ToString();
		string iD2 = Singleton<PlayerInfoScript>.Instance.GetCurrentLoadout().Leader.SelectedSkin.ID;
		string text = string.Empty;
		string value4 = string.Empty;
		int num = 5;
		if (Singleton<PlayerInfoScript>.Instance.StateData.SelectedHelper != null)
		{
			value4 = Singleton<PlayerInfoScript>.Instance.StateData.SelectedHelper.HelperCreature.Creature.ToString();
			num--;
		}
		for (int i = 0; i < num; i++)
		{
			InventorySlotItem inventorySlotItem = Singleton<PlayerInfoScript>.Instance.GetCurrentLoadout().CreatureSet[i];
			if (Singleton<PlayerInfoScript>.Instance.GetCurrentLoadout().CreatureSet[i] != null)
			{
				text += inventorySlotItem.Creature.ToString();
				if (i < num - 1)
				{
					text += ", ";
				}
			}
		}
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("questID", iD);
		dictionary.Add("battleID", battleID);
		dictionary.Add("energyCost", value);
		dictionary.Add("playerRank", value2);
		dictionary.Add("playerEnergy", value3);
		dictionary.Add("hero", iD2);
		dictionary.Add("helperCreature", value4);
		dictionary.Add("creatureList", text);
		RecordCustomEvent(upsightEvent, dictionary);
	}

	private void GenerateBattleID()
	{
		battleID = Singleton<PlayerInfoScript>.Instance.GetFormattedPlayerCode();
		string text = TFUtils.ServerTime.ToString("yyyyMMddHHmmss");
		battleID = battleID + "." + text;
	}

	public void KPIPVPBattleCompleted(PVPWinCondition winCondition, bool win)
	{
		if (!Singleton<PlayerInfoScript>.Instance.StateData.MultiplayerMode)
		{
			return;
		}
		string upsightEvent = "Multiplayer.PvPMatch.MatchCompleted";
		string empty = string.Empty;
		string empty2 = string.Empty;
		if (win)
		{
			empty = Singleton<PlayerInfoScript>.Instance.RankData.ID;
			empty2 = Singleton<PlayerInfoScript>.Instance.PvPData.OpponentID;
		}
		else
		{
			empty = Singleton<PlayerInfoScript>.Instance.PvPData.OpponentID;
			empty2 = Singleton<PlayerInfoScript>.Instance.RankData.ID;
		}
		string matchId = Singleton<PlayerInfoScript>.Instance.PvPData.MatchId;
		string value = Singleton<DWGame>.Instance.turnNumber.ToString();
		string value2 = Singleton<DWGame>.Instance.battleDuration.ToString();
		string value3 = Singleton<PlayerInfoScript>.Instance.StateData.CurrentLoadout.Leader.ToString();
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("winCondition", winCondition.ToString());
		dictionary.Add("victorID", empty);
		dictionary.Add("loserID", empty2);
		dictionary.Add("matchID", matchId);
		dictionary.Add("turns", value);
		dictionary.Add("time", value2);
		int num = 1;
		foreach (InventorySlotItem item in Singleton<PlayerInfoScript>.Instance.StateData.CurrentLoadout.CreatureSet)
		{
			string empty3 = string.Empty;
			if (item != null && item.Creature != null)
			{
				string key = "creature0" + num;
				empty3 = item.Creature.ToString() + ", " + item.Creature.Level + ", " + item.Creature.StarRating;
				dictionary.Add(key, empty3);
				num++;
			}
		}
		dictionary.Add("leaderID", value3);
		Singleton<KFFUpsightVGController>.Instance.RecordCustomEvent(upsightEvent, dictionary);
		if (winCondition == PVPWinCondition.Victory)
		{
			upsightEvent = "Multiplayer.PvPMatch.UserDisconnected";
			dictionary.Clear();
			dictionary.Add("userID", empty2);
			dictionary.Add("matchID", matchId);
			dictionary.Add("turns", value);
			dictionary.Add("time", value2);
			Singleton<KFFUpsightVGController>.Instance.RecordCustomEvent(upsightEvent, dictionary);
		}
		Singleton<DWGame>.Instance.turnNumber = 0;
		Singleton<DWGame>.Instance.battleDuration = 0f;
	}

	private void OnEnable()
	{
		UpsightManager.billboardDidReceivePurchaseEvent += makePurchase;
		UpsightManager.onBillboardAppearEvent += contentWillDisplay;
		UpsightManager.onBillboardDismissEvent += contentDismissed;
	}

	private void OnDisable()
	{
		UpsightManager.billboardDidReceivePurchaseEvent -= makePurchase;
		UpsightManager.onBillboardAppearEvent -= contentWillDisplay;
		UpsightManager.onBillboardDismissEvent -= contentDismissed;
	}

	private void purchaseFailed(string error)
	{
		if (IsPlacementInProgress)
		{
		}
		PlacementFinished();
	}

	private void purchaseCancelled(string msg)
	{
		if (IsPlacementInProgress)
		{
		}
		PlacementFinished();
	}

	private void openRequestFailed(string error)
	{
		PlacementFailed();
	}

	private void contentRequestLoaded(string placementID)
	{
	}

	private void contentRequestFailed(string placementID, string error)
	{
		PlacementFailed();
	}

	private void contentWillDisplay(string placementID)
	{
		PlacementStarted(placementID);
	}

	private void contentDismissed(string dismissType)
	{
		switch (dismissType)
		{
		}
		PlacementFinished();
	}

	private void makePurchase(UpsightPurchase purchase)
	{
		purchaseID = purchase.productIdentifier;
		Singleton<PurchaseManager>.Instance.PurchaseProduct(purchaseID, PurchaseCallback);
	}

	private void PurchaseCallback(PurchaseManager.ProductPurchaseResult result)
	{
		switch (result)
		{
		case PurchaseManager.ProductPurchaseResult.Success:
			CompletePurchase();
			break;
		case PurchaseManager.ProductPurchaseResult.Failed:
			PlacementFailed();
			break;
		}
	}

	private void CompletePurchase()
	{
		VirtualGoods data = VirtualGoodsDataManager.Instance.GetData(purchaseID);
		if (data != null)
		{
			StartCoroutine(RedeemVirtualGoods(data));
		}
		else
		{
			PlacementFailed();
		}
	}

	public void DebugRedeemVirtualGoods()
	{
		StartCoroutine(DebugDebugRedeemVirtualGoodsHelper());
	}

	private IEnumerator DebugDebugRedeemVirtualGoodsHelper()
	{
		List<VirtualGoods> purchaseIds = VirtualGoodsDataManager.Instance.GetDatabase();
		if (purchaseIds != null && purchaseIds.Count != 0)
		{
			if (mDebugPurchaseIdIndex >= purchaseIds.Count)
			{
				mDebugPurchaseIdIndex = 0;
			}
			string purchaseID = purchaseIds[mDebugPurchaseIdIndex++].ID;
			VirtualGoods vg = VirtualGoodsDataManager.Instance.GetData(purchaseID);
			if (vg != null)
			{
				yield return new WaitForSeconds(0.5f);
				yield return StartCoroutine(RedeemVirtualGoods(vg));
			}
			yield return null;
		}
	}

	private IEnumerator RedeemVirtualGoods(VirtualGoods vg)
	{
		yield return StartCoroutine(vg.Redeem());
		PlacementFinished();
	}
}
