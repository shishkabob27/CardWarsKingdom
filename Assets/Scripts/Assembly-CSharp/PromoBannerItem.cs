using System;
using UnityEngine;

public class PromoBannerItem : MonoBehaviour
{
	private Action _OnClickedDelegate;

	[SerializeField]
	private UILabel _TitleLabel;

	[SerializeField]
	private UILabel _DescriptionLabel;

	[SerializeField]
	private UILabel _CurrencyLabel;

	[SerializeField]
	private UILabel _ButtonTextLabel;

	[SerializeField]
	private UISprite _CurrencySprite;

	[SerializeField]
	private UIButton _Button;

	[SerializeField]
	private UITexture _BackgroundTexture;

	[SerializeField]
	private Color[] _ButtonLabelOutlineColors = new Color[3];

	private DateTime endTime;

	private bool endTimeUsed;

	private bool endTimeUsed24;

	private void Update()
	{
		if ((endTimeUsed || endTimeUsed24) && _CurrencyLabel != null)
		{
			string text = string.Empty;
			if (endTimeUsed)
			{
				text = convertEndDateToTimeLeft(endTime);
			}
			else if (endTimeUsed24)
			{
				text = getTimeLeft24Hours();
			}
			_CurrencyLabel.text = text;
		}
	}

	public void Init(PromoBannerData inData)
	{
		if (_TitleLabel != null)
		{
			_TitleLabel.text = ((!inData.ShouldLocalizeText) ? inData.TitleText : KFFLocalization.Get(inData.TitleText));
			_TitleLabel.color = inData.TitleTextColor;
			_TitleLabel.effectColor = inData.TitleTextOutlineColor;
			_TitleLabel.width = inData.TitleTextWidth;
			if (_TitleLabel.LabelShadow != null)
			{
				_TitleLabel.LabelShadow.RefreshShadowLabel();
			}
		}
		if (_DescriptionLabel != null)
		{
			_DescriptionLabel.text = ((!inData.ShouldLocalizeText) ? inData.DescriptionText : KFFLocalization.Get(inData.DescriptionText));
			_DescriptionLabel.color = inData.DescriptionTextColor;
			_DescriptionLabel.effectColor = inData.DescriptionTextOutlineColor;
			_DescriptionLabel.width = inData.DescriptionTextWidth;
			if (_DescriptionLabel.LabelShadow != null)
			{
				_DescriptionLabel.LabelShadow.RefreshShadowLabel();
			}
		}
		endTimeUsed = false;
		endTimeUsed24 = false;
		if (_CurrencyLabel != null)
		{
			string text = inData.CurrencyText;
			if (text.Trim() == "[TIME_LEFT]")
			{
				endTime = inData.BannerEndDate;
				text = convertEndDateToTimeLeft(endTime);
				endTimeUsed = true;
			}
			else if (text.Trim() == "[TIME_LEFT_24]")
			{
				text = getTimeLeft24Hours();
				endTimeUsed24 = true;
			}
			_CurrencyLabel.text = text;
			_CurrencyLabel.color = inData.CurrencyTextColor;
			_CurrencyLabel.effectColor = inData.CurrencyTextOutlineColor;
			_CurrencyLabel.width = inData.CurrencyTextWidth;
			if (_CurrencyLabel.LabelShadow != null)
			{
				_CurrencyLabel.LabelShadow.RefreshShadowLabel();
			}
		}
		if (_ButtonTextLabel != null)
		{
			_ButtonTextLabel.text = ((!inData.ShouldLocalizeText) ? inData.ButtonText : KFFLocalization.Get(inData.ButtonText));
		}
		if (_CurrencySprite != null)
		{
			_CurrencySprite.spriteName = "Icon_Currency_" + inData.CurrencyType;
			_CurrencySprite.gameObject.SetActive(inData.CurrencyType != PromoCurrencyType.None);
		}
		if (_BackgroundTexture != null)
		{
			_BackgroundTexture.ReplaceTexture(inData.BgTextureName);
		}
		_OnClickedDelegate = inData.OnClickedDelegate;
		if (!(_Button != null))
		{
			return;
		}
		bool flag = true;
		if (flag)
		{
			UIEventTrigger component = GetComponent<UIEventTrigger>();
			if (component != null)
			{
				UnityEngine.Object.Destroy(component);
			}
			UIButtonScale component2 = GetComponent<UIButtonScale>();
			if (component2 != null)
			{
				UnityEngine.Object.Destroy(component2);
			}
			UIPlaySound[] components = GetComponents<UIPlaySound>();
			UIPlaySound[] array = components;
			foreach (UIPlaySound obj in array)
			{
				UnityEngine.Object.Destroy(obj);
			}
		}
		_Button.gameObject.SetActive(flag);
		if (inData.ButtonColor == Color.green)
		{
			_Button.normalSprite = "UI_Action_Button_Green";
			_Button.pressedSprite = "UI_Action_Button_Green_Pressed";
		}
		else if (inData.ButtonColor == Color.blue)
		{
			_Button.normalSprite = "UI_Action_Button_Blue";
			_Button.pressedSprite = "UI_Action_Button_Blue_Pressed";
		}
		else if (inData.ButtonColor == Color.red)
		{
			_Button.normalSprite = "UI_Action_Button_Red";
			_Button.pressedSprite = "UI_Action_Button_Red_Pressed";
		}
		else
		{
			_Button.normalSprite = "UI_Action_Button_Green";
			_Button.pressedSprite = "UI_Action_Button_Green_Pressed";
		}
		Color effectColor = Color.Lerp(inData.ButtonColor, Color.black, 0.7f);
		_ButtonTextLabel.effectColor = effectColor;
		if (_ButtonTextLabel.LabelShadow != null)
		{
			_ButtonTextLabel.LabelShadow.RefreshShadowLabel();
		}
	}

	public void HandleButtonPress()
	{
		if (_OnClickedDelegate != null)
		{
			_OnClickedDelegate();
		}
	}

	public string convertEndDateToTimeLeft(DateTime BannerEndDate)
	{
		TimeSpan ts = new TimeSpan(23, 59, 59);
		TimeSpan timeSpan = BannerEndDate.Subtract(DateTime.Now).Add(ts);
		return "Ending in " + timeSpan.Days + "d" + timeSpan.Hours + "h" + timeSpan.Minutes + "m";
	}

	public string getTimeLeft24Hours()
	{
		TimeSpan timeSpan = DateTime.Today.AddDays(1.0).Subtract(DateTime.Now);
		string text = "Ending in ";
		if (timeSpan.Days > 0)
		{
			return text + timeSpan.Days + "d" + timeSpan.Hours + "h" + timeSpan.Minutes + "m";
		}
		if (timeSpan.Hours > 0)
		{
			return text + timeSpan.Hours + "h" + timeSpan.Minutes + "m";
		}
		return text + timeSpan.Minutes + "m";
	}
}
