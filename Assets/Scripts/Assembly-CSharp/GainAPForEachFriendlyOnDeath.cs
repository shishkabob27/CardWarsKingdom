public class GainAPForEachFriendlyOnDeath : OnDied
{
	public override bool OnEnable()
	{
		bool result = false;
		foreach (LaneState lane in base.Owner.Owner.Lanes)
		{
			if (lane.Creature != null)
			{
				base.Owner.Owner.AddActionPoints(base.Val1);
				result = true;
			}
		}
		return result;
	}
}
