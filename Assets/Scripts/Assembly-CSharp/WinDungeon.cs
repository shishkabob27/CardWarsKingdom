public class WinDungeon : Mission
{
	public override int ProgressValue
	{
		get
		{
			return base.Progress.DungeonBattlesWon;
		}
	}
}
