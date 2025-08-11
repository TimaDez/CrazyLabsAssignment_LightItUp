using UnityEngine;

namespace HyperCasual.PsdkSupport
{
	public class NoInternetHUD : MonoBehaviour
	{
		private static string path = "NoInternetPrefab";
	
		public static void Show()
		{
			var prefab = Resources.Load(path);

			if (prefab == null)
			{
				Debug.LogError("can't find no internet prefab at: " + path);
				return;
			}
		
			Instantiate(prefab);
		}

		public void Hide()
		{
			Destroy(gameObject);
		}	

		public void Update()
		{
			HandleBackButton();
		}

		private void HandleBackButton()
		{
			if (Input.GetKeyDown(KeyCode.Escape)) 
				Hide();


		}
	}
}
