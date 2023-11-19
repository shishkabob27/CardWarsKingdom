using System;
using UnityEngine;

[Serializable]
public class PropertyReference
{
	[SerializeField]
	private Component mTarget;
	[SerializeField]
	private string mName;
}
