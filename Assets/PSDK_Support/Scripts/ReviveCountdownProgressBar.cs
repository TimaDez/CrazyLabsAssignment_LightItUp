using UnityEngine;
using UnityEngine.UI;
using System;

namespace HyperCasual.Revive
{
	public class ReviveCountdownProgressBar : MonoBehaviour
	{
		public static event Action ExpiredEvent = () => {};
		
		public Text timeLeftText;
		public Image bar;
		
		bool isActive;
		DateTime timeStart, timeEnd;
		float counterSeconds, counterMiliseconds;
		int lastSeconds = -1;
		bool isPaused;
		TimeSpan timeLeftBeforePause;
		private float currentElapsedTime;
		private float targetTime;
		private bool isTimerActive = false;
		private bool skipNextFrame = false;
		private bool isAppActive = true;
		void Start()
		{
			timeLeftText.text = Mathf.CeilToInt(counterSeconds).ToString();
			if (bar != null)
				bar.fillAmount = 1f;
		}

		public void StartCount(int seconds)
		{
			isTimerActive = true;
			currentElapsedTime = 0f;
			targetTime = seconds;
			skipNextFrame = false;
			isActive=true;
			Time.timeScale = 1f;
			counterSeconds = seconds;
			counterMiliseconds = counterSeconds*1000f;
			timeStart = DateTime.Now;
			timeEnd = timeStart.AddSeconds(counterSeconds);
			isActive=true;
			isPaused = false;
			isAppActive = true;
		}
		public void StopCount()
		{
			isActive = false;
			isPaused = false;
		}

		void OnApplicationPause(bool pauseStatus)
		{
			skipNextFrame = true;
//			if (pauseStatus)
//			{
//				if (isActive)
//				{
//					isPaused = true;
//					timeLeftBeforePause = (timeEnd-DateTime.Now);
//				}
//			}
//			else
//			{
//				if (isPaused)
//				{
//					timeEnd = DateTime.Now + timeLeftBeforePause;
//					isPaused = false;
//				}
//				
//			}
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
		void Update ()
		{
			
			if (skipNextFrame) {
				skipNextFrame = false;
				return;
			}

			if (isTimerActive && isAppActive) {
				currentElapsedTime += Time.deltaTime;

				if (bar != null)
				{
					float fill = currentElapsedTime / targetTime;
					bar.fillAmount = fill;
				}
				if (currentElapsedTime >= targetTime) {
					ExpiredEvent();
				}


			}
		}
		public void FixedUpdate()
		{
//			if (!isActive || isPaused)
//				return;
//
//			TimeSpan timeLeft = (timeEnd-DateTime.Now);
//
//			if (timeLeft.TotalMilliseconds < 0)
//			{
//				isActive=false;
//				
//				ExpiredEvent();
//				return;
//			}
//
//			if (bar != null)
//			{
//				float fill = (timeLeft.Milliseconds + timeLeft.Seconds*1000f) / counterMiliseconds;
//				bar.fillAmount = fill;
//			}
//			if (lastSeconds != timeLeft.Seconds){
//				timeLeftText.text = Mathf.Ceil(timeLeft.Seconds+1f).ToString();
//				lastSeconds = timeLeft.Seconds;
//			}

		}

		
		
	}
}
