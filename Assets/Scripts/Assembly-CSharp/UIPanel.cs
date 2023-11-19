using UnityEngine;

public class UIPanel : UIRect
{
	public enum RenderQueue
	{
		Automatic = 0,
		StartAt = 1,
		Explicit = 2,
	}

	public bool showInPanelTool;
	public bool generateNormals;
	public bool widgetsAreStatic;
	public bool cullWhileDragging;
	public bool alwaysOnScreen;
	public bool anchorOffset;
	public RenderQueue renderQueue;
	public int startingRenderQueue;
	[SerializeField]
	private float mAlpha;
	[SerializeField]
	private UIDrawCall.Clipping mClipping;
	[SerializeField]
	private Vector4 mClipRange;
	[SerializeField]
	private Vector2 mClipSoftness;
	[SerializeField]
	private int mDepth;
	[SerializeField]
	private int mSortingOrder;
	[SerializeField]
	private Vector2 mClipOffset;
}
