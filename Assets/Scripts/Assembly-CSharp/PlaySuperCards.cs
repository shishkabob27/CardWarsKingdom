public class PlaySuperCards : Mission
{
	public override int ProgressValue
	{
		get
		{
			return base.Progress.SuperCardsPlayed;
		}
	}
}
