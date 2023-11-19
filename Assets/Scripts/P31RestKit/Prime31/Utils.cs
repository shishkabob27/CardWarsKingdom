using System;
using System.Text;
using UnityEngine;

namespace Prime31
{
	public static class Utils
	{
		private static System.Random _random;

		private static System.Random random
		{
			get
			{
				if (_random == null)
				{
					_random = new System.Random();
				}
				return _random;
			}
		}

		public static string randomString(int size = 38)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < size; i++)
			{
				char value = Convert.ToChar(Convert.ToInt32(Math.Floor(26.0 * random.NextDouble() + 65.0)));
				stringBuilder.Append(value);
			}
			return stringBuilder.ToString();
		}

		public static void logObject(object obj)
		{
			string json = Json.encode(obj);
			prettyPrintJson(json);
		}

		public static void prettyPrintJson(string json)
		{
			string text = string.Empty;
			if (json != null)
			{
				text = JsonFormatter.prettyPrint(json);
			}
			try
			{
				Debug.Log(text);
			}
			catch (Exception)
			{
				Console.WriteLine(text);
			}
		}
	}
}
