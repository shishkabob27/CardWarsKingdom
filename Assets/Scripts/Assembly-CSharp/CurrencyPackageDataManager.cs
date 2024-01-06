using System.IO;

public class CurrencyPackageDataManager : DataManager<CurrencyPackageData>
{
	private static CurrencyPackageDataManager _instance;

	public static CurrencyPackageDataManager Instance
	{
		get
		{
			if (_instance == null)
			{
				string path = Path.Combine(SQSettings.CDN_URL, "Blueprints", "db_CurrencyPackages.json");
				_instance = new CurrencyPackageDataManager(path);
			}
			return _instance;
		}
	}

	public CurrencyPackageDataManager(string path)
	{
		base.FilePath = path;
	}
}
