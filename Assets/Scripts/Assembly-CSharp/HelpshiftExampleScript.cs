using System;
using System.Collections.Generic;
using Helpshift;
using HSMiniJSON;
using UnityEngine;
using UnityEngine.UI;

public class HelpshiftExampleScript : MonoBehaviour
{
	private HelpshiftSdk _support;

	public void updateMetaData(string nothing)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("user-level", "21");
		dictionary.Add("hs-tags", new string[1] { "Tag-1" });
		_support.updateMetaData(dictionary);
	}

	public void helpshiftSessionBegan(string message)
	{
	}

	public void helpshiftSessionEnded(string message)
	{
	}

	public void alertToRateAppAction(string result)
	{
	}

	public void didReceiveNotificationCount(string count)
	{
	}

	public void didReceiveInAppNotificationCount(string count)
	{
	}

	public void newConversationStarted(string message)
	{
	}

	public void userRepliedToConversation(string newMessage)
	{
	}

	public void userCompletedCustomerSatisfactionSurvey(string json)
	{
		Dictionary<string, object> dictionary = (Dictionary<string, object>)Json.Deserialize(json);
	}

	private void Start()
	{
		_support = HelpshiftSdk.getInstance();
		_support.install();
		_support.login("identifier", "name", "email");
	}

	public void onShowFAQsClick()
	{
		_support.showFAQs();
	}

	public void onShowConversationClick()
	{
		_support.showConversation();
	}

	public void onShowFAQSectionClick()
	{
		GameObject gameObject = GameObject.FindGameObjectWithTag("faq_section_id");
		InputField component = gameObject.GetComponent<InputField>();
		try
		{
			Convert.ToInt16(component.text);
			_support.showFAQSection(component.text);
		}
		catch (FormatException)
		{
		}
	}

	public void onShowFAQClick()
	{
		GameObject gameObject = GameObject.FindGameObjectWithTag("faq_id");
		InputField component = gameObject.GetComponent<InputField>();
		try
		{
			Convert.ToInt16(component.text);
			_support.showSingleFAQ(component.text);
		}
		catch (FormatException)
		{
		}
	}

	public void onShowReviewReminderClick()
	{
		_support.showAlertToRateAppWithURL("market://details?id=com.RunnerGames.game.YooNinja_Lite");
	}
}
