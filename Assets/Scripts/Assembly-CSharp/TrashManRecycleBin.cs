using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public sealed class TrashManRecycleBin
{
	public GameObject prefab;

	public int instancesToPreallocate = 5;

	public int instancesToAllocateIfEmpty = 1;

	public bool imposeHardLimit;

	public int hardLimit = 5;

	public bool cullExcessPrefabs;

	public int instancesToMaintainInPool = 5;

	public float cullInterval = 10f;

	private Stack<GameObject> _gameObjectPool;

	private float _timeOfLastCull = float.MinValue;

	private int _spawnedInstanceCount;

	public event Action<GameObject> onSpawnedEvent;

	public event Action<GameObject> onDespawnedEvent;

	private void allocateGameObjects(int count)
	{
		if (imposeHardLimit && _gameObjectPool.Count + count > hardLimit)
		{
			count = hardLimit - _gameObjectPool.Count;
		}
		for (int i = 0; i < count; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(prefab.gameObject);
			gameObject.name = prefab.name;
			gameObject.transform.parent = TrashMan.instance.transform;
			gameObject.SetActive(false);
			_gameObjectPool.Push(gameObject);
		}
	}

	private GameObject pop()
	{
		if (imposeHardLimit && _spawnedInstanceCount >= hardLimit)
		{
			return null;
		}
		if (_gameObjectPool.Count > 0)
		{
			_spawnedInstanceCount++;
			return _gameObjectPool.Pop();
		}
		allocateGameObjects(instancesToAllocateIfEmpty);
		return pop();
	}

	public void initialize()
	{
		_gameObjectPool = new Stack<GameObject>(instancesToPreallocate);
		allocateGameObjects(instancesToPreallocate);
	}

	public void cullExcessObjects()
	{
		if (cullExcessPrefabs && _gameObjectPool.Count > instancesToMaintainInPool && Time.time > _timeOfLastCull + cullInterval)
		{
			_timeOfLastCull = Time.time;
			for (int i = instancesToMaintainInPool; i <= _gameObjectPool.Count; i++)
			{
				UnityEngine.Object.Destroy(_gameObjectPool.Pop());
			}
		}
	}

	public GameObject spawn()
	{
		GameObject gameObject = pop();
		if (gameObject != null && this.onSpawnedEvent != null)
		{
			this.onSpawnedEvent(gameObject);
		}
		return gameObject;
	}

	public void despawn(GameObject go)
	{
		go.SetActive(false);
		_spawnedInstanceCount--;
		_gameObjectPool.Push(go);
		if (this.onDespawnedEvent != null)
		{
			this.onDespawnedEvent(go);
		}
	}
}
