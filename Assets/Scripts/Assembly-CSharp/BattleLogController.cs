using System.Collections;
using UnityEngine;

public class BattleLogController : Singleton<BattleLogController>
{
	public int MaxEntries;

	public GameObject LogEntryPrefab;

	public UIGrid LogEntryGrid;

	private int mLogIndex = 1;

	public bool CanShowDragAttack { get; set; }

	public IEnumerator LogCardPlay(CardData card, CreatureState target)
	{
		BattleLogEntry logEntry = AddEntry();
		logEntry.PopulateCardPlay(card, target);
		yield break;
	}

	public IEnumerator LogCreaturePlay(CreatureItem creature)
	{
		BattleLogEntry logEntry = AddEntry();
		logEntry.PopulateCreaturePlay(creature);
		yield break;
	}

	public IEnumerator LogDragAttack(CreatureState creature, CreatureState target)
	{
		if (target != null)
		{
			CanShowDragAttack = false;
			while (!CanShowDragAttack)
			{
				yield return null;
			}
		}
		BattleLogEntry logEntry = AddEntry();
		logEntry.PopulateDragAttack(creature, target);
	}

	private BattleLogEntry AddEntry()
	{
		BattleLogEntry component = LogEntryGrid.gameObject.InstantiateAsChild(LogEntryPrefab).GetComponent<BattleLogEntry>();
		component.name = (1000000 - mLogIndex).ToString("D6") + "_" + component.name;
		mLogIndex++;
		LogEntryGrid.Reposition();
		if (LogEntryGrid.transform.childCount > MaxEntries)
		{
			LogEntryGrid.transform.GetChild(0).GetComponent<BattleLogEntry>().FadeOut();
		}
		return component;
	}
}
