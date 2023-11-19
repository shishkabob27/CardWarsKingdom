using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace Prime31
{
	public class EtceteraUIManager : MonoBehaviourGUI
	{
		public GameObject testPlane;

		private void Start()
		{
			EtceteraAndroid.initTTS();
			EtceteraAndroid.setAlertDialogTheme(3);
		}

		private void OnEnable()
		{
			EtceteraAndroidManager.albumChooserSucceededEvent += imageLoaded;
			EtceteraAndroidManager.photoChooserSucceededEvent += imageLoaded;
		}

		private void OnDisable()
		{
			EtceteraAndroidManager.albumChooserSucceededEvent -= imageLoaded;
			EtceteraAndroidManager.photoChooserSucceededEvent -= imageLoaded;
		}

		private IEnumerator saveScreenshotToSDCard(Action<string> completionHandler)
		{
			yield return new WaitForEndOfFrame();
			Texture2D tex = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
			tex.ReadPixels(new Rect(0f, 0f, Screen.width, Screen.height), 0, 0, false);
			byte[] bytes = tex.EncodeToPNG();
			UnityEngine.Object.Destroy(tex);
			string path = Path.Combine(Application.persistentDataPath, "myImage.png");
			File.WriteAllBytes(path, bytes);
			completionHandler(path);
		}

		private void OnGUI()
		{
			beginColumn();
			if (GUILayout.Button("Show Toast"))
			{
				EtceteraAndroid.showToast("Hi. Something just happened in the game and I want to tell you but not interrupt you", true);
			}
			if (GUILayout.Button("Play Video"))
			{
				EtceteraAndroid.playMovie("http://techslides.com/demos/sample-videos/small.3gp", 16711680u, false, EtceteraAndroid.ScalingMode.AspectFit, true);
			}
			if (GUILayout.Button("Show Alert"))
			{
				EtceteraAndroid.showAlert("Alert Title Here", "Something just happened.  Do you want to have a snack?", "Yes", "Not Now");
			}
			if (GUILayout.Button("Single Field Prompt"))
			{
				EtceteraAndroid.showAlertPrompt("Enter Digits", "I'll call you if you give me your number", "phone number", "867-5309", "Send", "Not a Chance");
			}
			if (GUILayout.Button("Two Field Prompt"))
			{
				EtceteraAndroid.showAlertPromptWithTwoFields("Need Info", "Enter your credentials:", "username", "harry_potter", "password", string.Empty, "OK", "Cancel");
			}
			if (GUILayout.Button("Show Progress Dialog"))
			{
				EtceteraAndroid.showProgressDialog("Progress is happening", "it will be over in just a second...");
				Invoke("hideProgress", 1f);
			}
			if (GUILayout.Button("Text to Speech Speak"))
			{
				EtceteraAndroid.setPitch(UnityEngine.Random.Range(0, 5));
				EtceteraAndroid.setSpeechRate(UnityEngine.Random.Range(0.5f, 1.5f));
				EtceteraAndroid.speak("Howdy. Im a robot voice");
			}
			if (GUILayout.Button("Prompt for Video"))
			{
				EtceteraAndroid.promptToTakeVideo("fancyVideo");
			}
			endColumn(true);
			if (GUILayout.Button("Show Web View"))
			{
				EtceteraAndroid.showWebView("http://prime31.com");
			}
			if (GUILayout.Button("Email Composer"))
			{
				StartCoroutine(saveScreenshotToSDCard(delegate(string path)
				{
					EtceteraAndroid.showEmailComposer("noone@nothing.com", "Message subject", "click <a href='http://somelink.com'>here</a> for a present", true, path);
				}));
			}
			if (GUILayout.Button("SMS Composer"))
			{
				EtceteraAndroid.showSMSComposer("I did something really cool in this game!");
			}
			if (GUILayout.Button("Share Image Natively"))
			{
				StartCoroutine(saveScreenshotToSDCard(delegate(string path)
				{
					EtceteraAndroid.shareImageWithNativeShareIntent(path, "Sharing a screenshot...");
				}));
			}
			if (GUILayout.Button("Share Text and Image Natively"))
			{
				StartCoroutine(saveScreenshotToSDCard(delegate(string path)
				{
					EtceteraAndroid.shareWithNativeShareIntent("Check this out!", "Some Subject", "Sharing a screenshot and text...", path);
				}));
			}
			if (GUILayout.Button("Prompt to Take Photo"))
			{
				EtceteraAndroid.promptToTakePhoto("photo.jpg");
			}
			if (GUILayout.Button("Prompt for Album Image"))
			{
				EtceteraAndroid.promptForPictureFromAlbum("albumImage.jpg");
			}
			if (GUILayout.Button("Save Image to Gallery"))
			{
				StartCoroutine(saveScreenshotToSDCard(delegate(string path)
				{
					bool flag = EtceteraAndroid.saveImageToGallery(path, "My image from Unity");
				}));
			}
			if (GUILayout.Button("Ask For Review"))
			{
				EtceteraAndroid.resetAskForReview();
				EtceteraAndroid.askForReviewNow("Please rate my app!", "It will really make me happy if you do...");
			}
			endColumn();
			if (bottomRightButton("Next Scene"))
			{
				MonoBehaviourGUI.loadLevel("EtceteraTestSceneTwo");
			}
		}

		private void hideProgress()
		{
			EtceteraAndroid.hideProgressDialog();
		}

		public void imageLoaded(string imagePath)
		{
			EtceteraAndroid.scaleImageAtPath(imagePath, 0.1f);
			testPlane.GetComponent<Renderer>().material.mainTexture = EtceteraAndroid.textureFromFileAtPath(imagePath);
		}
	}
}
