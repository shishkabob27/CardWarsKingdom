using System;
using UnityEngine;

[Serializable]
public class TrashManRecycleBin
{
	public GameObject prefab;
	public int instancesToPreallocate;
	public int instancesToAllocateIfEmpty;
	public bool imposeHardLimit;
	public int hardLimit;
	public bool cullExcessPrefabs;
	public int instancesToMaintainInPool;
	public float cullInterval;
}
