public class GainBuffs : Mission
{
	public override int ProgressValue
	{
		get
		{
			return base.Progress.BuffsGranted;
		}
	}
}
