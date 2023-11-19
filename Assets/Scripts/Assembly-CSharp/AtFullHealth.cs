public abstract class AtFullHealth : AbilityState
{
	public override bool ProcessMessage(GameMessage Message)
	{
		bool result = false;
		if (Message.Action == GameEvent.CREATURE_DEPLOYED && Message.Creature == base.Owner)
		{
			result = OnEnable();
			IsActive = true;
		}
		if (IsActive && Message.Action == GameEvent.DAMAGE_CREATURE && Message.Creature == base.Owner && base.Owner.HP < base.Owner.MaxHP)
		{
			OnDisable();
			IsActive = false;
		}
		if (!IsActive && Message.Action == GameEvent.HEAL_CREATURE && Message.Creature == base.Owner && base.Owner.HP >= base.Owner.MaxHP)
		{
			result = OnEnable();
			IsActive = true;
		}
		return result;
	}
}
