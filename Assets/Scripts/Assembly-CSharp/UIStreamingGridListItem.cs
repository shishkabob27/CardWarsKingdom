using UnityEngine;

public abstract class UIStreamingGridListItem : MonoBehaviour
{
	public UIStreamingGrid ParentGrid { get; set; }

	public abstract void Populate(object dataObj);

	public virtual void Unload()
	{
	}

	private void OnDestroy()
	{
		Unload();
	}
}
