public class Vampiric : StatusState
{
	public override void ProcessMessage(GameMessage Message)
	{
		if (base.Target != null && base.Target.HP > 0 && Message.Action == GameEvent.CREATURE_ATTACKED && Message.Creature == base.Target)
		{
			int num = (int)(Message.Amount * base.Intensity);
			ReportStatusAction(GameEvent.STATUS_ACTION_VAMPIRIC, num, false, Message.SecondCreature);
			base.Target.Heal(num);
			Disable();
		}
	}
}
