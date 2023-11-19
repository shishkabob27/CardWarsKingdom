using System.IO;

public class GachaSlotDataManager : DataManager<GachaSlotData>
{
	public static int PremiumGachaCost = 500;

	private static GachaSlotDataManager _instance;

	public static GachaSlotDataManager Instance
	{
		get
		{
			if (_instance == null)
			{
				string path = Path.Combine("Blueprints", "db_GachaSlots.json");
				_instance = new GachaSlotDataManager(path);
			}
			return _instance;
		}
	}

	public GachaSlotDataManager(string path)
	{
		base.FilePath = path;
		AddDependency(GachaWeightDataManager.Instance);
	}
}
