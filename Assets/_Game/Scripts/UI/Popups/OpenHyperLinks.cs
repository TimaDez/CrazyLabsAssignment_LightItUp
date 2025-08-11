using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class OpenHyperLinks : MonoBehaviour
{

	private string privacyPolicyLink = "https://tabtale.com/apps-privacy-policy/";
	private string termsOfUseLink = "https://tabtale.com/terms_of_use_none_multiview.html";

	public void OnPrivacyPolicyClick()
	{
		Application.OpenURL(privacyPolicyLink);
	}

	public void OnTermsOfUseClick()
	{
		Application.OpenURL(termsOfUseLink);
	}
}
