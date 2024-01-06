using System.Collections.Generic;
using System.IO;

public class NotificationDataManager : DataManager<NotificationData>
{
	private static NotificationDataManager _instance;

	public bool isLoaded;

	public static NotificationDataManager Instance
	{
		get
		{
			if (_instance == null)
			{
				string path = Path.Combine(SQSettings.CDN_URL, "Blueprints", "db_Notifications.json");
				_instance = new NotificationDataManager(path);
			}
			return _instance;
		}
	}

	public NotificationDataManager(string path)
	{
		base.FilePath = path;
	}

	public List<NotificationData> GetNotificationList(NotificationType type)
	{
		return DatabaseArray.FindAll((NotificationData m) => m.nType == type);
	}

	protected override void PostLoad()
	{
		isLoaded = true;
	}
}
