using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

internal class CreditsUI : MonoBehaviour
{
	private class APCreditInfo
	{
		[XmlAttribute("Text")]
		public string Text;

		[XmlAttribute("FontSize")]
		public float FontSize = 45f;

		[XmlAttribute("Texture")]
		public string Texture;

		[XmlAttribute("ColorR")]
		public float ColorR = 1f;

		[XmlAttribute("ColorG")]
		public float ColorG = 1f;

		[XmlAttribute("ColorB")]
		public float ColorB = 1f;

		[XmlAttribute("ColorA")]
		public float ColorA = 1f;

		[XmlAttribute("OutlineColorR")]
		public float OutlineColorR;

		[XmlAttribute("OutlineColorG")]
		public float OutlineColorG;

		[XmlAttribute("OutlineColorB")]
		public float OutlineColorB;

		[XmlAttribute("OutlineColorA")]
		public float OutlineColorA = 1f;

		[XmlAttribute("LineSpacing")]
		public float LineSpacing;

		[XmlAttribute("YOffset")]
		public float YOffset;

		[XmlAttribute("TextureWidth")]
		public float TextureWidth;

		[XmlAttribute("TextureHeight")]
		public float TextureHeight;

		public static APCreditInfo FromXmlNode(XmlNode node)
		{
			APCreditInfo aPCreditInfo = new APCreditInfo();
			FieldInfo[] fields = typeof(APCreditInfo).GetFields();
			FieldInfo[] array = fields;
			foreach (FieldInfo fieldInfo in array)
			{
				if (node.Attributes[fieldInfo.Name] != null)
				{
					if (fieldInfo.FieldType == typeof(string))
					{
						fieldInfo.SetValue(aPCreditInfo, node.Attributes[fieldInfo.Name].InnerText);
					}
					else
					{
						fieldInfo.SetValue(aPCreditInfo, XmlBypass.GetFloat(node.Attributes[fieldInfo.Name], 0f));
					}
				}
			}
			return aPCreditInfo;
		}
	}

	[XmlRoot("CreditsList")]
	private class APCreditsList
	{
		[XmlAttribute("ItemSpacing")]
		public float itemSpacing = 20f;

		[XmlArray("Credits")]
		[XmlArrayItem("Credit")]
		public List<APCreditInfo> credits;

		private APCreditsList()
		{
			credits = new List<APCreditInfo>();
		}

		public static APCreditsList FromXmlNode(XmlNode node)
		{
			APCreditsList aPCreditsList = new APCreditsList();
			if (node.Name != "CreditsList")
			{
				return null;
			}
			aPCreditsList.itemSpacing = XmlBypass.GetFloat(node.Attributes["ItemSpacing"], aPCreditsList.itemSpacing);
			XmlElement xmlElement = node["Credits"];
			foreach (XmlNode childNode in xmlElement.ChildNodes)
			{
				if (childNode != null && !(childNode.Name != "Credit"))
				{
					aPCreditsList.credits.Add(APCreditInfo.FromXmlNode(childNode));
				}
			}
			return aPCreditsList;
		}
	}

	private delegate void CreditsEndCallback();

	public float FIRST_ITEM_Y = -40f;

	public GameObject creditLabel;

	public UIScrollView creditsLabelParent;

	public float scrollSpeed = 0.1f;

	private List<GameObject> creditsLabels = new List<GameObject>();

	private CreditsEndCallback creditsEndCallback;

	private bool closed;

	private float scrollHeight;

	private float lastY;

	private float topY;

	private bool initialized;

	private static string CreditsFilename = "Credits/CreditsList";

	private static APCreditsList creditsList;

	private void Awake()
	{
		topY = creditsLabelParent.transform.localPosition.y;
	}

	private static void showobject(GameObject obj, bool show)
	{
		obj.SetActive(show);
	}

	private void ClearCreditsLabels()
	{
		foreach (GameObject creditsLabel in creditsLabels)
		{
			if (creditsLabel != null)
			{
				UnityEngine.Object.DestroyImmediate(creditsLabel);
			}
		}
		creditsLabels.Clear();
	}

	private void Start()
	{
		LoadCreditsList(CreditsFilename);
		if (creditLabel != null)
		{
			creditLabel.SetActive(false);
		}
		Setup(true, CreditsDoneCallback);
		initialized = true;
	}

	private void OnEnable()
	{
		closed = false;
		SetScrollHeight();
		if (creditsLabelParent != null)
		{
			UIPanel component = creditsLabelParent.gameObject.GetComponent<UIPanel>();
			if (component != null && initialized)
			{
				creditsLabelParent.MoveRelative(new Vector3(0f, 0f - creditsLabelParent.transform.localPosition.y, 0f));
			}
		}
	}

	private void SetScrollHeight()
	{
		if (creditsLabelParent != null && creditsLabelParent.panel != null)
		{
			scrollHeight = Mathf.Abs(lastY) + creditsLabelParent.panel.clipRange.w * 2f;
		}
	}

	private float SetupLabel(GameObject instance, string txt, float fontsize, float r, float g, float b, float a, float or, float og, float ob, float oa, float linespacing, float x, float y)
	{
		Vector3 localScale = instance.transform.localScale;
		instance.transform.parent = ((!creditsLabelParent) ? base.gameObject.transform : creditsLabelParent.gameObject.transform);
		instance.transform.localScale = localScale;
		showobject(instance, true);
		UILabel uILabel = instance.GetComponentInChildren(typeof(UILabel)) as UILabel;
		if ((bool)uILabel)
		{
			uILabel.fontSize = (int)fontsize;
			uILabel.color = new Color(r, g, b, a);
			uILabel.effectColor = new Color(or, og, ob, oa);
			uILabel.text = KFFLocalization.Get(txt);
			instance.transform.localPosition = new Vector3(x, y, 0f);
			y -= NGUIMath.CalculateAbsoluteWidgetBounds(uILabel.transform).size.y + linespacing;
		}
		return y;
	}

