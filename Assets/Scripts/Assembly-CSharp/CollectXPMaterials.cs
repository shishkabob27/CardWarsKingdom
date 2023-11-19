public class CollectXPMaterials : Mission
{
	public override int ProgressValue
	{
		get
		{
			return base.Progress.XPMaterialsCollected;
		}
	}
}
