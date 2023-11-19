using UnityEngine;

public class UITextList : MonoBehaviour
{
	public enum Style
	{
		Text = 0,
		Chat = 1,
	}

	public UILabel textLabel;
	public UIProgressBar scrollBar;
	public Style style;
	public int paragraphHistory;
}
