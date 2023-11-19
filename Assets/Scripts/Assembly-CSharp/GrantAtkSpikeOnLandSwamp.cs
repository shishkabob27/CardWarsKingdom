public class GrantAtkSpikeOnLandSwamp : OnLandSwamp
{
	public override bool OnEnable()
	{
		ApplyStatus(base.Owner, StatusEnum.AttackSpike, base.Val1);
		return true;
	}
}
