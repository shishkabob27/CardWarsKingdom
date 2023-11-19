public class GrantAttkSpikeOnFactionCreatureDie : OnFriendlyDamaged
{
	public override bool OnEnable()
	{
		bool result = false;
		if (Attacked.HP <= 0 && Attacked.Data.Faction == (CreatureFaction)base.Val2)
		{
			foreach (LaneState lane in base.Owner.Owner.Lanes)
			{
				if (lane.Creature != null)
				{
					ApplyStatus(lane.Creature, StatusEnum.AttackSpike, base.Val1);
					result = true;
				}
			}
			return result;
		}
		return result;
	}
}
