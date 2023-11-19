using System;
using System.Collections.Generic;

[Serializable]
public class GameStateData
{
	public QuestData CurrentActiveQuest;

	public bool DungeonMapQuest;

	public Loadout CurrentLoadout;

	public TutorialState ActiveTutorialState;

	public TutorialState ActiveConditionalState;

	public bool MultiplayerMode;

	public int[] BadgeCounts = new int[4];

	public HelperItem SelectedHelper;

	public List<string> UsedHelperList;

	public QuestBonusType ActiveQuestBonus;

	public Dictionary<string, object> MatchHistoryInvitesSent = new Dictionary<string, object>();

	public QuestData QuestWinToBroadcast;

	public bool PerfectRandomDungeonRun;

	public bool NoReviveRandomDungeonRun;

	public string tutorialBoardID;

	public InventorySlotItem HelperCreature
	{
		get
		{
			return (SelectedHelper == null) ? null : SelectedHelper.HelperCreature;
		}
	}

	public int HelperSlot
	{
		get
		{
			return (SelectedHelper == null) ? (-1) : 5;
		}
	}
}
