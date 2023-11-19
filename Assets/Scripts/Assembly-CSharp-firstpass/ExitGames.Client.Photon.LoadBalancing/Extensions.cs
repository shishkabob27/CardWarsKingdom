using System.Collections;

namespace ExitGames.Client.Photon.LoadBalancing
{
	public static class Extensions
	{
		public static void Merge(this IDictionary target, IDictionary addHash)
		{
			if (addHash == null || target.Equals(addHash))
			{
				return;
			}
			foreach (object key in addHash.Keys)
			{
				target[key] = addHash[key];
			}
		}

		public static void MergeStringKeys(this IDictionary target, IDictionary addHash)
		{
			if (addHash == null || target.Equals(addHash))
			{
				return;
			}
			foreach (object key in addHash.Keys)
			{
				if (key is string)
				{
					target[key] = addHash[key];
				}
			}
		}

		public static Hashtable StripToStringKeys(this IDictionary original)
		{
			Hashtable hashtable = new Hashtable();
			if (original != null)
			{
				foreach (object key in original.Keys)
				{
					if (key is string)
					{
						hashtable[key] = original[key];
					}
				}
				return hashtable;
			}
			return hashtable;
		}

		public static void StripKeysWithNullValues(this IDictionary original)
		{
			object[] array = new object[original.Count];
			original.Keys.CopyTo(array, 0);
			foreach (object key in array)
			{
				if (original[key] == null)
				{
					original.Remove(key);
				}
			}
		}

		public static bool Contains(this int[] target, int nr)
		{
			if (target == null)
			{
				return false;
			}
			for (int i = 0; i < target.Length; i++)
			{
				if (target[i] == nr)
				{
					return true;
				}
			}
			return false;
		}
	}
}
