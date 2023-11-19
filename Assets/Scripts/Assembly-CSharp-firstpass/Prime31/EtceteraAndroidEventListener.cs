using System.Collections.Generic;
using UnityEngine;

namespace Prime31
{
	public class EtceteraAndroidEventListener : MonoBehaviour
	{
		private void OnEnable()
		{
			EtceteraAndroidManager.alertButtonClickedEvent += alertButtonClickedEvent;
			EtceteraAndroidManager.alertCancelledEvent += alertCancelledEvent;
			EtceteraAndroidManager.promptFinishedWithTextEvent += promptFinishedWithTextEvent;
			EtceteraAndroidManager.promptCancelledEvent += promptCancelledEvent;
			EtceteraAndroidManager.twoFieldPromptFinishedWithTextEvent += twoFieldPromptFinishedWithTextEvent;
			EtceteraAndroidManager.twoFieldPromptCancelledEvent += twoFieldPromptCancelledEvent;
			EtceteraAndroidManager.webViewCancelledEvent += webViewCancelledEvent;
			EtceteraAndroidManager.inlineWebViewJSCallbackEvent += inlineWebViewJSCallbackEvent;
			EtceteraAndroidManager.albumChooserCancelledEvent += albumChooserCancelledEvent;
			EtceteraAndroidManager.albumChooserSucceededEvent += albumChooserSucceededEvent;
			EtceteraAndroidManager.photoChooserCancelledEvent += photoChooserCancelledEvent;
			EtceteraAndroidManager.photoChooserSucceededEvent += photoChooserSucceededEvent;
			EtceteraAndroidManager.videoRecordingCancelledEvent += videoRecordingCancelledEvent;
			EtceteraAndroidManager.videoRecordingSucceededEvent += videoRecordingSucceededEvent;
			EtceteraAndroidManager.ttsInitializedEvent += ttsInitializedEvent;
			EtceteraAndroidManager.ttsFailedToInitializeEvent += ttsFailedToInitializeEvent;
			EtceteraAndroidManager.askForReviewDontAskAgainEvent += askForReviewDontAskAgainEvent;
			EtceteraAndroidManager.askForReviewRemindMeLaterEvent += askForReviewRemindMeLaterEvent;
			EtceteraAndroidManager.askForReviewWillOpenMarketEvent += askForReviewWillOpenMarketEvent;
			EtceteraAndroidManager.notificationReceivedEvent += notificationReceivedEvent;
			EtceteraAndroidManager.contactsLoadedEvent += contactsLoadedEvent;
			EtceteraAndroidManager.onRequestPermissionsResultEvent += onRequestPermissionsResultEvent;
		}

		private void OnDisable()
		{
			EtceteraAndroidManager.alertButtonClickedEvent -= alertButtonClickedEvent;
			EtceteraAndroidManager.alertCancelledEvent -= alertCancelledEvent;
			EtceteraAndroidManager.promptFinishedWithTextEvent -= promptFinishedWithTextEvent;
			EtceteraAndroidManager.promptCancelledEvent -= promptCancelledEvent;
			EtceteraAndroidManager.twoFieldPromptFinishedWithTextEvent -= twoFieldPromptFinishedWithTextEvent;
			EtceteraAndroidManager.twoFieldPromptCancelledEvent -= twoFieldPromptCancelledEvent;
			EtceteraAndroidManager.webViewCancelledEvent -= webViewCancelledEvent;
			EtceteraAndroidManager.inlineWebViewJSCallbackEvent -= inlineWebViewJSCallbackEvent;
			EtceteraAndroidManager.albumChooserCancelledEvent -= albumChooserCancelledEvent;
			EtceteraAndroidManager.albumChooserSucceededEvent -= albumChooserSucceededEvent;
			EtceteraAndroidManager.photoChooserCancelledEvent -= photoChooserCancelledEvent;
			EtceteraAndroidManager.photoChooserSucceededEvent -= photoChooserSucceededEvent;
			EtceteraAndroidManager.videoRecordingCancelledEvent -= videoRecordingCancelledEvent;
			EtceteraAndroidManager.videoRecordingSucceededEvent -= videoRecordingSucceededEvent;
			EtceteraAndroidManager.ttsInitializedEvent -= ttsInitializedEvent;
			EtceteraAndroidManager.ttsFailedToInitializeEvent -= ttsFailedToInitializeEvent;
			EtceteraAndroidManager.askForReviewDontAskAgainEvent -= askForReviewDontAskAgainEvent;
			EtceteraAndroidManager.askForReviewRemindMeLaterEvent -= askForReviewRemindMeLaterEvent;
			EtceteraAndroidManager.askForReviewWillOpenMarketEvent -= askForReviewWillOpenMarketEvent;
			EtceteraAndroidManager.notificationReceivedEvent -= notificationReceivedEvent;
			EtceteraAndroidManager.contactsLoadedEvent -= contactsLoadedEvent;
			EtceteraAndroidManager.onRequestPermissionsResultEvent -= onRequestPermissionsResultEvent;
		}

		private void alertButtonClickedEvent(string positiveButton)
		{
		}

		private void alertCancelledEvent()
		{
		}

		private void promptFinishedWithTextEvent(string param)
		{
		}

		private void promptCancelledEvent()
		{
		}

		private void twoFieldPromptFinishedWithTextEvent(string text1, string text2)
		{
		}

		private void twoFieldPromptCancelledEvent()
		{
		}

		private void webViewCancelledEvent()
		{
		}

		private void inlineWebViewJSCallbackEvent(string message)
		{
		}

		private void albumChooserCancelledEvent()
		{
		}

		private void albumChooserSucceededEvent(string imagePath)
		{
		}

		private void photoChooserCancelledEvent()
		{
		}

		private void photoChooserSucceededEvent(string imagePath)
		{
		}

		private void videoRecordingCancelledEvent()
		{
		}

		private void videoRecordingSucceededEvent(string path)
		{
		}

		private void ttsInitializedEvent()
		{
		}

		private void ttsFailedToInitializeEvent()
		{
		}

		private void askForReviewDontAskAgainEvent()
		{
		}

		private void askForReviewRemindMeLaterEvent()
		{
		}

		private void askForReviewWillOpenMarketEvent()
		{
		}

		private void notificationReceivedEvent(string extraData)
		{
		}

		private void contactsLoadedEvent(List<EtceteraAndroid.Contact> contacts)
		{
			Utils.logObject(contacts);
		}

		private void onRequestPermissionsResultEvent(EtceteraAndroidManager.PermissionsResult result)
		{
			Utils.logObject(result);
		}
	}
}
