public class Burned : StatusState
{
	public override void ProcessMessage(GameMessage Message)
	{
		if (base.Target != null && Message.Action == GameEvent.CREATURE_ATTACKED && Message.Creature == base.Target)
		{
			int num = (int)((float)base.Target.MaxHP * base.Intensity);
			ReportStatusAction(GameEvent.STATUS_ACTION_BURNED, num);
			base.Target.DealDamage(num, AttackBase.None, false, null, this);
			Disable();
		}
	}
}
