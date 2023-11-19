using UnityEngine;
using System.Collections.Generic;

public class UIProgressBar : UIWidgetContainer
{
	public enum FillDirection
	{
		LeftToRight = 0,
		RightToLeft = 1,
		BottomToTop = 2,
		TopToBottom = 3,
	}

	public Transform thumb;
	[SerializeField]
	protected UIWidget mBG;
	[SerializeField]
	protected UIWidget mFG;
	[SerializeField]
	protected float mValue;
	[SerializeField]
	protected FillDirection mFill;
	public int numberOfSteps;
	public List<EventDelegate> onChange;
}
