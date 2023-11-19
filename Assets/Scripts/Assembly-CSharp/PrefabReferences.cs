using UnityEngine;
using System.Collections.Generic;

public class PrefabReferences : Singleton<PrefabReferences>
{
	public GameObject InventoryTile;
	public GameObject Card;
	public List<GameObject> LootChests;
	public List<GameObject> GachaChests;
	public List<GameObject> GachaKeys;
	public UITweenController BounceTween;
}
