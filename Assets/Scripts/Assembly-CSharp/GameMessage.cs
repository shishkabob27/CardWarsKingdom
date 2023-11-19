using System.Collections.Generic;

public class GameMessage
{
	public List<GameMessage> Children = new List<GameMessage>();

	public GameMessage Parent { get; set; }

	public GameEvent Action { get; set; }

	public PlayerState WhichPlayer { get; set; }

	public LaneState Lane { get; set; }

	public LaneState SecondTarget { get; set; }

	public CreatureState Creature { get; set; }

	public CreatureState SecondCreature { get; set; }

	public CardData Card { get; set; }

	public float RawAmount { get; set; }

	public float Amount { get; set; }

	public float AmountChange { get; set; }

	public bool IsCritical { get; set; }

	public bool IsMiss { get; set; }

	public bool IsShield { get; set; }

	public bool PhysicalDamage { get; set; }

	public bool MagicDamage { get; set; }

	public bool Presented { get; set; }

	public bool IsDirect { get; set; }

	public bool IsCounter { get; set; }

	public bool IsDrag { get; set; }

	public StatusData Status { get; set; }

	public int Index { get; set; }

	public AttackBase AttackType { get; set; }

	public bool AutoReviveDeath { get; set; }

	public AbilityState CausedByPassive { get; set; }

	public override string ToString()
	{
		return Action.ToString();
	}
}
