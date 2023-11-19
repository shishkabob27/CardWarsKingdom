using System;
using UnityEngine;

[Serializable]
public class EventDelegate
{
	[Serializable]
	public class Parameter
	{
		public Object obj;
		public string field;
	}

	[SerializeField]
	private MonoBehaviour mTarget;
	[SerializeField]
	private string mMethodName;
	[SerializeField]
	private Parameter[] mParameters;
	public bool oneShot;
}
