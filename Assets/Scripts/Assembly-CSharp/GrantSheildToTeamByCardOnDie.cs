using UnityEngine;

public class GrantSheildToTeamByCardOnDie : OnDied
{
	public override bool OnEnable()
	{
		bool result = false;
		int num = (int)Mathf.Floor(base.Owner.Owner.Hand.Count / base.Val1);
		if (num > 0)
		{
			foreach (LaneState lane in base.Owner.Owner.Lanes)
			{
				if (lane.Creature != null)
				{
					ApplyStatus(lane.Creature, StatusEnum.Shield, num);
					result = true;
				}
			}
			return result;
		}
		return result;
	}
}
