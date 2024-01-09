using System;
using System.Collections.Generic;
using UnityEngine;

public static class UnityExtensions
{

	public static GameObject InstantiateAsChild(this Transform parent, GameObject original)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(original, parent.position, parent.rotation) as GameObject;
		gameObject.transform.parent = parent;
		gameObject.transform.localScale = original.transform.localScale;
		gameObject.transform.localPosition = original.transform.localPosition;
		return gameObject;
	}

	public static Transform InstantiateAsChild(this GameObject parent, Transform original)
	{
		Transform transform = UnityEngine.Object.Instantiate(original, parent.transform.position, parent.transform.rotation) as Transform;
		transform.parent = parent.transform;
		transform.localScale = original.localScale;
		transform.localPosition = original.localPosition;
		return transform;
	}

	public static GameObject InstantiateAsChild(this GameObject parent, GameObject original)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(original, parent.transform.position, parent.transform.rotation) as GameObject;
		gameObject.transform.parent = parent.transform;
		gameObject.transform.localScale = original.transform.localScale;
		gameObject.transform.localPosition = original.transform.localPosition;
		return gameObject;
	}

	public static void DestroyAllChildren(this Transform trans)
	{
		List<GameObject> list = new List<GameObject>();
		foreach (Transform tran in trans)
		{
			list.Add(tran.gameObject);
		}
		list.ForEach(delegate(GameObject child)
		{
			child.transform.parent = null;
			UnityEngine.Object.Destroy(child);
		});
	}

	public static uint UnixTimestamp(this DateTime dateTime)
	{
		return (uint)dateTime.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
	}

	public static void ChangeLayer(this GameObject obj, int layer)
	{
		Transform[] componentsInChildren = obj.GetComponentsInChildren<Transform>(true);
		foreach (Transform transform in componentsInChildren)
		{
			transform.gameObject.layer = layer;
		}
		UIWidget[] componentsInChildren2 = obj.GetComponentsInChildren<UIWidget>(true);
		foreach (UIWidget uIWidget in componentsInChildren2)
		{
			uIWidget.ParentHasChanged();
		}
	}

	public static void ChangeLayerToParent(this GameObject obj)
	{
		obj.ChangeLayer(obj.transform.parent.gameObject.layer);
	}

	public static void SetLocalPositionX(this Transform trans, float x)
	{
		Vector3 localPosition = trans.localPosition;
		localPosition.x = x;
		trans.localPosition = localPosition;
	}

	public static void SetLocalPositionY(this Transform trans, float y)
	{
		Vector3 localPosition = trans.localPosition;
		localPosition.y = y;
		trans.localPosition = localPosition;
	}

	public static void SetLocalPositionZ(this Transform trans, float z)
	{
		Vector3 localPosition = trans.localPosition;
		localPosition.z = z;
		trans.localPosition = localPosition;
	}

	public static void AddLocalPositionX(this Transform trans, float x)
	{
		Vector3 localPosition = trans.localPosition;
		localPosition.x += x;
		trans.localPosition = localPosition;
	}

	public static void AddLocalPositionY(this Transform trans, float y)
	{
		Vector3 localPosition = trans.localPosition;
		localPosition.y += y;
		trans.localPosition = localPosition;
	}

	public static void AddLocalPositionZ(this Transform trans, float z)
	{
		Vector3 localPosition = trans.localPosition;
		localPosition.z += z;
		trans.localPosition = localPosition;
	}

	public static void SetLocalScaleX(this Transform trans, float x)
	{
		Vector3 localScale = trans.localScale;
		localScale.x = x;
		trans.localScale = localScale;
	}

	public static void SetLocalScaleY(this Transform trans, float y)
	{
		Vector3 localScale = trans.localScale;
		localScale.y = y;
		trans.localScale = localScale;
	}

	public static void SetLocalScaleZ(this Transform trans, float z)
	{
		Vector3 localScale = trans.localScale;
		localScale.z = z;
		trans.localScale = localScale;
	}

	public static GameObject FindInChildren(this GameObject obj, string name)
	{
		Transform[] componentsInChildren = obj.GetComponentsInChildren<Transform>(true);
		Transform[] array = componentsInChildren;
		foreach (Transform transform in array)
		{
			if (transform.name == name)
			{
				return transform.gameObject;
			}
		}
		return null;
	}

	public static T Find<T>(this T[] array, Predicate<T> pred) where T : class
	{
		foreach (T val in array)
		{
			if (pred(val))
			{
				return val;
			}
		}
		return (T)null;
	}

	public static int IndexOf<T>(this T[] array, T obj) where T : class
	{
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == obj)
			{
				return i;
			}
		}
		return -1;
	}

	public static List<T> FindAll<T>(this T[] array, Predicate<T> pred)
	{
		List<T> list = new List<T>();
		foreach (T val in array)
		{
			if (pred(val))
			{
				list.Add(val);
			}
		}
		return list;
	}

	public static List<int> FindIndexes<T>(this List<T> list, Predicate<T> pred)
	{
		List<int> list2 = new List<int>();
		for (int i = 0; i < list.Count; i++)
		{
			if (pred(list[i]))
			{
				list2.Add(i);
			}
		}
		return list2;
	}

	public static bool Contains<T>(this List<T> list, Predicate<T> pred)
	{
		return list.Find(pred) != null;
	}

	public static float TickTowards(this float val, float target, float perSecond)
	{
		return val.TickTowards(target, perSecond, perSecond);
	}

	public static float TickTowards(this float val, float target, float perSecondUp, float perSecondDown)
	{
		perSecondUp = Mathf.Abs(perSecondUp);
		perSecondDown = Mathf.Abs(perSecondDown);
		if (val < target)
		{
			val += Time.deltaTime * perSecondUp;
			if (val > target)
			{
				val = target;
			}
		}
		else if (val > target)
		{
			val -= Time.deltaTime * perSecondDown;
			if (val < target)
			{
				val = target;
			}
		}
		return val;
	}

	public static string ToHexString(this Color color)
	{
		return ((int)(color.r * 255f)).ToString("X2") + ((int)(color.g * 255f)).ToString("X2") + ((int)(color.b * 255f)).ToString("X2");
	}

	public static Color ToColor(this string inColorString)
	{
		if (string.IsNullOrEmpty(inColorString))
		{
			return Color.clear;
		}
		switch (inColorString.ToLower())
		{
		case "red":
			return Color.red;
		case "green":
			return Color.green;
		case "blue":
			return Color.blue;
		case "yellow":
			return Color.yellow;
		case "white":
			return Color.white;
		case "black":
			return Color.black;
		case "grey":
		case "gray":
			return Color.grey;
		case "clear":
			return Color.clear;
		default:
		{
			string[] array = inColorString.Trim().Split(' ');
			if (array.Length == 3)
			{
				byte r = byte.Parse(array[0]);
				byte g = byte.Parse(array[1]);
				byte b = byte.Parse(array[2]);
				byte a = byte.MaxValue;
				return new Color32(r, g, b, a);
			}
			if (array.Length == 4)
			{
				byte r2 = byte.Parse(array[0]);
				byte g2 = byte.Parse(array[1]);
				byte b2 = byte.Parse(array[2]);
				byte a2 = byte.Parse(array[3]);
				return new Color32(r2, g2, b2, a2);
			}
			return Color.magenta;
		}
		}
	}

	public static List<T> Copy<T>(this List<T> list)
	{
		List<T> list2 = new List<T>(list.Count);
		foreach (T item in list)
		{
			list2.Add(item);
		}
		return list2;
	}

	public static void SetParentActive(this Transform trans, bool value)
	{
		trans.parent.gameObject.SetActive(value);
	}

	public static void SetParentActive(this Component component, bool value)
	{
		component.transform.parent.gameObject.SetActive(value);
	}

	public static T RandomElement<T>(this List<T> list)
	{
		if (list == null || list.Count == 0)
		{
			return default(T);
		}
		if (list.Count == 1)
		{
			return list[0];
		}
		return list[UnityEngine.Random.Range(0, list.Count)];
	}

	public static TV GetValue<TK, TV>(this Dictionary<TK, TV> dict, TK key, TV defaultValue)
	{
		TV value;
		if (dict.TryGetValue(key, out value))
		{
			return value;
		}
		return defaultValue;
	}

	public static void Shuffle<T>(this List<T> list)
	{
		int num = list.Count;
		while (num > 1)
		{
			num--;
			int index = UnityEngine.Random.Range(0, num + 1);
			T value = list[index];
			list[index] = list[num];
			list[num] = value;
		}
	}

	public static int PositiveMod(this int value, int modVal)
	{
		return (value + modVal) % modVal;
	}
}
