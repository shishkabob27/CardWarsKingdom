namespace Facebook.Unity
{
	internal abstract class MethodCall<T> where T : IResult
	{
		public string MethodName { get; private set; }

		public FacebookDelegate<T> Callback { protected get; set; }

		protected FacebookBase FacebookImpl { get; set; }

		protected MethodArguments Parameters { get; set; }

		public MethodCall(FacebookBase facebookImpl, string methodName)
		{
			Parameters = new MethodArguments();
			FacebookImpl = facebookImpl;
			MethodName = methodName;
		}

		public abstract void Call(MethodArguments args = null);
	}
}
