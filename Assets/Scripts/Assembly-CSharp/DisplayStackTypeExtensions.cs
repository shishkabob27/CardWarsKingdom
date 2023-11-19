using UnityEngine;

public static class DisplayStackTypeExtensions
{
	public static string Format(this DisplayStackType stackType, float value, bool verbose = false)
	{
		switch (stackType)
		{
		case DisplayStackType.None:
			return string.Empty;
		case DisplayStackType.Count:
			return (int)value + "x";
		case DisplayStackType.Turns:
			if (verbose)
			{
				if ((int)value == 1)
				{
					return KFFLocalization.Get("!!1_TURN").Replace("<val1>", 1.ToString());
				}
				return (int)value + " " + KFFLocalization.Get("!!TURNS");
			}
			return (int)value + " " + KFFLocalization.Get("!!TURNS_LETTER");
		case DisplayStackType.Percent:
			return (int)(100f * (value + 1E-05f)) + "%";
		case DisplayStackType.Amount:
			return ((int)(100f * (value + 1E-05f))).ToString();
		case DisplayStackType.IntAmount:
			return Mathf.RoundToInt(value).ToString();
		default:
			return string.Empty;
		}
	}
}
