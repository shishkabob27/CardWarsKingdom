using UnityEngine;

public class UIGrid : UIWidgetContainer
{
	public enum Arrangement
	{
		Horizontal = 0,
		Vertical = 1,
	}

	public enum Sorting
	{
		None = 0,
		Alphabetic = 1,
		Horizontal = 2,
		Vertical = 3,
		Custom = 4,
	}

	public Arrangement arrangement;
	public Sorting sorting;
	public UIWidget.Pivot pivot;
	public int maxPerLine;
	public float cellWidth;
	public float cellHeight;
	public float sizeLimit;
	public bool animateSmoothly;
	public bool hideInactive;
	public bool keepWithinPanel;
	[SerializeField]
	private bool sorted;
}
