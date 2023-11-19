using System;

namespace DarkTonic.MasterAudio
{
	public class AudioScriptOrder : Attribute
	{
		public int Order;

		public AudioScriptOrder(int order)
		{
			Order = order;
		}
	}
}
