public class InflictBlindIfAtkSpike : OnAttack
{
	public override bool OnEnable()
	{
		float num = (Damage - (float)base.Owner.Data.STR) / (float)base.Owner.Data.STR;
		if (num > base.Val1Pct && !Target.IsBlind)
		{
			ApplyStatus(Target, StatusEnum.Blind, 1f);
			return true;
		}
		return false;
	}
}
