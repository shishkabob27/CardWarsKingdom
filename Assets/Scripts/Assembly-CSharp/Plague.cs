using UnityEngine;

public class Plague : StatusState
{
	public override void ProcessMessage(GameMessage Message)
	{
		if (base.Target != null && Message.Action == GameEvent.START_TURN && Message.WhichPlayer == base.Target.Owner)
		{
			int num = Mathf.CeilToInt((float)base.Target.MaxHP * base.Intensity);
			ReportStatusAction(GameEvent.STATUS_ACTION_PLAGUE, num);
			base.Target.DealDamage(num, AttackBase.None, false, null, this);
		}
	}
}
