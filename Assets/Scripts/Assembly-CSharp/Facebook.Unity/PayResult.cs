namespace Facebook.Unity
{
	internal class PayResult : ResultBase, IPayResult, IResult
	{
		internal PayResult(string result)
			: base(result)
		{
		}
	}
}
