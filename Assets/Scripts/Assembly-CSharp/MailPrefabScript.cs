using System.Collections.Generic;

public class MailPrefabScript : UIStreamingGridListItem
{
	public UILabel Description;

	public UILabel Body;

	public UISprite EnvelopeOpened;

	public UISprite EnvelopeClosed;

	public UISprite ReadCheckmark;

	public MailItem Mail { get; set; }

	public override void Populate(object dataObj)
	{
		Mail = dataObj as MailItem;
		Description.text = Mail.MailTitle;
		Body.text = Mail.MailBody;
		RefreshOverlay();
		CloseEnvelope();
	}

	public void RefreshOverlay()
	{
		if (Mail != null)
		{
			if (Mail.Opened)
			{
				ReadCheckmark.gameObject.SetActive(true);
			}
			else
			{
				ReadCheckmark.gameObject.SetActive(false);
			}
			if (Mail.MailType == MailType.Scheduled)
			{
			}
			Singleton<PlayerInfoScript>.Instance.UpdateAllBadgeCounts();
		}
	}

	public void CloseEnvelope()
	{
		EnvelopeClosed.gameObject.SetActive(true);
		EnvelopeOpened.gameObject.SetActive(false);
	}

	public void CloseAllEnvelopes()
	{
		MailPrefabScript[] componentsInChildren = base.gameObject.transform.parent.gameObject.GetComponentsInChildren<MailPrefabScript>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].CloseEnvelope();
		}
	}

	public void ReadMail()
	{
		PlayerInfoScript instance = Singleton<PlayerInfoScript>.Instance;
		CloseAllEnvelopes();
		EnvelopeClosed.gameObject.SetActive(false);
		EnvelopeOpened.gameObject.SetActive(true);
		int num = 0;
		int rewardedCurrencyTotal = 0;
		string rewardedCurrencySprite = null;
		InventorySlotItem rewardedItem = null;
		int num2 = 0;
		if (Mail.MailType == MailType.Scheduled || Mail.MailType == MailType.AdminMessage)
		{
			if (!Mail.Rewarded)
			{
				if (Mail.SoftQuantity > 0)
				{
					instance.SaveData.SoftCurrency += Mail.SoftQuantity;
					num = Mail.SoftQuantity;
					rewardedCurrencyTotal = instance.SaveData.SoftCurrency;
					rewardedCurrencySprite = "Icon_Currency_Soft";
					string upsightEvent = "Economy.CoinEnter.MailGift";
					Dictionary<string, object> dictionary = new Dictionary<string, object>();
					dictionary.Add("mailID", Mail.ID);
					dictionary.Add("amount", num);
					Singleton<KFFUpsightVGController>.Instance.RecordCustomEvent(upsightEvent, dictionary);
				}
				if (Mail.PVPQuantity > 0)
				{
					instance.SaveData.PvPCurrency += Mail.PVPQuantity;
					num = Mail.PVPQuantity;
					rewardedCurrencyTotal = instance.SaveData.PvPCurrency;
					rewardedCurrencySprite = "Icon_Currency_PVPCurrency";
					string upsightEvent2 = "Economy.WishboneEnter.MailGift";
					Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
					dictionary2.Add("mailID", Mail.ID);
					dictionary2.Add("amount", num);
					Singleton<KFFUpsightVGController>.Instance.RecordCustomEvent(upsightEvent2, dictionary2);
				}
				if (Mail.HardQuantity > 0)
				{
					Singleton<PlayerInfoScript>.Instance.AddHardCurrency2(0, Mail.HardQuantity, "mail", -1, string.Empty);
					num = Mail.HardQuantity;
					rewardedCurrencyTotal = instance.SaveData.HardCurrency;
					rewardedCurrencySprite = "Icon_Currency_Hard";
					string upsightEvent3 = "Economy.GemEnter.MailGift";
					Dictionary<string, object> dictionary3 = new Dictionary<string, object>();
					dictionary3.Add("mailID", Mail.ID);
					dictionary3.Add("amount", num);
					Singleton<KFFUpsightVGController>.Instance.RecordCustomEvent(upsightEvent3, dictionary3);
				}
				if (Mail.XPMaterialID != null && Mail.XPMaterialQuantity > 0)
				{
					XPMaterialData data = XPMaterialDataManager.Instance.GetData(Mail.XPMaterialID);
					if (data != null)
					{
						for (int i = 0; i < Mail.XPMaterialQuantity; i++)
						{
							rewardedItem = Singleton<PlayerInfoScript>.Instance.SaveData.AddXPMaterial(data);
						}
						num2 = Mail.XPMaterialQuantity;
						string iD = data.ID;
						string upsightEvent4 = "Ingredients.Acquired";
						Dictionary<string, object> dictionary4 = new Dictionary<string, object>();
						dictionary4.Add("ingredientID", iD);
						dictionary4.Add("type", InventorySlotType.XPMaterial.ToString());
						dictionary4.Add("amount", num2);
						dictionary4.Add("source", "Mail");
						dictionary4.Add("sourceID", Mail.ID);
						Singleton<KFFUpsightVGController>.Instance.RecordCustomEvent(upsightEvent4, dictionary4);
					}
					if (Mail.CreatureID != null)
					{
						CreatureData data2 = CreatureDataManager.Instance.GetData(Mail.CreatureID);
						if (data2 != null)
						{
							CreatureItem creatureItem = new CreatureItem(data2);
							rewardedItem = Singleton<PlayerInfoScript>.Instance.SaveData.AddCreature(creatureItem);
							string upsightEvent5 = "Creatures.CreatureAcquired";
							string value = creatureItem.ToString();
							Dictionary<string, object> dictionary5 = new Dictionary<string, object>();
							dictionary5.Add("creatureID", value);
							dictionary5.Add("acquisition", "Mail");
							dictionary5.Add("id", Mail.ID);
							Singleton<KFFUpsightVGController>.Instance.RecordCustomEvent(upsightEvent5, dictionary5);
						}
					}
				}
				Mail.Opened = true;
				Mail.Rewarded = true;
				instance.Save();
				RefreshOverlay();
			}
			Singleton<MailController>.Instance.ShowMailPreview(Mail.MailTitle, Mail.MailBody, num, rewardedCurrencyTotal, rewardedCurrencySprite, rewardedItem, num2, DeleteMail);
		}
		else if (Mail.MailType == MailType.AllyInvite)
		{
			Singleton<MailController>.Instance.ClearMailPreview();
			string title = KFFLocalization.Get("!!REGISTER_HELPER");
			int helpPointForExplorer = MiscParams.HelpPointForExplorer;
			string body = KFFLocalization.Get("!!HELPER_REQUESTMESSAGE");
			Singleton<HelperRequestController>.Instance.CurrentHelper = Mail.Inviter;
			Singleton<HelperRequestController>.Instance.ShowAllyInviteAcceptConfirm(title, body, Singleton<HelperRequestController>.Instance.SendAllyInviteAccept, Singleton<HelperRequestController>.Instance.SendAllyInviteReject);
			string upsightEvent6 = "Economy.WishboneEnter.MailGift";
			Dictionary<string, object> dictionary6 = new Dictionary<string, object>();
			dictionary6.Add("mailID", Mail.ID);
			dictionary6.Add("amount", helpPointForExplorer.ToString());
			Singleton<KFFUpsightVGController>.Instance.RecordCustomEvent(upsightEvent6, dictionary6);
		}
	}

	private void DeleteMail()
	{
		if (Mail.ID != string.Empty)
		{
			Singleton<PlayerInfoScript>.Instance.SaveData.DeleteMail(Mail.ID);
		}
		Singleton<PlayerInfoScript>.Instance.SaveData.RemoveMail(Mail);
		Singleton<MailController>.Instance.DeleteMail(Mail);
		Singleton<PlayerInfoScript>.Instance.Save();
		Singleton<SocialController>.Instance.RefreshCurrentTab();
	}
}
