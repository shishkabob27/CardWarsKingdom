using System.IO;

public class VirtualGoodsDataManager : DataManager<VirtualGoods>
{
	private static VirtualGoodsDataManager _instance;

	public static VirtualGoodsDataManager Instance
	{
		get
		{
			if (_instance == null)
			{
				string path = Path.Combine("Blueprints", "db_VirtualGoods.json");
				_instance = new VirtualGoodsDataManager(path);
			}
			return _instance;
		}
	}

	public VirtualGoodsDataManager(string path)
	{
		base.FilePath = path;
	}
}
