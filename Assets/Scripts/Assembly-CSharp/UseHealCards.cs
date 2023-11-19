public class UseHealCards : Mission
{
	public override int ProgressValue
	{
		get
		{
			return base.Progress.HealCardsPlayed;
		}
	}
}
