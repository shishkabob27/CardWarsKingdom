using System;
using System.Collections.Generic;

public class KFFDictionary
{
	public List<KFFDictionaryEntry> entries = new List<KFFDictionaryEntry>();

	private Dictionary<string, string> dict = new Dictionary<string, string>();

	public void CreateDictionary()
	{
		dict.Clear();
		foreach (KFFDictionaryEntry entry in entries)
		{
			dict[entry.key] = entry.value;
		}
	}

	public string GetValue(string key)
	{
		if (!ContainsKey(key))
		{
			return null;
		}
		string value = string.Empty;
		dict.TryGetValue(key, out value);
		return value;
	}

	public int GetValueAsInt(string key)
	{
		//Discarded unreachable code: IL_002c
		string value = GetValue(key);
		if (value == null || value == string.Empty)
		{
			return 0;
		}
		try
		{
			return int.Parse(value);
		}
		catch (Exception)
		{
		}
		return 0;
	}

	public float GetValueAsFloat(string key)
	{
		//Discarded unreachable code: IL_0036
		string value = GetValue(key);
		if (value == null || value == string.Empty)
		{
			return 0f;
		}
		try
		{
			return float.Parse(GetValue(key));
		}
		catch (Exception)
		{
		}
		return 0f;
	}

	public string GetValueAsString(string key)
	{
		string value = GetValue(key);
		if (value == null)
		{
			return string.Empty;
		}
		return value;
	}

	public bool ContainsKey(string key)
	{
		return dict.ContainsKey(key);
	}

	public void SetValue(string key, string value)
	{
		if (ContainsKey(key))
		{
			foreach (KFFDictionaryEntry entry in entries)
			{
				if (entry.key == key)
				{
					entry.value = value;
					break;
				}
			}
		}
		else
		{
			KFFDictionaryEntry kFFDictionaryEntry = new KFFDictionaryEntry();
			kFFDictionaryEntry.key = key;
			kFFDictionaryEntry.value = value;
			entries.Add(kFFDictionaryEntry);
		}
		dict[key] = value;
	}

	public void SetValue(string key, float value)
	{
		SetValue(key, value.ToString("F16"));
	}

	public void SetValue(string key, int value)
	{
		SetValue(key, string.Empty + value);
	}

	public void SetValue(string key, object value)
	{
		if (value == null)
		{
			SetValue(key, null);
			return;
		}
		if (value.GetType() == typeof(int))
		{
			SetValue(key, (int)value);
			return;
		}
		if (value.GetType() == typeof(float))
		{
			SetValue(key, (float)value);
			return;
		}
		try
		{
			SetValue(key, value.ToString());
		}
		catch (Exception)
		{
			SetValue(key, null);
		}
	}
}
