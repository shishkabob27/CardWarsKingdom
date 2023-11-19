using UnityEngine;

public class LandPlains : StatusState
{
	public override int BonusEnergy()
	{
		return Mathf.RoundToInt(base.Intensity * 100f);
	}

	public override void ProcessMessage(GameMessage Message)
	{
		if (Message.Action == GameEvent.START_TURN && Message.WhichPlayer == base.Target.Owner)
		{
			int num = Mathf.RoundToInt((float)base.Target.Owner.Leader.Form.APThreshold * base.Intensity2);
			base.Target.Owner.FillAPMeter(-num);
		}
	}
}
