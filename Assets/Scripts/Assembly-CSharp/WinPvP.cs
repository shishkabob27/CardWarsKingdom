public class WinPvP : Mission
{
	public override int ProgressValue
	{
		get
		{
			return base.Progress.PvPBattlesWon;
		}
	}
}
