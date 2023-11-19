namespace DarkTonic.MasterAudio
{
	public static class UtilStrings
	{
		public static string TrimSpace(string untrimmed)
		{
			if (string.IsNullOrEmpty(untrimmed))
			{
				return string.Empty;
			}
			return untrimmed.Trim();
		}

		public static string ReplaceUnsafeChars(string source)
		{
			source = source.Replace("'", "&apos;");
			source = source.Replace("\"", "&quot;");
			source = source.Replace("&", "&amp;");
			return source;
		}
	}
}
