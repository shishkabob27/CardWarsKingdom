using System.Collections;
using UnityEngine;

public class DebugFPSCounter : MonoBehaviour
{
	public float frequency = 0.5f;

	public UISprite bar;

	private UILabel uiLabel;

	private UIAnchor uiAnchor;

	public float FramesPerSec { get; protected set; }

	private void Start()
	{
		uiAnchor = GetComponent<UIAnchor>();
	}

	private void OnEnable()
	{
		uiLabel = base.gameObject.GetComponent<UILabel>();
		StartCoroutine(FPS());
	}

	private IEnumerator FPS()
	{
		while (true)
		{
			int lastFrameCount = Time.frameCount;
			float lastTime = Time.realtimeSinceStartup;
			yield return new WaitForSeconds(frequency);
			float timeSpan = Time.realtimeSinceStartup - lastTime;
			int frameCount = Time.frameCount - lastFrameCount;
			FramesPerSec = (float)frameCount / timeSpan;
			if (uiLabel != null)
			{
				uiLabel.text = string.Format("{0:F1} FPS", FramesPerSec);
			}
			bar.fillAmount = FramesPerSec / 60f;
			if (FramesPerSec < 30f)
			{
				if (uiLabel != null)
				{
					uiLabel.color = Color.yellow;
				}
				if (bar != null)
				{
					bar.color = Color.yellow;
				}
			}
			else if (FramesPerSec < 10f)
			{
				if (uiLabel != null)
				{
					uiLabel.color = Color.red;
				}
				if (bar != null)
				{
					bar.color = Color.red;
				}
			}
			else
			{
				if (uiLabel != null)
				{
					uiLabel.color = Color.cyan;
				}
				if (bar != null)
				{
					bar.color = Color.cyan;
				}
			}
		}
	}

	private void Update()
	{
		if (uiAnchor != null && uiAnchor.uiCamera == null)
		{
			uiAnchor.uiCamera = NGUITools.FindCameraForLayer(base.gameObject.layer);
			uiAnchor.enabled = true;
		}
	}
}
