public class RemoveLandDrawCardOnAttack : OnAttack
{
	public override bool OnEnable()
	{
		bool flag = false;
		StatusState statusState = Target.StatusEffects.Find((StatusState s) => s is LandCorn);
		StatusState statusState2 = Target.StatusEffects.Find((StatusState s) => s is LandPlains);
		StatusState statusState3 = Target.StatusEffects.Find((StatusState s) => s is LandSand);
		StatusState statusState4 = Target.StatusEffects.Find((StatusState s) => s is LandNice);
		StatusState statusState5 = Target.StatusEffects.Find((StatusState s) => s is LandSwamp);
		if (statusState != null)
		{
			CancelStatus(Target, StatusEnum.LandCorn);
			flag = true;
		}
		if (statusState2 != null)
		{
			CancelStatus(Target, StatusEnum.LandPlains);
			flag = true;
		}
		if (statusState3 != null)
		{
			CancelStatus(Target, StatusEnum.LandSand);
			flag = true;
		}
		if (statusState4 != null)
		{
			CancelStatus(Target, StatusEnum.LandNice);
			flag = true;
		}
		if (statusState5 != null)
		{
			CancelStatus(Target, StatusEnum.LandSwamp);
			flag = true;
		}
		if (flag)
		{
			for (int i = 0; i < base.Val1; i++)
			{
				base.Owner.DrawCard();
			}
		}
		return flag;
	}
}
