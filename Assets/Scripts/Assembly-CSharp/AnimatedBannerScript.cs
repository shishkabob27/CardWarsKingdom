using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class AnimatedBannerScript : MonoBehaviour
{
	private const int MAX_LETTER_COUNT = 20;

	public float interval = 0.3f;

	public float IntervalDelayRandom = 0.2f;

	public UILabel LabelReference;

	public float TextSoundDelay = 0.3f;

	public GameObject ParticleOnLetter;

	private Color originalColor;

	private Bounds mBounds;

	private string mHexValue;

	private string mAnimateStr;

	public bool TriggerPrepLabels;

	public Color[] RandomColor;

	public bool UseRandomColor;

	private List<GameObject> mTextStrings = new List<GameObject>();

	public float IncrementMultiplier = 1f;

	private float TargetWidth;

	public bool DestroyWhenComplete;

	public bool StartBannerAnim;

	private void Awake()
	{
		originalColor = LabelReference.color;
		LabelReference.text = KFFLocalization.Get(LabelReference.text);
		PrepLabels();
	}

	private void RefreshBounds()
	{
		string text = LabelReference.text;
		if (LabelReference.text.StartsWith("["))
		{
			mHexValue = LabelReference.text.Substring(0, 8);
			mAnimateStr = LabelReference.text.Substring(8);
			LabelReference.text = mAnimateStr;
		}
		else
		{
			mHexValue = string.Empty;
			mAnimateStr = string.Empty;
		}
		foreach (GameObject mTextString in mTextStrings)
		{
			mTextString.transform.position = new Vector3(0f, mTextString.transform.position.y, mTextString.transform.position.z);
		}
		LabelReference.enabled = true;
		mBounds = NGUIMath.CalculateRelativeWidgetBounds(LabelReference.transform);
		TargetWidth = mBounds.size.x * IncrementMultiplier;
		LabelReference.enabled = false;
		LabelReference.text = text;
	}

	public void TriggerBannerAnim()
	{
		RefreshBounds();
		StartCoroutine(TextAnim());
	}

	public void PrepLabels()
	{
		for (int i = 0; i < 20; i++)
		{
			GameObject gameObject = base.transform.InstantiateAsChild(LabelReference.gameObject);
			mTextStrings.Add(gameObject);
			UILabel component = gameObject.GetComponent<UILabel>();
			component.pivot = UIWidget.Pivot.Left;
		}
	}

	private IEnumerator TextAnim()
	{
		string str = ((!(mAnimateStr == string.Empty)) ? mAnimateStr : LabelReference.text);
		float x = (0f - TargetWidth) / 2f;
		float spaceBetweenLetters = (TargetWidth - mBounds.size.x) / (float)(str.Length - 1);
		int count = 0;
		string text = str;
		foreach (char ch in text)
		{
			if (count > 19)
			{
				GameObject addLabel = base.transform.InstantiateAsChild(LabelReference.gameObject);
				mTextStrings.Add(addLabel);
			}
			GameObject obj = mTextStrings[count];
			UILabel lb = obj.GetComponent<UILabel>();
			lb.enabled = true;
			lb.text = mHexValue + ch;
			obj.transform.AddLocalPositionX(x);
			SLOTGame.SetLayerRecursive(obj, base.transform.gameObject.layer);
			x += NGUIMath.CalculateRelativeWidgetBounds(obj.transform).size.x + spaceBetweenLetters;
			if (UseRandomColor)
			{
				int rnd = UnityEngine.Random.Range(0, RandomColor.Length);
				lb.color = RandomColor[rnd];
			}
			TweenPosition[] tweens2 = obj.GetComponents<TweenPosition>();
			TweenPosition[] array = tweens2;
			foreach (TweenPosition tw in array)
			{
				tw.from = new Vector3(tw.from.x + x, tw.from.y, tw.from.z);
				tw.to = new Vector3(tw.to.x + x, tw.to.y, tw.to.z);
				tw.ResetToBeginning();
				tw.Play();
			}
			TweenScale[] tweens = obj.GetComponents<TweenScale>();
			TweenScale[] array2 = tweens;
			foreach (TweenScale tw2 in array2)
			{
				tw2.ResetToBeginning();
				tw2.Play();
			}
			TweenAlpha[] tweens3 = obj.GetComponents<TweenAlpha>();
			TweenAlpha[] array3 = tweens3;
			foreach (TweenAlpha tw3 in array3)
			{
				tw3.ResetToBeginning();
				tw3.Play();
			}
			TweenRotation[] tweens4 = obj.GetComponents<TweenRotation>();
			TweenRotation[] array4 = tweens4;
			foreach (TweenRotation tw4 in array4)
			{
				tw4.ResetToBeginning();
				tw4.Play();
			}
			StartCoroutine(PlayTextEffect(obj));
			float rndF = UnityEngine.Random.Range(interval - IntervalDelayRandom, interval + IntervalDelayRandom);
			if (rndF != 0f)
			{
				yield return new WaitForSeconds(rndF);
			}
			count++;
		}
		yield return null;
	}

	private IEnumerator PlayTextEffect(GameObject obj)
	{
		yield return new WaitForSeconds(TextSoundDelay);
		Singleton<SLOTAudioManager>.Instance.PlaySound("gacha/SFX_GachaBannerText");
		GameObject fxObj = obj.InstantiateAsChild(ParticleOnLetter);
		fxObj.ChangeLayer(obj.layer);
	}

	private IEnumerator DestroyAfterTween(GameObject obj)
	{
		float duration = GetLongestTween(obj);
		yield return new WaitForSeconds(duration);
		UnityEngine.Object.Destroy(obj);
	}

	private float GetLongestTween(GameObject obj)
	{
		UITweener[] components = obj.GetComponents<UITweener>();
		float num = 0f;
		UITweener[] array = components;
		foreach (UITweener uITweener in array)
		{
			if (uITweener.duration + uITweener.delay >= num)
			{
				num = uITweener.duration + uITweener.delay;
			}
		}
		return num;
	}

	private Component CopyComponent(Component original, GameObject destination)
	{
		Type type = original.GetType();
		Component component = destination.AddComponent(type);
		FieldInfo[] fields = type.GetFields();
		FieldInfo[] array = fields;
		foreach (FieldInfo fieldInfo in array)
		{
			fieldInfo.SetValue(component, fieldInfo.GetValue(original));
		}
		return component;
	}

	private void Update()
	{
		if (StartBannerAnim)
		{
			TriggerBannerAnim();
			StartBannerAnim = false;
		}
		if (TriggerPrepLabels)
		{
			PrepLabels();
			TriggerPrepLabels = false;
		}
	}
}
