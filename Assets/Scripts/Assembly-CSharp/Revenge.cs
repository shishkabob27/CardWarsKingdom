public class Revenge : StatusState
{
	public override void ProcessMessage(GameMessage Message)
	{
		if (Message.Action == GameEvent.DAMAGE_CREATURE && Message.Creature.Owner == base.Target.Owner)
		{
			base.Intensity += Message.AmountChange;
			ReportStatusAction(GameEvent.TICK_STATUS, base.Intensity, true);
		}
		else
		{
			if (Message.Action != 0 || Message.WhichPlayer != base.Target.Owner || base.Target.HP <= 0)
			{
				return;
			}
			if (base.Intensity > 0f)
			{
				CreatureState autoTarget = Message.WhichPlayer.GetAutoTarget(base.Target, AttackBase.STR, AttackRange.Single);
				if (autoTarget != null)
				{
					base.Target.Owner.Attack(base.Target, autoTarget, AttackBase.STR, AttackCause.Revenge, base.Intensity);
				}
			}
			Disable();
		}
	}
}
