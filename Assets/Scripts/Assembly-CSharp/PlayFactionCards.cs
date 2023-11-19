public class PlayFactionCards : Mission
{
	public override int ProgressValue
	{
		get
		{
			return base.Progress.FactionCards[base.Val2];
		}
	}
}
