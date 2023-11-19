namespace Helpshift
{
	public class HelpshiftLog
	{
		public static int v(string tag, string log)
		{
			return HelpshiftAndroidLog.v(tag, log);
		}

		public static int d(string tag, string log)
		{
			return HelpshiftAndroidLog.d(tag, log);
		}

		public static int i(string tag, string log)
		{
			return HelpshiftAndroidLog.i(tag, log);
		}

		public static int w(string tag, string log)
		{
			return HelpshiftAndroidLog.w(tag, log);
		}

		public static int e(string tag, string log)
		{
			return HelpshiftAndroidLog.e(tag, log);
		}
	}
}