	private float SetupTexture(GameObject instance, string texName, float w, float h, float r, float g, float b, float a, float linespacing, float x, float y)
	{
		Vector3 localScale = instance.transform.localScale;
		instance.transform.parent = ((!creditsLabelParent) ? base.gameObject.transform : creditsLabelParent.gameObject.transform);
		instance.transform.localScale = localScale;
		showobject(instance, true);
		Texture texture = Resources.Load(texName) as Texture;
		if (texture != null)
		{
			UITexture uITexture = instance.AddComponent(typeof(UITexture)) as UITexture;
			if (uITexture != null)
			{
				try
				{
					uITexture.shader = Shader.Find("Unlit/Transparent Colored");
					uITexture.pivot = UIWidget.Pivot.Top;
					uITexture.gameObject.transform.localScale = new Vector3((!(w > 0f) || !(h > 0f)) ? ((float)texture.width) : w, (!(w > 0f) || !(h > 0f)) ? ((float)texture.height) : h, 1f);
					uITexture.transform.localPosition = new Vector3(x, y, 0f);
					uITexture.color = new Color(r, g, b, a);
					uITexture.mainTexture = texture;
					y -= NGUIMath.CalculateAbsoluteWidgetBounds(uITexture.transform).size.y + linespacing;
					return y;
				}
				catch (NullReferenceException)
				{
					return y;
				}
			}
		}
		return y;
	}

	private void Setup()
	{
		if (creditsLabelParent != null)
		{
			creditsLabelParent.MoveRelative(new Vector3(0f, 0f - creditsLabelParent.transform.localPosition.y, 0f));
		}
		Setup(false, null);
	}

	private void Setup(bool backButtonShown, CreditsEndCallback callback)
	{
		creditsEndCallback = callback;
		ClearCreditsLabels();
		if (creditsLabelParent != null)
		{
			creditsLabelParent.ResetPosition();
		}
		if (creditLabel != null)
		{
			float x = 0f;
			float num = FIRST_ITEM_Y;
			foreach (APCreditInfo credit in creditsList.credits)
			{
				if ((credit.Texture != null && credit.Texture.Length > 0) || (credit.Text != null && credit.Text.Length > 0))
				{
					if (credit.Texture != null && credit.Texture.Length > 0)
					{
						GameObject gameObject = new GameObject();
						if (gameObject != null)
						{
							SLOTGame.SetLayerRecursive(gameObject, base.gameObject.layer);
							creditsLabels.Add(gameObject);
							num = SetupTexture(gameObject, credit.Texture, credit.TextureWidth, credit.TextureHeight, credit.ColorR, credit.ColorG, credit.ColorB, credit.ColorA, credit.LineSpacing, x, num - credit.YOffset);
						}
					}
					else
					{
						GameObject gameObject = UnityEngine.Object.Instantiate(creditLabel);
						if (gameObject != null)
						{
							SLOTGame.SetLayerRecursive(gameObject, base.gameObject.layer);
							creditsLabels.Add(gameObject);
							num = SetupLabel(gameObject, credit.Text, credit.FontSize, credit.ColorR, credit.ColorG, credit.ColorB, credit.ColorA, credit.OutlineColorR, credit.OutlineColorG, credit.OutlineColorB, credit.OutlineColorA, credit.LineSpacing, x, num - credit.YOffset);
						}
					}
				}
				else
				{
					num -= credit.LineSpacing;
				}
				num -= creditsList.itemSpacing;
			}
			lastY = num;
		}
		if (creditsLabelParent != null)
		{
			creditsLabelParent.ResetPosition();
		}
		SetScrollHeight();
	}

	private void CloseScreen()
	{
		if (!closed)
		{
			closed = true;
			ClearCreditsLabels();
		}
	}

	private void backfadeoutcallback()
	{
		if (creditsEndCallback != null)
		{
			MonoBehaviour.print(" Calling credits end call back");
			creditsEndCallback();
		}
	}

	private void Update()
	{
		if (NGUITools.GetActive(base.gameObject) && base.transform.localScale != Vector3.zero && creditsLabelParent != null)
		{
			if (!creditsLabelParent.isDragging)
			{
				float y = scrollSpeed * Time.deltaTime;
				creditsLabelParent.MoveRelative(new Vector3(0f, y, 0f));
			}
			if (creditsLabelParent.transform.localPosition.y + topY > scrollHeight)
			{
				creditsLabelParent.MoveRelative(new Vector3(0f, 0f - creditsLabelParent.transform.localPosition.y, 0f));
			}
			else if (creditsLabelParent.transform.localPosition.y < 0f)
			{
				creditsLabelParent.MoveRelative(new Vector3(0f, 0f - creditsLabelParent.transform.localPosition.y - topY + scrollHeight, 0f));
			}
		}
	}

	private bool LoadCreditsList(string filename)
	{
		TextAsset textAsset = Singleton<SLOTResourceManager>.Instance.LoadResource(filename) as TextAsset;
		if (textAsset != null)
		{
			XmlDocument xmlDocument = XmlBypass.ParseString(textAsset.text);
			XmlNode node = xmlDocument["CreditsList"];
			creditsList = APCreditsList.FromXmlNode(node);
			return true;
		}
		return false;
	}

	private void BackClicked()
	{
		CloseUI();
	}

	private void CloseUI()
	{
		ClearCreditsLabels();
		NGUITools.SetActive(base.gameObject, false);
	}

	private void CreditsDoneCallback()
	{
	}
}
