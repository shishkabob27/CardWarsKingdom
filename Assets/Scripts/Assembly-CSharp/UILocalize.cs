using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/Localize")]
[RequireComponent(typeof(UIWidget))]
public class UILocalize : MonoBehaviour
{
	public string key;

	private bool mStarted;

	public string value
	{
		set
		{
			if (string.IsNullOrEmpty(value))
			{
				return;
			}
			UIWidget component = GetComponent<UIWidget>();
			UILabel uILabel = component as UILabel;
			UISprite uISprite = component as UISprite;
			if (uILabel != null)
			{
				UIInput uIInput = NGUITools.FindInParents<UIInput>(uILabel.gameObject);
				if (uIInput != null && uIInput.label == uILabel)
				{
					uIInput.defaultText = value;
				}
				else
				{
					uILabel.text = value;
				}
			}
			else if (uISprite != null)
			{
				uISprite.spriteName = value;
				uISprite.MakePixelPerfect();
			}
		}
	}

	private void OnEnable()
	{
		if (mStarted)
		{
			OnLocalize();
		}
	}

	private void Start()
	{
		mStarted = true;
		OnLocalize();
	}

	private void OnLocalize()
	{
		if (string.IsNullOrEmpty(key))
		{
			UILabel component = GetComponent<UILabel>();
			if (component != null)
			{
				key = component.text;
			}
		}
		if (!string.IsNullOrEmpty(key))
		{
			value = Localization.Get(key);
		}
	}
}
