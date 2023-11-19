using UnityEngine;

public class CreatureDropTile : MonoBehaviour
{
	public UITweenController BasicRevealTween;

	public UITweenController FancyRevealTween;

	public Transform EggSpawnPoint;

	public UITexture Image;

	public UISprite FrameSprite;

	public GameObject EggObj;

	private InventorySlotItem mItem;

	public void Populate(InventorySlotItem item)
	{
		mItem = item;
		if (mItem.SlotType == InventorySlotType.Creature)
		{
			GameObject original = Singleton<PrefabReferences>.Instance.LootChests[mItem.Creature.Form.Rarity - 1];
			EggObj = EggSpawnPoint.InstantiateAsChild(original);
			SLOTGame.SetLayerRecursive(EggObj, LayerMask.NameToLayer("GUI"));
			EggObj.transform.localScale = Vector3.one;
			Image.ReplaceTexture(mItem.Creature.Form.PortraitTexture);
			Image.gameObject.SetActive(false);
			FrameSprite.spriteName = mItem.Creature.Faction.CreaturePortraitFrameSpriteName();
			FrameSprite.gameObject.SetActive(false);
		}
		else if (mItem.SlotType != InventorySlotType.EvoMaterial)
		{
		}
	}

	public bool Open()
	{
		if (!mItem.Creature.Form.AlreadyCollected)
		{
			FancyRevealTween.Play();
			Singleton<GachaOpenSequencer>.Instance.ShowGachaSequence(mItem);
			return true;
		}
		EggObj.SetActive(false);
		BasicRevealTween.Play();
		Image.gameObject.SetActive(true);
		FrameSprite.gameObject.SetActive(true);
		return false;
	}

	public void ShowCreatureIcon()
	{
		Image.gameObject.SetActive(true);
		FrameSprite.gameObject.SetActive(true);
	}

	public void OnFancyTweenFinished()
	{
	}
}
