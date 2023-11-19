using System.Collections.Generic;

public class DWGame : Singleton<DWGame>
{
	public enum TurnActions
	{
		PlayCard = 0,
		PlayMonster = 1,
		Attack = 2,
	}

	public bool IsTutorialSetup;
	public int turnNumber;
	public float battleDuration;
	public bool battleStarted;
	public bool turnStarted;
	public float turnDuration;
	public List<DWGame.TurnActions> turnActions;
}
