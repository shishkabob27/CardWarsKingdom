using UnityEngine;
using System.Collections.Generic;

public class TypewriterEffect : MonoBehaviour
{
	public int charsPerSecond;
	public float fadeInTime;
	public float delayOnPeriod;
	public float delayOnNewLine;
	public UIScrollView scrollView;
	public bool keepFullDimensions;
	public List<EventDelegate> onFinished;
}
