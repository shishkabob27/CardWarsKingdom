using System.Collections.Generic;

public abstract class OnEnemyAttackThreshold : AbilityState
{
	protected CreatureState Attacker;

	protected CreatureState Target;

	protected bool IsDragAttack;

	public override bool ProcessMessage(GameMessage Message)
	{
		bool result = false;
		if (Message.Action == GameEvent.START_TURN && Message.WhichPlayer == base.Owner.Owner)
		{
			IsActive = false;
			StoredCreatures.Clear();
		}
		if (!IsActive && Message.Action == GameEvent.CREATURE_ATTACKED && Message.Creature.Owner == base.Owner.Owner.Opponent)
		{
			Attacker = Message.Creature;
			Target = Message.SecondCreature;
			IsDragAttack = Message.IsDrag;
			if (!StoredCreatures.ContainsKey(Message.Creature))
			{
				StoredCreatures.Add(Message.Creature, 1);
			}
			else
			{
				Dictionary<CreatureState, int> storedCreatures;
				Dictionary<CreatureState, int> dictionary = (storedCreatures = StoredCreatures);
				CreatureState creature;
				CreatureState key = (creature = Message.Creature);
				int num = storedCreatures[creature];
				dictionary[key] = num + 1;
			}
			if (StoredCreatures[Message.Creature] >= base.Val1)
			{
				result = OnEnable();
				IsActive = true;
			}
		}
		return result;
	}
}
