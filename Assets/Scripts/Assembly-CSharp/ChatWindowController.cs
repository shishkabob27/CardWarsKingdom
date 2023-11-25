using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatWindowController : MonoBehaviour
{
	private enum ConnectionStatus
	{
		NotStarted,
		Connecting,
		Connected
	}

	public class ReceivedMessage
	{
		public string PlayerName;

		public string Message;

		public ChatMetaData MetaData;

		public CreatureData GachaCreature;

		public QuestData Dungeon;
	}

	private const int MESSAGE_BUFFER = 500;

	public float TextPosWhenChatDisabled;

	public GameObject ChatEntryPrefab;

	public UITweenController ExpandTween;

	public UITweenController CollapseTween;

	public UITweenController FadeTextTween;

	public Transform CollapsedChatEntryParent;

	public Transform ExpandedChatEntryParent;

	public UIPanel CollapsedChatPanel;

	public UIPanel ExpandedChatPanel;

	public Transform ExpandedChatPanelParent;

	public UILabel InputText;

	public UIInput InputObject;

	public UILabel EntryBarLabel;

	public BoxCollider EntryBarCollider;

	public GameObject ChatWindow;

	public Transform PrefabPoolParent;

	public GameObject ExpandedBackgroundCollider;

	public GameObject EntryBar;

	public UISprite ChatIcon;

	public UISprite WindowOutline;

	public UISprite WindowFill;

	public UISprite MessageArea;

	public float SnapSpeed;

	public float TopOffset;

	private static ConnectionStatus mStatus = ConnectionStatus.NotStarted;

	private static ReceivedMessage[] mReceivedMessages = new ReceivedMessage[500];

	private static int mFirstMessageIndex = -1;

	private static bool mMessageBufferFilled = false;

	private static float[] mGachaMessageCooldowns = new float[2];

	private bool mExpanded;

	private List<ChatEntryScript> mEntryPool;

	private ChatEntryScript mDummyEntry;

	public float mThrottlePoints;

	public float mThrottleMultiplier = 1f;

	private string mLastStringSent;

	private bool mDisabled;

	private bool mIntialized;

	private static bool mChatDisabled = false;

	private static bool mInBlacklistedCountry = false;

	private static float mReconnectInterval = 1f;

	private static float mNextRetryTimeout = -1f;

	private static ChatWindowController mInstance = null;

	public bool Expanded
	{
		get
		{
			return mExpanded;
		}
	}

	public static ChatWindowController Instance
	{
		get
		{
			return mInstance;
		}
	}

	private void Awake()
	{
		mInstance = this;
		mStatus = ConnectionStatus.NotStarted;
		mDummyEntry = PrefabPoolParent.InstantiateAsChild(ChatEntryPrefab).GetComponent<ChatEntryScript>();
		mDummyEntry.gameObject.SetActive(false);
		mDummyEntry.gameObject.name = "DummyEntry";
		mEntryPool = new List<ChatEntryScript>();
		ChatEntryScript component;
		for (float num = 0f; num <= ExpandedChatPanel.GetViewSize().y; num += (float)component.Label.height)
		{
			component = PrefabPoolParent.InstantiateAsChild(ChatEntryPrefab).GetComponent<ChatEntryScript>();
			component.gameObject.SetActive(false);
			mEntryPool.Add(component);
		}
		StartCoroutine(SetScheduledDisplayElementsCo());
	}

	public IEnumerator SetScheduledDisplayElementsCo()
	{
		TownScheduleData townScheduleData = null;
		while (townScheduleData == null)
		{
			townScheduleData = TownScheduleDataManager.Instance.GetCurrentScheduledTownData();
			yield return new WaitForEndOfFrame();
		}
		ChatIcon.spriteName = townScheduleData.ChatIconSprite;
		WindowOutline.color = townScheduleData.ChatWindowFrameColor;
		WindowFill.color = townScheduleData.ChatWindowFillColor;
		MessageArea.spriteName = townScheduleData.ChatMessageBgSprite;
	}

	private ReceivedMessage TestMessage(int i)
	{
		ReceivedMessage receivedMessage = new ReceivedMessage();
		receivedMessage.MetaData = new ChatMetaData();
		receivedMessage.MetaData.CountryCode = "US";
		receivedMessage.MetaData.FacebookId = "0000000000000000";
		receivedMessage.MetaData.Leader = "Leader_Finn";
		receivedMessage.MetaData.Name = "PlayerNameHere";
		receivedMessage.MetaData.PortraitId = "Default";
		receivedMessage.MetaData.UserId = "123_4567890";
		receivedMessage.PlayerName = "PlayerNameHere";
		receivedMessage.Message = i.ToString();
		int num = Random.Range(5, 200);
		for (int j = 0; j < num; j++)
		{
			receivedMessage.Message += " i";
		}
		return receivedMessage;
	}

	private ChatEntryScript GetChatEntry(bool expanded)
	{
		if (mEntryPool.Count == 0)
		{
			ChatEntryScript component = PrefabPoolParent.InstantiateAsChild(ChatEntryPrefab).GetComponent<ChatEntryScript>();
			component.transform.parent = ((!expanded) ? CollapsedChatEntryParent : ExpandedChatEntryParent);
			component.gameObject.SetActive(true);
			return component;
		}
		ChatEntryScript chatEntryScript = mEntryPool[mEntryPool.Count - 1];
		mEntryPool.RemoveAt(mEntryPool.Count - 1);
		chatEntryScript.transform.parent = ((!expanded) ? CollapsedChatEntryParent : ExpandedChatEntryParent);
		chatEntryScript.gameObject.SetActive(true);
		return chatEntryScript;
	}

	private void ReleaseChatEntry(ChatEntryScript entry)
	{
		entry.gameObject.SetActive(false);
		entry.transform.parent = PrefabPoolParent;
		mEntryPool.Add(entry);
	}

	private void Update()
	{
		if (!Singleton<PlayerInfoScript>.Instance.IsInitialized)
		{
			return;
		}
		if (!mIntialized)
		{
			mDisabled = !Singleton<PlayerInfoScript>.Instance.IsPastAgeGate(MiscParams.chatAgeGate) || !Singleton<TutorialController>.Instance.IsFTUETutorialComplete();
			string chatSwitch = SessionManager.Instance.theSession.ChatSwitch;
			if (chatSwitch == "0" || MiscParams.DisableChat)
			{
				mChatDisabled = true;
			}
			string myCountryCode = Singleton<CountryFlagManager>.Instance.GetMyCountryCode();
			mInBlacklistedCountry = ChatCountryBlacklistDataManager.Instance.IsCountryBlacklisted(myCountryCode);
			mIntialized = true;
		}
		if (mDisabled)
		{
			ChatWindow.SetActive(false);
		}
		if (LoadingScreenController.ShowingLoadingScreen())
		{
			return;
		}
		if (mStatus == ConnectionStatus.NotStarted && !mDisabled && !string.IsNullOrEmpty(Singleton<PlayerInfoScript>.Instance.SaveData.MultiplayerPlayerName))
		{
			mNextRetryTimeout -= Time.deltaTime;
			if (mNextRetryTimeout < 0f)
			{
				mStatus = ConnectionStatus.Connecting;
				Singleton<ChatManager>.Instance.StartChat(Singleton<TBPvPManager>.Instance.CountryCode, Singleton<PlayerInfoScript>.Instance.SaveData.MultiplayerPlayerName, Singleton<PlayerInfoScript>.Instance.GetPlayerCode(), ChatCallback);
			}
		}
		if (!SessionManager.Instance.IsLoadDataDone() || !Singleton<TownController>.Instance.IsIntroDone())
		{
			return;
		}
		bool flag = mThrottlePoints > 1f;
		mThrottlePoints -= Time.deltaTime * MiscParams.ThrottlePointDecay;
		if (mThrottlePoints < 0f)
		{
			mThrottlePoints = 0f;
		}
		mThrottleMultiplier -= Time.deltaTime * MiscParams.ThrottleMultiplierDecay;
		if (mThrottleMultiplier < 1f)
		{
			mThrottleMultiplier = 1f;
		}
		bool flag2 = mThrottlePoints > 1f;
		if (flag && !flag2)
		{
			SendChatMessage();
		}
		if (EntryBarLabel.gameObject.activeInHierarchy)
		{
			if (mStatus == ConnectionStatus.Connected)
			{
				if (mChatDisabled)
				{
					EntryBarCollider.enabled = false;
					EntryBarLabel.text = string.Empty;
				}
				else if (flag2)
				{
					EntryBarCollider.enabled = false;
					EntryBarLabel.text = KFFLocalization.Get("!!SENDING_MESSAGE");
				}
				else
				{
					EntryBarCollider.enabled = true;
					EntryBarLabel.text = KFFLocalization.Get("!!ENTER_A_MESSAGE");
				}
			}
			else
			{
				EntryBarCollider.enabled = false;
				EntryBarLabel.text = KFFLocalization.Get("!!0_CONNECTING");
			}
		}
		for (int i = 0; i < mGachaMessageCooldowns.Length; i++)
		{
			mGachaMessageCooldowns[i] -= Time.deltaTime;
			if (mGachaMessageCooldowns[i] < 0f)
			{
				mGachaMessageCooldowns[i] = 0f;
			}
		}
		UpdateTextLines();
	}

	private static void ChatCallback(ChatManager.chatEventCode eventCode, object obj)
	{
		switch (eventCode)
		{
		case ChatManager.chatEventCode.Connected:
			mStatus = ConnectionStatus.Connected;
			Singleton<ChatManager>.Instance.SelectChannel();
			if (mFirstMessageIndex == -1)
			{
				if (mChatDisabled)
				{
					ReceiveMessage(null, KFFLocalization.Get("!!ONLINE"), null);
				}
				else
				{
					ReceiveMessage(null, KFFLocalization.Get("!!CHAT_WELCOME_MESSAGE"), null);
				}
			}
			if (DetachedSingleton<SceneFlowManager>.Instance.InFrontEnd())
			{
				Instance.PopulateFromStoredList();
			}
			mReconnectInterval = 1f;
			mNextRetryTimeout = -1f;
			break;
		case ChatManager.chatEventCode.Disconnected:
			mStatus = ConnectionStatus.NotStarted;
			mReconnectInterval *= 2f;
			if (mReconnectInterval > 64f)
			{
				mReconnectInterval = 64f;
			}
			mNextRetryTimeout = mReconnectInterval;
			break;
		case ChatManager.chatEventCode.IncomingMessages:
		{
			ChatManager.IncomingMessageObject incomingMessageObject2 = obj as ChatManager.IncomingMessageObject;
			if (!string.IsNullOrEmpty(incomingMessageObject2.MetaData.GachaCreature))
			{
				ReceiveGachaMessage(incomingMessageObject2.PlayerName, incomingMessageObject2.MetaData.GachaCreature);
			}
			if (!string.IsNullOrEmpty(incomingMessageObject2.MetaData.Dungeon))
			{
				ReceiveDungeonMessage(incomingMessageObject2.PlayerName, incomingMessageObject2.MetaData.Dungeon, incomingMessageObject2.MetaData);
			}
			else if (!string.IsNullOrEmpty(incomingMessageObject2.Message) && !mChatDisabled)
			{
				ReceiveMessage(incomingMessageObject2.PlayerName, incomingMessageObject2.Message, incomingMessageObject2.MetaData);
			}
			break;
		}
		case ChatManager.chatEventCode.IncomingPrivateMessage:
		{
			ChatManager.IncomingMessageObject incomingMessageObject = obj as ChatManager.IncomingMessageObject;
			Singleton<MultiplayerMessageHandler>.Instance.ReceiveChatBasedMessage(incomingMessageObject.MetaData.InviteData);
			break;
		}
		}
	}

	public static void FakeGachaMessage()
	{
		List<CreatureData> list = CreatureDataManager.Instance.GetDatabase().FindAll((CreatureData m) => m.Rarity >= 1);
		int index = Random.Range(0, list.Count);
		ReceiveGachaMessage("Fake Player", list[index].ID);
	}

	public static void FakeChatMessage()
	{
		ReceivedMessage receivedMessage = Instance.TestMessage(0);
		ReceiveMessage(receivedMessage.PlayerName, receivedMessage.Message, receivedMessage.MetaData);
	}

	private static void ReceiveMessage(string playerName, string message, ChatMetaData metaData)
	{
		if ((!mInBlacklistedCountry || playerName == null) && (metaData == null || !Singleton<PlayerInfoScript>.Instance.SaveData.IgnoredPlayers.ContainsKey(metaData.UserId)))
		{
			message = ProfanityFilterDataManager.Instance.ReplaceProfanity(message);
			ReceivedMessage receivedMessage = new ReceivedMessage();
			receivedMessage.PlayerName = playerName;
			receivedMessage.Message = message;
			receivedMessage.MetaData = metaData;
			AddMessage(receivedMessage);
		}
	}

	private static void ReceiveGachaMessage(string playerName, string creatureId)
	{
		CreatureData data = CreatureDataManager.Instance.GetData(creatureId);
		if (data != null)
		{
			ReceivedMessage receivedMessage = new ReceivedMessage();
			receivedMessage.PlayerName = playerName;
			receivedMessage.GachaCreature = data;
			AddMessage(receivedMessage);
		}
	}

	private static void ReceiveDungeonMessage(string playerName, string dungeonId, ChatMetaData metaData)
	{
		QuestData data = QuestDataManager.Instance.GetData(dungeonId);
		if (data != null)
		{
			ReceivedMessage receivedMessage = new ReceivedMessage();
			receivedMessage.PlayerName = playerName;
			receivedMessage.Dungeon = data;
			receivedMessage.MetaData = metaData;
			AddMessage(receivedMessage);
		}
	}

	private static void AddMessage(ReceivedMessage message)
	{
		mFirstMessageIndex++;
		if (mFirstMessageIndex >= mReceivedMessages.Length)
		{
			mMessageBufferFilled = true;
			mFirstMessageIndex = 0;
		}
		mReceivedMessages[mFirstMessageIndex] = message;
		if (mInstance != null && !mInstance.FadeTextTween.AnyTweenPlaying())
		{
			mInstance.CreateMessageAtBottom();
		}
	}

	public void OnClickWindow()
	{
		if (Singleton<TownController>.Instance.IsIntroDone() && mStatus == ConnectionStatus.Connected)
		{
			if (mExpanded)
			{
				Singleton<SLOTAudioManager>.Instance.PlaySound("ui/SFX_ChatHide");
				CollapseTween.Play();
				ExpandedBackgroundCollider.SetActive(false);
			}
			else
			{
				ExpandedBackgroundCollider.SetActive(true);
				Singleton<SLOTAudioManager>.Instance.PlaySound("ui/SFX_ChatShow");
				ExpandTween.Play();
				Singleton<TownNotificationController>.Instance.HidePopup();
			}
			EntryBar.SetActive(!mInBlacklistedCountry);
			if (!EntryBar.activeSelf)
			{
				Vector3 localPosition = ExpandedChatPanelParent.localPosition;
				localPosition.y = TextPosWhenChatDisabled;
				ExpandedChatPanelParent.localPosition = localPosition;
			}
			mExpanded = !mExpanded;
			FadeTextTween.PlayWithCallback(OnTextFadedOut);
		}
	}

	private void OnTextFadedOut()
	{
		PopulateFromStoredList();
	}

	private void PopulateFromStoredList()
	{
		while (CollapsedChatEntryParent.childCount > 0)
		{
			ReleaseChatEntry(CollapsedChatEntryParent.GetChild(0).GetComponent<ChatEntryScript>());
		}
		while (ExpandedChatEntryParent.childCount > 0)
		{
			ReleaseChatEntry(ExpandedChatEntryParent.GetChild(0).GetComponent<ChatEntryScript>());
		}
		ExpandedChatPanel.clipOffset = Vector2.zero;
		ExpandedChatPanel.transform.localPosition = Vector3.zero;
		UIScrollView component = ExpandedChatPanel.transform.GetComponent<UIScrollView>();
		component.UpdateScrollbars();
		float num = ((!mExpanded) ? CollapsedChatPanel.GetViewSize().y : ExpandedChatPanel.GetViewSize().y);
		int num2 = mFirstMessageIndex;
		float num3 = 0f;
		while (true)
		{
			ChatEntryScript chatEntry = GetChatEntry(mExpanded);
			chatEntry.Populate(mReceivedMessages[num2], num2, mExpanded);
			chatEntry.SetPosition(num3);
			num3 += (float)chatEntry.Label.height;
			if (num3 > num)
			{
				break;
			}
			num2--;
			if (num2 < 0)
			{
				num2 = mReceivedMessages.Length - 1;
			}
		}
	}

	private void UpdateTextLines()
	{
		Transform transform = ((!mExpanded) ? CollapsedChatEntryParent : ExpandedChatEntryParent);
		if (transform.childCount == 0)
		{
			return;
		}
		UIPanel uIPanel = ((!mExpanded) ? CollapsedChatPanel : ExpandedChatPanel);
		float num = ((!mExpanded) ? 0f : ExpandedChatPanel.clipOffset.y);
		float num2 = uIPanel.GetViewSize().y - TopOffset;
		Transform transform2 = null;
		Transform transform3 = null;
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			if (transform2 == null || child.localPosition.y < transform2.localPosition.y)
			{
				transform2 = child;
			}
			if (transform3 == null || child.localPosition.y > transform3.localPosition.y)
			{
				transform3 = child;
			}
		}
		ChatEntryScript component = transform3.GetComponent<ChatEntryScript>();
		if (mExpanded)
		{
			UIScrollView component2 = uIPanel.GetComponent<UIScrollView>();
			component2.enabled = component.Label.text != string.Empty;
		}
		float num3 = 0f;
		float num4 = 0f;
		float num5 = transform3.localPosition.y - num;
		if (num5 > num2)
		{
			ReleaseChatEntry(component);
		}
		else if (num5 + (float)component.Label.height < num2)
		{
			int num6 = component.MessageIndex - 1;
			if (num6 < 0)
			{
				num6 = ((!mMessageBufferFilled) ? mFirstMessageIndex : 499);
			}
			if (num6 == mFirstMessageIndex)
			{
				num4 = num5 + (float)component.Label.height - num2;
			}
			else
			{
				ChatEntryScript chatEntry = GetChatEntry(mExpanded);
				chatEntry.Populate(mReceivedMessages[num6], num6, mExpanded);
				Vector3 localPosition = transform3.localPosition;
				localPosition.y += component.Label.height;
				chatEntry.transform.localPosition = localPosition;
				chatEntry.TargetPos = component.TargetPos + (float)component.Label.height;
			}
		}
		ChatEntryScript component3 = transform2.GetComponent<ChatEntryScript>();
		float num7 = transform2.localPosition.y - num;
		if (num7 < (float)(-component3.Label.height))
		{
			ReleaseChatEntry(component3);
		}
		else if (num7 > 0f)
		{
			if (component3.MessageIndex == mFirstMessageIndex)
			{
				num3 = num7;
			}
			else
			{
				int num8 = component3.MessageIndex + 1;
				if (num8 >= 500)
				{
					num8 = 0;
				}
				ChatEntryScript chatEntry2 = GetChatEntry(mExpanded);
				chatEntry2.Populate(mReceivedMessages[num8], num8, mExpanded);
				Vector3 localPosition2 = transform2.localPosition;
				localPosition2.y -= chatEntry2.Label.height;
				chatEntry2.transform.localPosition = localPosition2;
				chatEntry2.TargetPos = component3.TargetPos - (float)chatEntry2.Label.height;
			}
		}
		float num9 = ((!(num3 > 0f)) ? num4 : num3);
		if (num9 != 0f)
		{
			Vector2 clipOffset = uIPanel.clipOffset;
			clipOffset.y = Mathf.Lerp(clipOffset.y, clipOffset.y + num9, Time.deltaTime * SnapSpeed);
			uIPanel.clipOffset = clipOffset;
			Vector3 localPosition3 = uIPanel.transform.localPosition;
			localPosition3.y = Mathf.Lerp(localPosition3.y, localPosition3.y - num9, Time.deltaTime * SnapSpeed);
			uIPanel.transform.localPosition = localPosition3;
		}
	}

	private void CreateMessageAtBottom()
	{
		Transform transform = ((!mExpanded) ? CollapsedChatEntryParent : ExpandedChatEntryParent);
		for (int i = 0; i < transform.childCount; i++)
		{
			ChatEntryScript component = transform.GetChild(i).GetComponent<ChatEntryScript>();
			if (component.MessageIndex == mFirstMessageIndex)
			{
				ReleaseChatEntry(component);
				break;
			}
		}
		mDummyEntry.Populate(mReceivedMessages[mFirstMessageIndex], mFirstMessageIndex, mExpanded);
		for (int j = 0; j < transform.childCount; j++)
		{
			transform.GetChild(j).GetComponent<ChatEntryScript>().TargetPos += mDummyEntry.Label.height;
		}
	}

	public void OnSubmitText()
	{
		if (mStatus == ConnectionStatus.Connected)
		{
			float num = 1f;
			if (mLastStringSent == InputText.text)
			{
				num = MiscParams.ThrottleDupeMultiplier;
			}
			mThrottlePoints += MiscParams.ThrottlePointsPerMessage * mThrottleMultiplier * num;
			mThrottleMultiplier += MiscParams.ThrottleMultiplierPerMessage * num;
			mLastStringSent = InputText.text;
			if (mThrottlePoints <= 1f)
			{
				SendChatMessage();
			}
		}
		InputObject.value = null;
		InputText.text = string.Empty;
	}

	private void SendChatMessage()
	{
		if (!mChatDisabled)
		{
			string inputLine = mLastStringSent;
			if (!Singleton<ChatManager>.Instance.SendLine(inputLine))
			{
				Singleton<SimplePopupController>.Instance.ShowMessage(KFFLocalization.Get("!!GAME_ERROR_CONTACTING"), KFFLocalization.Get("!!CHAT_UNDER_MAINTENANCE"), true);
			}
		}
	}

	public void PurgeBlockedUser(string userId)
	{
		List<ReceivedMessage> list = new List<ReceivedMessage>();
		int num = mFirstMessageIndex;
		while (mReceivedMessages[num] != null)
		{
			if (mReceivedMessages[num].MetaData == null || mReceivedMessages[num].MetaData.UserId != userId)
			{
				list.Add(mReceivedMessages[num]);
			}
			num--;
			if (num < 0)
			{
				num = mReceivedMessages.Length - 1;
			}
			if (num == mFirstMessageIndex)
			{
				break;
			}
		}
		mReceivedMessages = new ReceivedMessage[500];
		list.Reverse();
		mFirstMessageIndex = -1;
		foreach (ReceivedMessage item in list)
		{
			mFirstMessageIndex++;
			mReceivedMessages[mFirstMessageIndex] = item;
		}
		PopulateFromStoredList();
	}

	public static void SendGachaAnnouncementIfRare(CreatureItem creature)
	{
		if (creature.StarRating < MiscParams.GachaChatAnnounceRarity)
		{
			return;
		}
		for (int i = 0; i < mGachaMessageCooldowns.Length; i++)
		{
			if (mGachaMessageCooldowns[i] == 0f)
			{
				mGachaMessageCooldowns[i] = MiscParams.GachaChatAnnounceCooldown;
				break;
			}
			if (i == mGachaMessageCooldowns.Length - 1)
			{
				return;
			}
		}
		Singleton<ChatManager>.Instance.SendGachaCreatureAnnouncement(creature.Form.ID);
	}

	public static void SendDungeonAnnouncement(QuestData quest)
	{
		Singleton<ChatManager>.Instance.SendDungeonAnnouncement(quest.ID);
	}
}
