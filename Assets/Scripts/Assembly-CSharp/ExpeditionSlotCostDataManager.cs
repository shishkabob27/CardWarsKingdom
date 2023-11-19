using System.IO;

public class ExpeditionSlotCostDataManager : DataManager<ExpeditionSlotCostData>
{
	private static ExpeditionSlotCostDataManager _instance;

	public static ExpeditionSlotCostDataManager Instance
	{
		get
		{
			if (_instance == null)
			{
				string path = Path.Combine("Blueprints", "db_ExpeditionSlotCosts.json");
				_instance = new ExpeditionSlotCostDataManager(path);
			}
			return _instance;
		}
	}

	public ExpeditionSlotCostDataManager(string path)
	{
		base.FilePath = path;
	}

	public int GetNextSlotPurchaseCost()
	{
		ExpeditionSlotCostData data = GetData(((int)Singleton<PlayerInfoScript>.Instance.SaveData.ExpeditionSlots + 1).ToString());
		if (data != null)
		{
			return data.Cost;
		}
		return -1;
	}
}
