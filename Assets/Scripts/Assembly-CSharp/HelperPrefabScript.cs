using UnityEngine;

public class HelperPrefabScript : UIStreamingGridListItem
{
	public GameObject mTilePrefabObj;
	public InventoryTile TileScript;
	public UILabel HelperName;
	public UILabel Rank;
	public UILabel Type;
	public UILabel HelpReward;
	public UILabel LastActivity;
	public Transform CreatureTileSpawnNode;
	public GameObject OnlineObject;
	public UILabel OfflineLabel;
	public GameObject HighlightObject;
	public HelperMode Mode;
	public bool IsSelected;
}
