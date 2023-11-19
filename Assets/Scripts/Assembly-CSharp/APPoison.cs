public class APPoison : StatusState
{
	public override void ProcessMessage(GameMessage Message)
	{
		if (base.Target != null && Message.Action == GameEvent.START_TURN && Message.WhichPlayer == base.Target.Owner && base.Count > 0)
		{
			int cost = (int)(base.Intensity * 100f);
			base.Target.Owner.LoseActionPoints(cost);
			TickCount();
		}
	}
}
