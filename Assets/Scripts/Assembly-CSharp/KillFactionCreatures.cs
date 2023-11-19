public class KillFactionCreatures : Mission
{
	public override int ProgressValue
	{
		get
		{
			return base.Progress.FactionKills[base.Val2];
		}
	}
}
