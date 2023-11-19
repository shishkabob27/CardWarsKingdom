using System.Collections.Generic;
using UnityEngine;

public class ButtonDepressEffect : MonoBehaviour
{
	public UIButton myButton;

	[Header("Amount to Offset while depressed")]
	public Vector2 pushAmount = new Vector2(0f, -9f);

	[Header("Color objects are multiplied by")]
	public Color multiplyColor = new Color(0.75f, 0.75f, 0.75f, 1f);

	[Header("Object to tint")]
	public List<UISprite> childSprites;

	public List<UILabel> childLabels;

	private List<Color> origSpriteColors;

	private List<Color> origLabelColors;

	private bool isDepressed;

	private UIEventTrigger myEventTrigger;

	private string normalSpriteName;

	private string pressedSpriteName;

	private GameObject spriteTarget;

	private void Start()
	{
		if (myButton == null && base.gameObject.GetComponent<UIButton>() != null)
		{
			myButton = base.gameObject.GetComponent<UIButton>();
		}
		if (!(myButton != null))
		{
			return;
		}
		if (myEventTrigger == null)
		{
			if (base.gameObject.GetComponent<UIEventTrigger>() != null)
			{
				myEventTrigger = base.gameObject.GetComponent<UIEventTrigger>();
			}
			else
			{
				myEventTrigger = base.gameObject.AddComponent<UIEventTrigger>();
			}
		}
		myEventTrigger.onPress.Add(new EventDelegate(Depress));
		myEventTrigger.onRelease.Add(new EventDelegate(UnDepress));
		myEventTrigger.onDragOut.Add(new EventDelegate(UnDepress));
		origSpriteColors = new List<Color>();
		origLabelColors = new List<Color>();
		for (int i = 0; i < childSprites.Count; i++)
		{
			origSpriteColors.Add(childSprites[i].color);
		}
		for (int j = 0; j < childLabels.Count; j++)
		{
			origLabelColors.Add(childLabels[j].color);
		}
		normalSpriteName = myButton.normalSprite;
		pressedSpriteName = myButton.pressedSprite;
		spriteTarget = myButton.tweenTarget;
	}

	private void Depress()
	{
		UISprite component = spriteTarget.gameObject.GetComponent<UISprite>();
		if (component != null && pressedSpriteName != null)
		{
			component.spriteName = pressedSpriteName;
		}
		for (int i = 0; i < childSprites.Count; i++)
		{
			UISprite uISprite = childSprites[i];
			if (uISprite != null)
			{
				Vector3 localPosition = new Vector3(uISprite.gameObject.transform.localPosition.x + pushAmount.x, uISprite.gameObject.transform.localPosition.y + pushAmount.y, uISprite.gameObject.transform.localPosition.z);
				uISprite.gameObject.transform.localPosition = localPosition;
				uISprite.color = origSpriteColors[i] * multiplyColor;
			}
		}
		for (int j = 0; j < childLabels.Count; j++)
		{
			UILabel uILabel = childLabels[j];
			if (uILabel != null)
			{
				Vector3 localPosition2 = new Vector3(uILabel.gameObject.transform.localPosition.x + pushAmount.x, uILabel.gameObject.transform.localPosition.y + pushAmount.y, uILabel.gameObject.transform.localPosition.z);
				uILabel.gameObject.transform.localPosition = localPosition2;
				uILabel.color = origLabelColors[j] * multiplyColor;
			}
		}
		isDepressed = true;
	}

	private void UnDepress()
	{
		if (!isDepressed)
		{
			return;
		}
		UISprite component = spriteTarget.gameObject.GetComponent<UISprite>();
		if (component != null && normalSpriteName != null)
		{
			component.spriteName = normalSpriteName;
		}
		for (int i = 0; i < childSprites.Count; i++)
		{
			UISprite uISprite = childSprites[i];
			if (uISprite != null)
			{
				Vector3 localPosition = new Vector3(uISprite.gameObject.transform.localPosition.x - pushAmount.x, uISprite.gameObject.transform.localPosition.y - pushAmount.y, uISprite.gameObject.transform.localPosition.z);
				uISprite.gameObject.transform.localPosition = localPosition;
				uISprite.color = origSpriteColors[i];
			}
		}
		for (int j = 0; j < childLabels.Count; j++)
		{
			UILabel uILabel = childLabels[j];
			if (uILabel != null)
			{
				Vector3 localPosition2 = new Vector3(uILabel.gameObject.transform.localPosition.x - pushAmount.x, uILabel.gameObject.transform.localPosition.y - pushAmount.y, uILabel.gameObject.transform.localPosition.z);
				uILabel.gameObject.transform.localPosition = localPosition2;
				uILabel.color = origLabelColors[j];
			}
		}
		isDepressed = false;
	}

	public void ResetDepression()
	{
		UnDepress();
	}
}
