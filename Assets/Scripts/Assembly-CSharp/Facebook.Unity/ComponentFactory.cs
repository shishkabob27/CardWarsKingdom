using UnityEngine;

namespace Facebook.Unity
{
	internal class ComponentFactory
	{
		internal enum IfNotExist
		{
			AddNew,
			ReturnNull
		}

		public const string GameObjectName = "UnityFacebookSDKPlugin";

		private static GameObject facebookGameObject;

		private static GameObject FacebookGameObject
		{
			get
			{
				if (facebookGameObject == null)
				{
					facebookGameObject = new GameObject("UnityFacebookSDKPlugin");
				}
				return facebookGameObject;
			}
		}

		public static T GetComponent<T>(IfNotExist ifNotExist = IfNotExist.AddNew) where T : MonoBehaviour
		{
			GameObject gameObject = FacebookGameObject;
			T val = gameObject.GetComponent<T>();
			if ((Object)val == (Object)null && ifNotExist == IfNotExist.AddNew)
			{
				val = gameObject.AddComponent<T>();
			}
			return val;
		}

		public static T AddComponent<T>() where T : MonoBehaviour
		{
			return FacebookGameObject.AddComponent<T>();
		}
	}
}
