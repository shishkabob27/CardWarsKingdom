public abstract class OnHPThreshold : AbilityState
{
	public override bool ProcessMessage(GameMessage Message)
	{
		bool result = false;
		if (!IsActive && Message.Action == GameEvent.DAMAGE_CREATURE && Message.Creature == base.Owner && base.Owner.HPPct <= base.Val1Pct)
		{
			result = OnEnable();
			IsActive = true;
		}
		if (IsActive && Message.Action == GameEvent.HEAL_CREATURE && Message.Creature == base.Owner && base.Owner.HPPct > base.Val1Pct)
		{
			OnDisable();
			IsActive = false;
		}
		return result;
	}
}
