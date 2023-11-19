public class WinAny : Mission
{
	public override int ProgressValue
	{
		get
		{
			return base.Progress.BattlesWon;
		}
	}
}
