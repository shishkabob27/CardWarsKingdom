using System.IO;

public class XPMaterialDataManager : DataManager<XPMaterialData>
{
	private static XPMaterialDataManager _instance;

	public static XPMaterialDataManager Instance
	{
		get
		{
			if (_instance == null)
			{
				string path = Path.Combine(SQSettings.CDN_URL, "Blueprints", "db_XPMaterials.json");
				_instance = new XPMaterialDataManager(path);
			}
			return _instance;
		}
	}

	public XPMaterialDataManager(string path)
	{
		base.FilePath = path;
	}
}
