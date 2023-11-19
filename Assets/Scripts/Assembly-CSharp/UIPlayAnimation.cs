using UnityEngine;
using AnimationOrTween;
using System.Collections.Generic;

public class UIPlayAnimation : MonoBehaviour
{
	public Animation target;
	public Animator animator;
	public string clipName;
	public Trigger trigger;
	public Direction playDirection;
	public bool resetOnPlay;
	public bool clearSelection;
	public EnableCondition ifDisabledOnPlay;
	public DisableCondition disableWhenFinished;
	public List<EventDelegate> onFinished;
	[SerializeField]
	private GameObject eventReceiver;
	[SerializeField]
	private string callWhenFinished;
}
