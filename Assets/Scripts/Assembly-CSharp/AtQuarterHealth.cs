public abstract class AtQuarterHealth : AbilityState
{
	public override bool ProcessMessage(GameMessage Message)
	{
		bool result = false;
		if (!IsActive && Message.Action == GameEvent.DAMAGE_CREATURE && Message.Creature == base.Owner && base.Owner.HPPct <= 0.25f)
		{
			result = OnEnable();
			IsActive = true;
		}
		if (IsActive && Message.Action == GameEvent.HEAL_CREATURE && Message.Creature == base.Owner && base.Owner.HPPct > 0.25f)
		{
			OnDisable();
			IsActive = false;
		}
		return result;
	}
}
