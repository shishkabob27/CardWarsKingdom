using System;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	[Serializable]
	public struct ObscuredVector3
	{
		[Serializable]
		public struct RawEncryptedVector3
		{
			public int x;
			public int y;
			public int z;
		}

		private ObscuredVector3(ObscuredVector3.RawEncryptedVector3 encrypted) : this()
		{
		}

		[SerializeField]
		private int currentCryptoKey;
		[SerializeField]
		private RawEncryptedVector3 hiddenValue;
		[SerializeField]
		private Vector3 fakeValue;
		[SerializeField]
		private bool inited;
	}
}
