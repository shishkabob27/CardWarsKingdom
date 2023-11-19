using System;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	[Serializable]
	public struct ObscuredFloat
	{
		private ObscuredFloat(byte[] value) : this()
		{
		}

		[SerializeField]
		private int currentCryptoKey;
		[SerializeField]
		private byte[] hiddenValue;
		[SerializeField]
		private float fakeValue;
		[SerializeField]
		private bool inited;
	}
}
