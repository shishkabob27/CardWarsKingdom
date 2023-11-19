using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewsPopupController : Singleton<NewsPopupController>
{
	public UITweenController ShowTween;

	public GameObject NewsEntryObject;

	public Transform ScrollParent;

	public void CheckNewsToShow()
	{
		StartCoroutine(CheckNewsToShowCo());
	}

	private IEnumerator CheckNewsToShowCo()
	{
		if (!Singleton<MailController>.Instance.IsMessageRetrieveDone())
		{
			Singleton<BusyIconPanelController>.Instance.Show();
			while (!Singleton<MailController>.Instance.IsMessageRetrieveDone())
			{
				yield return null;
			}
			Singleton<BusyIconPanelController>.Instance.Hide();
		}
		List<MailData> mailToShow = Singleton<MailController>.Instance.GetUnreadPopupMail();
		if (mailToShow.Count == 0)
		{
			Singleton<TownController>.Instance.AdvanceIntroState();
			yield break;
		}
		ShowTween.Play();
		Vector3 position = Vector3.zero;
		for (int i = 0; i < mailToShow.Count; i++)
		{
			NewsPopupEntry entry = ScrollParent.InstantiateAsChild(NewsEntryObject).GetComponent<NewsPopupEntry>();
			entry.gameObject.SetActive(true);
			entry.Populate(mailToShow[i], i == mailToShow.Count - 1);
			entry.transform.localPosition = position;
			position.y -= entry.GetTotalHeight();
		}
	}

	public void OnClosed()
	{
		ScrollParent.DestroyAllChildren();
		Singleton<TownController>.Instance.AdvanceIntroState();
	}
}
