using UnityEngine;

namespace Prime31
{
	public class AndroidNotificationConfiguration
	{
		public long secondsFromNow;

		public string title = string.Empty;

		public string subtitle = string.Empty;

		public string tickerText = string.Empty;

		public string extraData = string.Empty;

		public string smallIcon = string.Empty;

		public string largeIcon = string.Empty;

		public int requestCode = -1;

		public string groupKey = string.Empty;

		public int color = -1;

		public bool isGroupSummary;

		public int cancelsNotificationId = -1;

		public bool sound = true;

		public bool vibrate = true;

		public bool useExactTiming;

		public AndroidNotificationConfiguration(long secondsFromNow, string title, string subtitle, string tickerText)
		{
			this.secondsFromNow = secondsFromNow;
			this.title = title;
			this.subtitle = subtitle;
			this.tickerText = tickerText;
		}

		public AndroidNotificationConfiguration build()
		{
			if (requestCode == -1)
			{
				requestCode = Random.Range(0, int.MaxValue);
			}
			return this;
		}
	}
}
