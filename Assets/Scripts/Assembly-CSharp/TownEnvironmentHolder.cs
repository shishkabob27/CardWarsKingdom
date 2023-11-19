using UnityEngine;

public class TownEnvironmentHolder : Singleton<TownEnvironmentHolder>
{
	public bool Loaded { get; private set; }

	private void Awake()
	{
		Resources.UnloadUnusedAssets();
		CheckLoad();
	}

	private void Update()
	{
		CheckLoad();
	}

	private void CheckLoad()
	{
		if (!Loaded && !(SessionManager.Instance == null) && SessionManager.Instance.IsLoadDataDone() && Singleton<PlayerInfoScript>.Instance.IsInitialized)
		{
			Loaded = true;
			string townPrefab = TownScheduleDataManager.Instance.GetCurrentScheduledTownData().TownPrefab;
			base.transform.InstantiateAsChild((GameObject)Singleton<SLOTResourceManager>.Instance.LoadResource(townPrefab));
		}
	}
}
