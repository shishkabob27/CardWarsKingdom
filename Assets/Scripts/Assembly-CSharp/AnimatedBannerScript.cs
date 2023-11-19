using UnityEngine;

public class AnimatedBannerScript : MonoBehaviour
{
	public float interval;
	public float IntervalDelayRandom;
	public UILabel LabelReference;
	public float TextSoundDelay;
	public GameObject ParticleOnLetter;
	public bool TriggerPrepLabels;
	public Color[] RandomColor;
	public bool UseRandomColor;
	public float IncrementMultiplier;
	public bool DestroyWhenComplete;
	public bool StartBannerAnim;
}
