using UnityEngine;

public class RandomizedGlowTweens : MonoBehaviour
{
	public AnimationCurve Pattern1;

	public Vector2 P1_ValueRange;

	public Vector2 P1_TimeRange;

	public float P1_Frequency;

	public AnimationCurve Pattern2;

	public Vector2 P2_ValueRange;

	public Vector2 P2_TimeRange;

	public float P2_Frequency;

	public AnimationCurve Pattern3;

	public Vector2 P3_ValueRange;

	public Vector2 P3_TimeRange;

	public float P3_Frequency;

	private AnimationCurve[] patternCurves;

	private Vector2[] valueRanges;

	private Vector2[] timeRanges;

	private void Start()
	{
		patternCurves = new AnimationCurve[3] { Pattern1, Pattern2, Pattern3 };
		valueRanges = new Vector2[3] { P1_ValueRange, P2_ValueRange, P3_ValueRange };
		timeRanges = new Vector2[3] { P1_TimeRange, P2_TimeRange, P3_TimeRange };
		StartRandomTween();
	}

	private void Update()
	{
	}

	private void StartRandomTween()
	{
		int num = 0;
		float max = (P1_Frequency + P2_Frequency + P3_Frequency) * 1f;
		float num2 = Random.Range(0f, max);
		num = ((num2 > P1_Frequency) ? ((!(num2 > P2_Frequency)) ? 1 : 2) : 0);
		TweenAlpha component = base.gameObject.GetComponent<TweenAlpha>();
		if (component != null)
		{
			Object.Destroy(component);
		}
		TweenAlpha tweenAlpha = base.gameObject.AddComponent<TweenAlpha>();
		tweenAlpha.animationCurve = patternCurves[num];
		tweenAlpha.onFinished.Add(new EventDelegate(StartRandomTween));
		tweenAlpha.SetStartToCurrentValue();
		tweenAlpha.to = Random.Range(valueRanges[num].x, valueRanges[num].y);
		tweenAlpha.duration = Random.Range(timeRanges[num].x, timeRanges[num].y);
	}
}
