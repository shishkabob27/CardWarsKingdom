using UnityEngine;

public class BattleLogController : Singleton<BattleLogController>
{
	public int MaxEntries;
	public GameObject LogEntryPrefab;
	public UIGrid LogEntryGrid;
}
