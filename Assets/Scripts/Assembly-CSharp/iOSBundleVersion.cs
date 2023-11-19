public class iOSBundleVersion : Singleton<iOSBundleVersion>
{
	private void Awake()
	{
		if (Singleton<iOSBundleVersion>.mInstance == null)
		{
			Singleton<iOSBundleVersion>.mInstance = this;
		}
	}

	protected static void GetVersionInfo()
	{
	}
}
