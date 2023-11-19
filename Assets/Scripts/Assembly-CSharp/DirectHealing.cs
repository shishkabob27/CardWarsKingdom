public class DirectHealing : StatusState
{
	protected override void OnEnable()
	{
		if (base.Intensity != 0f)
		{
			base.Target.Heal(base.Intensity);
		}
	}
}
