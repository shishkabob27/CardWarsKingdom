using System.Collections.Generic;
using System.IO;

public class EvoMaterialDataManager : DataManager<EvoMaterialData>
{
	private static EvoMaterialDataManager _instance;

	public static EvoMaterialDataManager Instance
	{
		get
		{
			if (_instance == null)
			{
				string path = Path.Combine(SQSettings.CDN_URL, "Blueprints", "db_EvoMaterials.json");
				_instance = new EvoMaterialDataManager(path);
			}
			return _instance;
		}
	}

	public EvoMaterialDataManager(string path)
	{
		base.FilePath = path;
	}

	public List<EvoMaterialData> GetDatabaseByExpeditionDrop()
	{
		List<EvoMaterialData> list = new List<EvoMaterialData>();
		foreach (EvoMaterialData item in DatabaseArray)
		{
			if (item.ExpeditionDrop)
			{
				list.Add(item);
			}
		}
		return list;
	}

	public List<EvoMaterialData> GetDatabaseByRarity(int rarity, bool filterByExpeditionDrop = false)
	{
		List<EvoMaterialData> list = new List<EvoMaterialData>();
		foreach (EvoMaterialData item in DatabaseArray)
		{
			if (item.Rarity == rarity && (!filterByExpeditionDrop || (filterByExpeditionDrop && item.ExpeditionDrop)))
			{
				list.Add(item);
			}
		}
		return list;
	}
}
