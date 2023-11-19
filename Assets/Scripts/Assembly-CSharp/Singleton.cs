using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	protected static T mInstance;

	public static T Instance
	{
		get
		{
			if ((Object)null == (Object)mInstance)
			{
				mInstance = (T)Object.FindObjectOfType(typeof(T));
				if (!((Object)null == (Object)mInstance))
				{
				}
			}
			return mInstance;
		}
	}

	public void Destroy()
	{
		mInstance = (T)null;
	}

	private void OnDestroy()
	{
		mInstance = (T)null;
	}
}
