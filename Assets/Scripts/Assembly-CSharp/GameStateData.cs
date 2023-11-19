using System;
using System.Collections.Generic;

[Serializable]
public class GameStateData
{
	public bool DungeonMapQuest;
	public bool MultiplayerMode;
	public int[] BadgeCounts;
	public List<string> UsedHelperList;
	public QuestBonusType ActiveQuestBonus;
	public bool PerfectRandomDungeonRun;
	public bool NoReviveRandomDungeonRun;
	public string tutorialBoardID;
}
