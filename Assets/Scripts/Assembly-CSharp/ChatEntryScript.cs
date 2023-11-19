using UnityEngine;

public class ChatEntryScript : MonoBehaviour
{
	public float MoveSpeed;

	public float EntryCutoffPosition;

	public float FullscreenEntryCutoffPosition;

	public Color PlayerNameColor;

	public Color GameMessageColor;

	public BoxCollider TapCollider;

	public TweenColor GachaColorTween;

	public Color RedClassColor;

	public Color GreenClassColor;

	public Color BlueClassColor;

	public Color DarkClassColor;

	public Color LightClassColor;

	public Color RedClassColorTo;

	public Color GreenClassColorTo;

	public Color BlueClassColorTo;

	public Color DarkClassColorTo;

	public Color LightClassColorTo;

	public Color DungeonColor;

	public Color DungeonColorTo;

	public UITweenController AppearTween;

	public UILabel Label;

	private bool mExpanded;

	private ChatMetaData mMetaData;

	private CreatureData mGachaCreature;

	private QuestData mDungeon;

	private int mGachaCreatureColorStartPos;

	private int mBaseLabelHeight;

	public float TargetPos { get; set; }

	public int MessageIndex { get; private set; }

	private void Awake()
	{
		mBaseLabelHeight = Label.height;
	}

	public void Populate(ChatWindowController.ReceivedMessage message, int messageIndex, bool expanded)
	{
		MessageIndex = messageIndex;
		Label.height = mBaseLabelHeight;
		Label.transform.localPosition = Vector3.zero;
		mExpanded = expanded;
		if (mExpanded)
		{
			Label.maxLineCount = 0;
			Label.overflowMethod = UILabel.Overflow.ResizeHeight;
		}
		else
		{
			Label.maxLineCount = 1;
			Label.overflowMethod = UILabel.Overflow.ClampContent;
		}
		GachaColorTween.enabled = false;
		mGachaCreatureColorStartPos = -1;
		if (message == null)
		{
			Label.text = string.Empty;
			TapCollider.enabled = false;
			return;
		}
		mMetaData = message.MetaData;
		mGachaCreature = message.GachaCreature;
		mDungeon = message.Dungeon;
		if (message.GachaCreature != null)
		{
			if (message.GachaCreature.Faction == CreatureFaction.Red)
			{
				GachaColorTween.from = RedClassColor;
				GachaColorTween.to = RedClassColorTo;
			}
			else if (message.GachaCreature.Faction == CreatureFaction.Green)
			{
				GachaColorTween.from = GreenClassColor;
				GachaColorTween.to = GreenClassColorTo;
			}
			else if (message.GachaCreature.Faction == CreatureFaction.Blue)
			{
				GachaColorTween.from = BlueClassColor;
				GachaColorTween.to = BlueClassColorTo;
			}
			else if (message.GachaCreature.Faction == CreatureFaction.Dark)
			{
				GachaColorTween.from = DarkClassColor;
				GachaColorTween.to = DarkClassColorTo;
			}
			else if (message.GachaCreature.Faction == CreatureFaction.Light)
			{
				GachaColorTween.from = LightClassColor;
				GachaColorTween.to = LightClassColorTo;
			}
			string text = "[" + GameMessageColor.ToHexString() + "]";
			string text2 = "[" + GachaColorTween.from.ToHexString() + "]" + message.GachaCreature.Name + text;
			Label.color = Color.white;
			Label.text = text + string.Format(KFFLocalization.Get("!!CHAT_GACHA_MESSAGE"), message.PlayerName, text2);
			mGachaCreatureColorStartPos = Label.text.IndexOf(text2) + 1;
			GachaColorTween.enabled = true;
			GachaColorTween.tweenFactor = Random.Range(0f, 1f);
		}
		else if (message.Dungeon != null)
		{
			GachaColorTween.from = DungeonColor;
			GachaColorTween.to = DungeonColorTo;
			string text3 = "[" + GameMessageColor.ToHexString() + "]";
			string text4 = "[" + GachaColorTween.from.ToHexString() + "]" + message.Dungeon.LevelName + text3;
			Label.color = Color.white;
			Label.text = text3 + string.Format(KFFLocalization.Get("!!CHAT_DUNGEON_MESSAGE"), message.PlayerName, text4);
			mGachaCreatureColorStartPos = Label.text.IndexOf(text4) + 1;
			GachaColorTween.enabled = true;
			GachaColorTween.tweenFactor = Random.Range(0f, 1f);
		}
		else if (message.PlayerName != null)
		{
			string text5 = "[FFFFFF]";
			Label.color = Color.white;
			Label.text = "[" + PlayerNameColor.ToHexString() + "]" + message.PlayerName + text5 + ": " + message.Message;
		}
		else
		{
			Label.color = GameMessageColor;
			Label.text = message.Message;
		}
		if (mExpanded)
		{
			TapCollider.enabled = true;
			TapCollider.size = Label.printedSize;
			TapCollider.transform.localPosition = TapCollider.size / 2f;
		}
		else
		{
			TapCollider.enabled = false;
		}
	}

	public void ShowPlayerInfo()
	{
		if (mGachaCreature != null)
		{
			Singleton<CreatureDetailsController>.Instance.ShowCollectionCreature(mGachaCreature, null);
		}
		else if (mMetaData != null)
		{
			Singleton<OtherPlayerInfoController>.Instance.Show(mMetaData);
		}
	}

	private void Update()
	{
		Vector3 localPosition = base.transform.localPosition;
		localPosition.y = Mathf.Lerp(base.transform.localPosition.y, TargetPos, Time.deltaTime * MoveSpeed);
		base.transform.localPosition = localPosition;
		if (mGachaCreatureColorStartPos != -1)
		{
			string value = GachaColorTween.value.ToHexString();
			Label.text = Label.text.Remove(mGachaCreatureColorStartPos, 6).Insert(mGachaCreatureColorStartPos, value);
		}
	}

	public void SetPosition(float pos)
	{
		Vector3 localPosition = base.transform.localPosition;
		localPosition.x = 0f;
		localPosition.y = pos;
		base.transform.localPosition = localPosition;
		TargetPos = pos;
	}
}
