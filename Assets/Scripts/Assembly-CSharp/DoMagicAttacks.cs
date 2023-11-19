public class DoMagicAttacks : Mission
{
	public override int ProgressValue
	{
		get
		{
			return base.Progress.MagicAttacks;
		}
	}
}
