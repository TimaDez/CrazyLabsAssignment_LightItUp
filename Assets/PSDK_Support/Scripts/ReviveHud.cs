using System;
using System.Collections;
using HyperCasual.PsdkSupport;
using UnityEngine;
using UnityEngine.UI;

namespace HyperCasual.Revive
{
	public class ReviveHud : MonoBehaviour
	{
		public static event Action<bool> ReviveDoneEvent = result => { }; 
		
		[Header("Revive Hud")]
		public ReviveCountdownProgressBar reviveCounter;
		public GameObject noInternetHud;
		
		public Button skipButton;
		public Button reviveButton;
		private bool counterActive =  false;
		private float currentTime = 0f;
		private float targetTime = 0f;
		private bool skipNextFrame;
		private bool isAppActive = true;
		public void StartCount(int countdownTime, int skipTime)
		{
			reviveButton.interactable = true;
			//SendDeltaEvent.rvImpression();
			reviveCounter.gameObject.SetActive(true);
			reviveCounter.StartCount(countdownTime);
			
			ReviveCountdownProgressBar.ExpiredEvent += OnReviveExpired;
			counterActive = true;
			currentTime = 0f;
			targetTime = skipTime;
			isAppActive = true;
			//StartCoroutine(ShowSkipButtonCoro(skipTime));
		}
		
		public void SkipReviveClicked()
		{
			Debug.Log("SkipReviveClicked ");
			reviveCounter.StopCount();
			ReviveDoneEvent(false);	
		}

		private void OnReviveExpired()
		{
			ReviveCountdownProgressBar.ExpiredEvent -= OnReviveExpired;
			ReviveDoneEvent(false);
		}

		private IEnumerator ShowSkipButtonCoro(int skipTime)
		{
			yield return new WaitForSeconds(skipTime);
			
			skipButton.gameObject.SetActive(true);
		}
		
		void OnRVFail()
		{				
			Destroy(gameObject);
			ReviveDoneEvent(false);		
		}
		
		void OnRVSuccess()
		{						
			//SendDeltaEvent.rvWatched();
			Destroy(gameObject);
			ReviveDoneEvent(true);
		}		

		void Update()
		{
			if (skipNextFrame) {
				skipNextFrame = false;
				return;
			}
			if (counterActive && isAppActive) {
				currentTime += Time.deltaTime;
				if (currentTime>=targetTime) {
					counterActive = false;
					currentTime = 0f;
					targetTime = 0f;
					skipButton.gameObject.SetActive(true);
				}
			}
		}
		void OnApplicationPause(bool pauseStatus)
		{
			skipNextFrame = true;
		}

		void OnApplicationFocus(bool pauseStatus)
		{
			skipNextFrame = true;
			if (pauseStatus) {
				isAppActive = true;
			} else {
				isAppActive = false;
			}
		}

	}
	

}