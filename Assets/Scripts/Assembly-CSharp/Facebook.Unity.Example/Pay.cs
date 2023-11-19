using UnityEngine;

namespace Facebook.Unity.Example
{
	internal class Pay : MenuBase
	{
		private string payProduct = string.Empty;

		protected override void GetGui()
		{
			LabelAndTextField("Product: ", ref payProduct);
			if (Button("Call Pay"))
			{
				CallFBPay();
			}
			GUILayout.Space(10f);
		}

		private void CallFBPay()
		{
			FacebookDelegate<IPayResult> callback = base.HandleResult;
			FB.Canvas.Pay(payProduct, "purchaseitem", 1, null, null, null, null, null, callback);
		}
	}
}
