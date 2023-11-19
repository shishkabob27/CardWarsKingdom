public class UIStreamingGrid : UIWidgetContainer
{
	public delegate void UpdateFunction();

	public delegate void ScrolFunction(int rowIndex);

	public float cellWidth = 200f;

	public float cellHeight = 200f;

	public float EndPadding;

	private UpdateFunction mUpdateFunction;

	private ScrolFunction mScrollFunction;

	public void SetFunctions(UpdateFunction updateFunction, ScrolFunction scrollFunction)
	{
		mUpdateFunction = updateFunction;
		mScrollFunction = scrollFunction;
	}

	private void Update()
	{
		if (mUpdateFunction != null)
		{
			mUpdateFunction();
		}
	}

	public void SetScrollPos(int rowIndex)
	{
		if (mScrollFunction != null)
		{
			mScrollFunction(rowIndex);
		}
	}
}
