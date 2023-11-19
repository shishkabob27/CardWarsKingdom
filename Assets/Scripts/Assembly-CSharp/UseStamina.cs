public class UseStamina : Mission
{
	public override int ProgressValue
	{
		get
		{
			return base.Progress.StaminaUsed;
		}
	}
}
