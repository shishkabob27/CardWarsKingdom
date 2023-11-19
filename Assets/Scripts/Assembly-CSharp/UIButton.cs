using UnityEngine;
using System.Collections.Generic;

public class UIButton : UIButtonColor
{
	public bool dragHighlight;
	public string hoverSprite;
	public string pressedSprite;
	public string disabledSprite;
	public Sprite hoverSprite2D;
	public Sprite pressedSprite2D;
	public Sprite disabledSprite2D;
	public bool pixelSnap;
	public List<EventDelegate> onClick;
}
