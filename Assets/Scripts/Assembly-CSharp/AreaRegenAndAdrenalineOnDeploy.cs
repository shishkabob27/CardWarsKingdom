public class AreaRegenAndAdrenalineOnDeploy : OnDeploy
{
	public override bool OnEnable()
	{
		foreach (LaneState thisAndAdjacentLane in base.Owner.Lane.ThisAndAdjacentLanes)
		{
			if (thisAndAdjacentLane.Creature != null)
			{
				ApplyStatus(thisAndAdjacentLane.Creature, StatusEnum.Regen, base.Val1);
				ApplyStatus(thisAndAdjacentLane.Creature, StatusEnum.Adrenaline, base.Val1);
			}
		}
		return true;
	}
}
