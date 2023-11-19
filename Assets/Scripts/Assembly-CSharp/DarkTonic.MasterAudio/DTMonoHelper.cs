using UnityEngine;

namespace DarkTonic.MasterAudio
{
	public static class DTMonoHelper
	{
		public static bool IsActive(GameObject go)
		{
			return go.activeInHierarchy;
		}

		public static void SetActive(GameObject go, bool isActive)
		{
			go.SetActive(isActive);
		}
	}
}
