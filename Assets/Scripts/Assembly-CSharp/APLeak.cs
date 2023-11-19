public class APLeak : StatusState
{
	public override void ProcessMessage(GameMessage Message)
	{
		if (base.Target != null && Message.Action == GameEvent.CREATURE_ATTACKED && Message.SecondCreature == base.Target)
		{
			int num = (int)((float)base.Target.Owner.ActionPoints * base.Intensity);
			ReportStatusAction(GameEvent.STATUS_ACTION_APLEAK, num);
			base.Target.Owner.LoseActionPoints(num);
		}
	}
}
