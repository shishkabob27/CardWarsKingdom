using System;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	[Serializable]
	public struct ObscuredDouble
	{
		private ObscuredDouble(byte[] value) : this()
		{
		}

		[SerializeField]
		private long currentCryptoKey;
		[SerializeField]
		private byte[] hiddenValue;
		[SerializeField]
		private double fakeValue;
		[SerializeField]
		private bool inited;
	}
}
