using System.Collections.Generic;
using UnityEngine;

namespace DarkTonic.MasterAudio
{
	public static class ArrayListUtil
	{
		public static void SortIntArray(ref List<int> list)
		{
			for (int i = 0; i < list.Count; i++)
			{
				int value = list[i];
				int index = Random.Range(i, list.Count);
				list[i] = list[index];
				list[index] = value;
			}
		}
	}
}
