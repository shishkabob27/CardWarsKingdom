using UnityEngine;

public class APSiphon : StatusState
{
	public override void ProcessMessage(GameMessage Message)
	{
		if (base.Target != null && Message.Action == GameEvent.CREATURE_ATTACKED && Message.Creature == base.Target)
		{
			int num = Mathf.RoundToInt(base.Intensity * 100f);
			ReportStatusAction(GameEvent.STATUS_ACTION_APSIPHON, num);
			base.Target.Owner.Opponent.LoseActionPoints(num);
			base.Target.Owner.AddActionPoints(num);
			Disable();
		}
	}
}
