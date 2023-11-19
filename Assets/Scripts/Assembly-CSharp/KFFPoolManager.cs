using System;
using System.Collections.Generic;

public class KFFPoolManager : DetachedSingleton<KFFPoolManager>
{
	private Dictionary<Type, List<object>> Pools;

	public KFFPoolManager()
	{
		Pools = new Dictionary<Type, List<object>>();
	}

	public void CreatePool(Type type, int size)
	{
		List<object> list;
		if (Pools.ContainsKey(type))
		{
			list = Pools[type];
		}
		else
		{
			list = new List<object>();
			Pools.Add(type, list);
		}
		for (int i = 0; i < size; i++)
		{
			object item = Activator.CreateInstance(type);
			list.Add(item);
		}
	}

	public void DestroyAllPools()
	{
		Pools.Clear();
	}

	public object GetObject(Type type)
	{
		if (!Pools.ContainsKey(type))
		{
			CreatePool(type, 1);
		}
		List<object> list = Pools[type];
		object result;
		if (list.Count > 0)
		{
			result = list[list.Count - 1];
			list.RemoveAt(list.Count - 1);
		}
		else
		{
			result = Activator.CreateInstance(type);
		}
		return result;
	}

	public void ReleaseObject(object obj)
	{
		Type type = obj.GetType();
		if (Pools.ContainsKey(type))
		{
			List<object> list = Pools[type];
			list.Add(obj);
		}
	}

	public void PrintPools()
	{
		foreach (Type key in Pools.Keys)
		{
			List<object> list = Pools[key];
		}
	}

	public void CheckUnreleasedObjects()
	{
	}
}
