public class DetachedSingleton<T> where T : class, new()
{
	protected static T mInstance;

	public static T Instance
	{
		get
		{
			if (mInstance == null)
			{
				mInstance = new T();
			}
			return mInstance;
		}
	}
}
