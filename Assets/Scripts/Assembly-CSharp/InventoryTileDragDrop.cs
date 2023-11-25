using UnityEngine;

public class InventoryTileDragDrop : UIDragDropItem
{
	private InventoryTile mTileScript;

	private void Awake()
	{
		mTileScript = GetComponent<InventoryTile>();
	}

	protected override void OnDragStart()
	{
		if (mTileScript.IsDraggable())
		{
			base.OnDragStart();
			Singleton<SLOTAudioManager>.Instance.PlaySound("ui/SFX_CreatureCardPickup");
		}
	}

	protected override void OnDragDropStart(GameObject cloneSource = null)
	{
		mTileScript.OnDragBegin(cloneSource);
		cloneOnDrag = false;
		base.OnDragDropStart();
	}

	protected override void OnDragDropMove(Vector3 delta)
	{
		if (DetachedSingleton<SceneFlowManager>.Instance.InFrontEnd())
		{
			Vector3 position = Singleton<TownController>.Instance.GetUICam().ScreenToWorldPoint(Input.mousePosition);
			position.z = 0f;
			base.transform.position = position;
		}
	}

	protected override void OnDragDropRelease(GameObject surface)
	{
		mCollider.enabled = true;
		mTouchID = int.MinValue;
		Singleton<SLOTAudioManager>.Instance.PlaySound("ui/SFX_CreatureCardDrop");
		bool flag = true;
		if (surface != null && TutorialController.IsValidDragTarget(surface))
		{
			CreaturePortraitDragTarget component = surface.GetComponent<CreaturePortraitDragTarget>();
			if (component != null)
			{
				flag = !mTileScript.OnDroppedOnTarget(component.SlotIndex);
			}
			else
			{
				InventoryTile component2 = surface.GetComponent<InventoryTile>();
				if (component2 != null && component2.AssignedSlot != -1)
				{
					flag = !mTileScript.OnDroppedOnTarget(component2.AssignedSlot);
				}
				else
				{
					mTileScript.OnDroppedOnTarget(-1);
				}
			}
		}
		else
		{
			mTileScript.OnDroppedOnTarget(-1);
		}
		mTileScript.OnDragFinished();
		if (flag)
		{
			NGUITools.Destroy(base.gameObject);
			Singleton<SLOTAudioManager>.Instance.PlaySound("ui/UI_Gem_Remove");
		}
		else
		{
			mTileScript.SetCreatureToTarget();
		}
	}
}
