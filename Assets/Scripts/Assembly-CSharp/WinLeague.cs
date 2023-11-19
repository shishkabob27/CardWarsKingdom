public class WinLeague : Mission
{
	public override int ProgressValue
	{
		get
		{
			return base.Progress.LeagueBattlesWon;
		}
	}
}
