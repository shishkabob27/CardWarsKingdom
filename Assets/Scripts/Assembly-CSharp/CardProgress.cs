using System.Collections.Generic;

public class CardProgress
{
	private static CardProgress _instance;

	public PlayerType UsingPlayer { get; set; }

	public PlayerType TargetPlayer { get; set; }

	public CardData Card { get; set; }

	public int LaneIndex { get; set; }

	public CardState State { get; set; }

	public List<CreatureState> Attackers { get; set; }

	public List<CreatureState> Targets { get; set; }

	public int AttackerIdx { get; set; }

	public int TargetIdx { get; set; }

	public int AttacksTaken { get; set; }

	public int NumAttacks { get; set; }

	public static CardProgress Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new CardProgress();
			}
			return _instance;
		}
	}

	public CardProgress()
	{
		State = CardState.Idle;
	}
}
