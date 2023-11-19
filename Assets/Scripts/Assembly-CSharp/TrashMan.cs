using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class TrashMan : MonoBehaviour
{
	public static TrashMan instance;

	public List<TrashManRecycleBin> recycleBinCollection;

	private Dictionary<int, TrashManRecycleBin> _instanceIdToRecycleBin = new Dictionary<int, TrashManRecycleBin>();

	private Dictionary<string, int> _poolNameToInstanceId = new Dictionary<string, int>();

	[HideInInspector]
	public new Transform transform;

	private void Awake()
	{
		if (instance != null)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			transform = base.gameObject.transform;
			instance = this;
			initializePrefabPools();
		}
		StartCoroutine(cullExcessObjects());
	}

	private void OnApplicationQuit()
	{
		instance = null;
	}

	private IEnumerator cullExcessObjects()
	{
		WaitForSeconds waiter = new WaitForSeconds(5f);
		while (true)
		{
			for (int i = 0; i < recycleBinCollection.Count; i++)
			{
				recycleBinCollection[i].cullExcessObjects();
			}
			yield return waiter;
		}
	}

	private void initializePrefabPools()
	{
		if (recycleBinCollection == null)
		{
			return;
		}
		foreach (TrashManRecycleBin item in recycleBinCollection)
		{
			if (item != null && !(item.prefab == null))
			{
				item.initialize();
				_instanceIdToRecycleBin.Add(item.prefab.GetInstanceID(), item);
				_poolNameToInstanceId.Add(item.prefab.name, item.prefab.GetInstanceID());
			}
		}
	}

	private static GameObject spawn(int gameObjectInstanceId, Vector3 position, Quaternion rotation)
	{
		if (instance._instanceIdToRecycleBin.ContainsKey(gameObjectInstanceId))
		{
			GameObject gameObject = instance._instanceIdToRecycleBin[gameObjectInstanceId].spawn();
			if (gameObject != null)
			{
				Transform transform = gameObject.transform;
				transform.parent = null;
				transform.position = position;
				transform.rotation = rotation;
				gameObject.SetActive(true);
			}
			return gameObject;
		}
		return null;
	}

	private IEnumerator internalDespawnAfterDelay(GameObject go, float delayInSeconds)
	{
		yield return new WaitForSeconds(delayInSeconds);
		despawn(go);
	}

	public static void AddRecycleBin(TrashManRecycleBin recycleBin)
	{
		if (!instance._poolNameToInstanceId.ContainsKey(recycleBin.prefab.name))
		{
			instance.recycleBinCollection.Add(recycleBin);
			recycleBin.initialize();
			instance._instanceIdToRecycleBin.Add(recycleBin.prefab.GetInstanceID(), recycleBin);
			instance._poolNameToInstanceId.Add(recycleBin.prefab.name, recycleBin.prefab.GetInstanceID());
		}
	}

	public static void RemoveRecycleBin(TrashManRecycleBin recycleBin)
	{
		if (instance._poolNameToInstanceId.ContainsKey(recycleBin.prefab.name))
		{
			instance.recycleBinCollection.Remove(recycleBin);
			instance._instanceIdToRecycleBin.Remove(recycleBin.prefab.GetInstanceID());
			instance._poolNameToInstanceId.Remove(recycleBin.prefab.name);
		}
	}

	public static GameObject spawn(GameObject go, [Optional] Vector3 position, [Optional] Quaternion rotation)
	{
		if (instance._instanceIdToRecycleBin.ContainsKey(go.GetInstanceID()))
		{
			return spawn(go.GetInstanceID(), position, rotation);
		}
		GameObject gameObject = Object.Instantiate(go, position, rotation) as GameObject;
		gameObject.transform.parent = null;
		return gameObject;
	}

	public static GameObject spawn(string gameObjectName, [Optional] Vector3 position, [Optional] Quaternion rotation)
	{
		int value = -1;
		if (instance._poolNameToInstanceId.TryGetValue(gameObjectName, out value))
		{
			return spawn(value, position, rotation);
		}
		return null;
	}

	public static void despawn(GameObject go)
	{
		if (!(go == null))
		{
			string key = go.name;
			if (!instance._poolNameToInstanceId.ContainsKey(key))
			{
				Object.Destroy(go);
				return;
			}
			instance._instanceIdToRecycleBin[instance._poolNameToInstanceId[key]].despawn(go);
			go.transform.parent = instance.transform;
		}
	}

	public static void despawnAfterDelay(GameObject go, float delayInSeconds)
	{
		if (!(go == null))
		{
			instance.StartCoroutine(instance.internalDespawnAfterDelay(go, delayInSeconds));
		}
	}

	public static TrashManRecycleBin recycleBinForGameObjectName(string gameObjectName)
	{
		if (instance._poolNameToInstanceId.ContainsKey(gameObjectName))
		{
			int key = instance._poolNameToInstanceId[gameObjectName];
			return instance._instanceIdToRecycleBin[key];
		}
		return null;
	}

	public static TrashManRecycleBin recycleBinForGameObject(GameObject go)
	{
		if (instance._instanceIdToRecycleBin.ContainsKey(go.GetInstanceID()))
		{
			return instance._instanceIdToRecycleBin[go.GetInstanceID()];
		}
		return null;
	}
}
