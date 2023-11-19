using UnityEngine;

public class SLOTGame : Singleton<SLOTGame>
{
	public enum AssetBundleType
	{
		DownloadFromServer = 0,
		Local = 1,
		Disabled = 2,
	}

	public AssetBundleType assetBundleType;
	public GameObject busyIcon;
	public Camera busyIconCamera;
	public GameObject errorPopupPrefab;
	public GameObject errorPopupPrefab_lowres;
}
