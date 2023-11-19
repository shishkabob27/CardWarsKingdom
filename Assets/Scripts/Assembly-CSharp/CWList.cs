using System.Collections.Generic;

public class CWList<T> : List<T>
{
	public T RandomItem()
	{
		if (Count <= 0)
		{
			return default(T);
		}
		int index = KFFRandom.RandomIndex(Count);
		return this[index];
	}
}
