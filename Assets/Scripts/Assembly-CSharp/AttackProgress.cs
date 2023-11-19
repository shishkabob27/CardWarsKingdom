using System.Collections.Generic;

public class AttackProgress
{
	public static List<AttackProgress> Attacks = new List<AttackProgress>();

	public PlayerType UsingPlayer { get; set; }

	public int TargetIndex { get; set; }

	public CreatureState Attacker { get; set; }

	public AttackCause Cause { get; set; }

	public int AttacksTaken { get; set; }

	public int NumAttacks { get; set; }

	public static bool AttacksInProgress
	{
		get
		{
			return Attacks.Count > 0;
		}
	}
}
