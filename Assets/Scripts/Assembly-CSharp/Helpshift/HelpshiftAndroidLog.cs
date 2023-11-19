using UnityEngine;

namespace Helpshift
{
	public class HelpshiftAndroidLog
	{
		private static AndroidJavaClass logger;

		private HelpshiftAndroidLog()
		{
		}

		private static void initLogger()
		{
			if (logger == null)
			{
				logger = new AndroidJavaClass("com.helpshift.Log");
			}
		}

		public static int v(string tag, string log)
		{
			initLogger();
			return logger.CallStatic<int>("v", new object[2] { tag, log });
		}

		public static int d(string tag, string log)
		{
			initLogger();
			return logger.CallStatic<int>("d", new object[2] { tag, log });
		}

		public static int i(string tag, string log)
		{
			initLogger();
			return logger.CallStatic<int>("i", new object[2] { tag, log });
		}

		public static int w(string tag, string log)
		{
			initLogger();
			return logger.CallStatic<int>("w", new object[2] { tag, log });
		}

		public static int e(string tag, string log)
		{
			initLogger();
			return logger.CallStatic<int>("e", new object[2] { tag, log });
		}
	}
}
