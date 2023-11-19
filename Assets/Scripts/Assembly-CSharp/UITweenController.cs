using UnityEngine;
using System.Collections.Generic;

public class UITweenController : MonoBehaviour
{
	public bool DisableInput;
	public bool ResetOnPlay;
	public bool DisableWhenFinished;
	public bool CallCleanupTweensOnPlay;
	public UITweenController CleanupTween;
	public UITweenController TweenToCancel;
	public Collider MenuStackCloseButton;
	public List<EventDelegate> OnFinished;
}
