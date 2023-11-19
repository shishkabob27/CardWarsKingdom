public class WinHighestLeague : Mission
{
	public override int ProgressValue
	{
		get
		{
			return base.Progress.HighestLeagueBattlesWon;
		}
	}
}
