using UnityEngine;

public class HandCardDragDropItem : UIDragDropItem
{
	private CardPrefabScript mPrefabScript;

	private void Awake()
	{
		mPrefabScript = GetComponent<CardPrefabScript>();
		if (mPrefabScript == null)
		{
			mPrefabScript = NGUITools.FindInParents<CardPrefabScript>(base.transform.parent);
		}
	}

	protected override void OnDragStart()
	{
		if (mPrefabScript.IsDraggable())
		{
			mPrefabScript.OnCardDragStart();
			base.OnDragStart();
		}
	}

	protected override void OnDragDropStart(GameObject cloneSource = null)
	{
		mCollider.enabled = false;
		mTouchID = UICamera.currentTouchID;
		mParent = mTrans.parent;
		mRoot = NGUITools.FindInParents<UIRoot>(mParent);
	}

	protected override void OnDragDropMove(Vector3 delta)
	{
		mPrefabScript.OnCardMoved();
	}

	protected override void OnDragDropRelease(GameObject surface)
	{
		base.OnDragDropRelease(surface);
		mPrefabScript.OnCardDropped();
	}

	public void ForceDrop()
	{
		OnDragDropRelease(null);
	}
}
