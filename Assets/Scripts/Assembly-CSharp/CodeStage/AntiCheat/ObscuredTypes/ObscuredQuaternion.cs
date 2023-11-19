using System;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	[Serializable]
	public struct ObscuredQuaternion
	{
		[Serializable]
		public struct RawEncryptedQuaternion
		{
			public int x;
			public int y;
			public int z;
			public int w;
		}

		private ObscuredQuaternion(ObscuredQuaternion.RawEncryptedQuaternion value) : this()
		{
		}

		[SerializeField]
		private int currentCryptoKey;
		[SerializeField]
		private RawEncryptedQuaternion hiddenValue;
		[SerializeField]
		private Quaternion fakeValue;
		[SerializeField]
		private bool inited;
	}
}
