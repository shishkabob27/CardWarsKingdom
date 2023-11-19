public class AutoRevive : StatusState
{
	public override void ProcessMessage(GameMessage Message)
	{
		if (Message.Action == GameEvent.CREATURE_DIED && Message.Creature == base.Target)
		{
			ReportStatusAction(GameEvent.STATUS_ACTION_AUTOREVIVE);
			base.Target.Owner.ResurectCreature(base.Target, base.Intensity);
		}
	}
}
