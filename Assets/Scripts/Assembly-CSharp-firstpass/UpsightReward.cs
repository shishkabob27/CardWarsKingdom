using System.Collections.Generic;
using UpsightMiniJSON;

public class UpsightReward
{
	public string productIdentifier { get; private set; }

	public int quantity { get; private set; }

	public string signatureData { get; private set; }

	public string billboardScope { get; private set; }

	public static UpsightReward rewardFromJson(string json)
	{
		UpsightReward upsightReward = new UpsightReward();
		upsightReward.populateFromJson(json);
		return upsightReward;
	}

	protected void populateFromJson(string json)
	{
		Dictionary<string, object> dictionary = Json.Deserialize(json) as Dictionary<string, object>;
		if (dictionary != null)
		{
			if (dictionary.ContainsKey("productIdentifier"))
			{
				productIdentifier = dictionary["productIdentifier"].ToString();
			}
			if (dictionary.ContainsKey("quantity"))
			{
				quantity = int.Parse(dictionary["quantity"].ToString());
			}
			if (dictionary.ContainsKey("signatureData"))
			{
				signatureData = Json.Serialize(dictionary["signatureData"]);
			}
			if (dictionary.ContainsKey("billboardScope"))
			{
				billboardScope = dictionary["billboardScope"].ToString();
			}
		}
	}

	public override string ToString()
	{
		return string.Format("[UpsightReward] productIdentifier: {0}, quantity: {1}, signatureData: {2}, billboardScope: {3}", productIdentifier, quantity, signatureData, billboardScope);
	}
}
