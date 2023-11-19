using UnityEngine;

public class LandSand : StatusState
{
	public override void ProcessMessage(GameMessage Message)
	{
		if (Message.Action != 0 || Message.WhichPlayer != base.Target.Owner)
		{
			return;
		}
		foreach (LaneState lane in base.Target.Owner.Opponent.Lanes)
		{
			if (lane.Creature != null)
			{
				int num = Mathf.CeilToInt((float)lane.Creature.MaxHP * base.Intensity);
				GameMessage gameMessage = new GameMessage();
				gameMessage.Action = GameEvent.STATUS_ACTION_SANDLAND;
				gameMessage.Creature = lane.Creature;
				gameMessage.Lane = lane;
				gameMessage.WhichPlayer = lane.Creature.Owner;
				gameMessage.Status = base.Data;
				gameMessage.Amount = num;
				base.Target.Owner.Game.AddMessage(gameMessage);
				lane.Creature.DealDamage(num, AttackBase.None, false, null, this);
			}
		}
	}

	public override bool GuaranteeCritAgainst()
	{
		return true;
	}
}
