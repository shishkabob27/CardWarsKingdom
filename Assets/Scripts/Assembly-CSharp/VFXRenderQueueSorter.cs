using UnityEngine;

public class VFXRenderQueueSorter : MonoBehaviour
{
	public enum RenderSortingType
	{
		FRONT = 0,
		BACK = 1,
	}

	public UIWidget mTarget;
	public int mQueue;
	public int mWidgetQueue;
	public RenderSortingType mType;
	public bool ShouldScaleVFX;
	public bool SetQueManually;
}
