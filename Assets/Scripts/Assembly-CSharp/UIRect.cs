using UnityEngine;
using System;

public class UIRect : MonoBehaviour
{
	[Serializable]
	public class AnchorPoint
	{
		public Transform target;
		public float relative;
		public int absolute;
	}

	public enum AnchorUpdate
	{
		OnEnable = 0,
		OnUpdate = 1,
	}

	public AnchorPoint leftAnchor;
	public AnchorPoint rightAnchor;
	public AnchorPoint bottomAnchor;
	public AnchorPoint topAnchor;
	public AnchorUpdate updateAnchors;
}
