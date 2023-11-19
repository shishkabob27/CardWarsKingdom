using System.Collections.Generic;
using UnityEngine;

public class PrefabReferences : Singleton<PrefabReferences>
{
	public GameObject InventoryTile;

	public GameObject Card;

	public List<GameObject> LootChests;

	public List<GameObject> GachaChests;

	public List<GameObject> GachaKeys;

	public UITweenController BounceTween;
}
