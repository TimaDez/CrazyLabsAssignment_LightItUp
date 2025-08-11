using LightItUp.UI;
using UnityEngine;

namespace LightItUp.UI
{
	public class UI_Quit : UI_Popup
	{
		public void No()
		{
			ClosePopup();			
		}

		public void Yes()
		{
	
#if UNITY_EDITOR

			UnityEditor.EditorApplication.isPlaying = false;
		
#else
		Application.Quit();
#endif
		}
	}
}