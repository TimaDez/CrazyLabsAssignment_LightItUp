using System.Collections;
using System.Collections.Generic;
using HyperCasual.Skins;
using UnityEngine;
using UnityEngine.UI;

namespace LightItUp
{
	public class SkinUnlockedButton : MonoBehaviour
	{
		public Image icon;
		
		public void Show(SkinConfig skinConfig)
		{
			gameObject.SetActive(true);
			icon.sprite = skinConfig.iconCircle;
		}

		public void Hide()
		{
			gameObject.SetActive(false);
		}
		
		
	}
}

