public class PosionEnergyBleedBurnOnCrit : OnCritical
{
	public override bool OnEnable()
	{
		ApplyStatus(Target, StatusEnum.Poison, base.Val1);
		ApplyStatus(Target, StatusEnum.EnergyBleed, base.Val1);
		ApplyStatus(Target, StatusEnum.Burn, base.Val2);
		return true;
	}
}
