using UnityEngine;

public class LandNice : StatusState
{
	public override void ProcessMessage(GameMessage Message)
	{
		if (base.Target != null && base.Target.HP > 0 && Message.Action == GameEvent.START_TURN && Message.WhichPlayer == base.Target.Owner)
		{
			int num = Mathf.CeilToInt((float)base.Target.MaxHP * base.Intensity);
			if (num < 1)
			{
				num = 1;
			}
			ReportStatusAction(GameEvent.STATUS_ACTION_REGEN, num);
			base.Target.Heal(num);
			TickCount();
		}
	}

	public override bool AttackRandom()
	{
		return true;
	}
}
