using UnityEngine;

namespace LightItUp.UI
{
	public class UI_PositionSwitch : MonoBehaviour
	{
		public Vector2 posA, posB;

		private void OnValidate()
		{
			//posA = GetComponent<RectTransform>().anchoredPosition;
		}

		void Awake()
		{
			//posA = GetComponent<RectTransform>().anchoredPosition;
		}

		public void Toggle(bool toggle)
		{
			GetComponent<RectTransform>().anchoredPosition = toggle ? posB : posA;
		}
	}
}
