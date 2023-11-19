public class DoCardAttacks : Mission
{
	public override int ProgressValue
	{
		get
		{
			return base.Progress.CardAttacks;
		}
	}
}
