public class EraseBraveryInflictPlaugeOnAttack : OnAttack
{
	public override bool OnEnable()
	{
		if ((bool)Target.HasBravery)
		{
			CancelStatus(Target, StatusEnum.Bravery);
			ApplyStatus(Target, StatusEnum.Plague, base.Val1);
			return true;
		}
		return false;
	}
}
