using System.Collections;
using System.Collections.Generic;
using LightItUp.Data;
using TMPro;
using UnityEngine;

public class LevelIdDisplay : MonoBehaviour
{
	public TextMeshProUGUI uiText;

	private void OnEnable()
	{
		uiText.gameObject.SetActive(DebugToggleLevelIdDisplay.IsOn);

		if (uiText.gameObject.activeInHierarchy)
		{
			uiText.text = GameManager.Instance.currentLevel.uid;
		}
	}
}
