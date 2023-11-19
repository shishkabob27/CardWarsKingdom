using UnityEngine;

namespace Facebook.Unity
{
	internal static class FacebookLogger
	{
		private class CustomLogger : IFacebookLogger
		{
			private IFacebookLogger logger;

			public CustomLogger()
			{
				logger = new AndroidLogger();
			}

			public void Log(string msg)
			{
				if (UnityEngine.Debug.isDebugBuild)
				{
					UnityEngine.Debug.Log(msg);
					logger.Log(msg);
				}
			}

			public void Info(string msg)
			{
				UnityEngine.Debug.Log(msg);
				logger.Info(msg);
			}

			public void Warn(string msg)
			{
				UnityEngine.Debug.LogWarning(msg);
				logger.Warn(msg);
			}

			public void Error(string msg)
			{
				UnityEngine.Debug.LogError(msg);
				logger.Error(msg);
			}
		}

		private class AndroidLogger : IFacebookLogger
		{
			public void Log(string msg)
			{
				using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("android.util.Log"))
				{
					androidJavaClass.CallStatic<int>("v", new object[2] { "Facebook.Unity.FBDebug", msg });
				}
			}

			public void Info(string msg)
			{
				using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("android.util.Log"))
				{
					androidJavaClass.CallStatic<int>("i", new object[2] { "Facebook.Unity.FBDebug", msg });
				}
			}

			public void Warn(string msg)
			{
				using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("android.util.Log"))
				{
					androidJavaClass.CallStatic<int>("w", new object[2] { "Facebook.Unity.FBDebug", msg });
				}
			}

			public void Error(string msg)
			{
				using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("android.util.Log"))
				{
					androidJavaClass.CallStatic<int>("e", new object[2] { "Facebook.Unity.FBDebug", msg });
				}
			}
		}

		private const string UnityAndroidTag = "Facebook.Unity.FBDebug";

		internal static IFacebookLogger Instance { private get; set; }

		static FacebookLogger()
		{
			Instance = new CustomLogger();
		}

		public static void Log(string msg)
		{
			Instance.Log(msg);
		}

		public static void Log(string format, params string[] args)
		{
			Log(string.Format(format, args));
		}

		public static void Info(string msg)
		{
			Instance.Info(msg);
		}

		public static void Info(string format, params string[] args)
		{
			Info(string.Format(format, args));
		}

		public static void Warn(string msg)
		{
			Instance.Warn(msg);
		}

		public static void Warn(string format, params string[] args)
		{
			Warn(string.Format(format, args));
		}

		public static void Error(string msg)
		{
			Instance.Error(msg);
		}

		public static void Error(string format, params string[] args)
		{
			Error(string.Format(format, args));
		}
	}
}
