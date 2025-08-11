using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightItUp.Singletons;
using System;

namespace LightItUp.Currency
{

	public class RVConsumableService : SingletonCreate<RVConsumableService> 
	{

		public RVConsumableConfig rvConfig;
		public static Action<bool> AnnounceRVStatus = (value) => {
		};
		private string RVWatchedTimestamp = "RVWatchedTimestamp";
		private bool isRVReady;


		void Start()
		{
			FirstStartCheck ();
			isRVReady = IsConditionMet ();
			AnnounceRVStatus (IsRvAvailable() && IsCooldownFinished());
		}


		void FirstStartCheck()
		{
			
			if (!PlayerPrefs.HasKey(RVWatchedTimestamp)) {
				PlayerPrefs.SetString(RVWatchedTimestamp, DateTime.Now.Subtract(new TimeSpan (0,0,(int)rvConfig.rvReplenishTime)).ToBinary().ToString());
			}
		}


		public bool IsRvAvailable()
		{
			return true;
		}

		public bool IsCooldownFinished()
		{

			bool isRVCooldownFinished = IsRVCooldownFinished ();
			return isRVCooldownFinished;	
		}

		public bool IsRVCooldownFinished()
		{

			if (!isRVReady) {
				isRVReady = IsConditionMet ();
			}
			return isRVReady;
		}

		public RVConsumableConfig GetRVConfig()
		{
			return rvConfig;
		}

		public void SetCurrentRVWatchedTime()
		{
			DateTime currentDate = System.DateTime.Now;
			PlayerPrefs.SetString(RVWatchedTimestamp, currentDate.ToBinary().ToString());
			isRVReady = false;
		}

		bool IsConditionMet()
		{
			if (rvConfig == null) {
				return false;
			}
			DateTime lastRVWatchedTime = DateTime.FromBinary(Convert.ToInt64(PlayerPrefs.GetString(RVWatchedTimestamp)));
			DateTime currentTime = System.DateTime.Now;
			TimeSpan timeDifference = currentTime.Subtract(lastRVWatchedTime);
			int elapsedSeconds = timeDifference.Seconds;
			int elapsedMinutes = timeDifference.Minutes;
			int elapsedDays = timeDifference.Days;
			int elapsedHours = timeDifference.Hours;

			if (elapsedMinutes*60 + elapsedDays * 24*60*60 + elapsedHours*60*60 + elapsedSeconds >= rvConfig.rvReplenishTime) {

				return true;
			}else
				{
				return false;
			}
		}

		public string GetRemainingTime()
		{
			DateTime lastRVWatchedTime = DateTime.FromBinary(Convert.ToInt64(PlayerPrefs.GetString(RVWatchedTimestamp)));
			DateTime currentTime = System.DateTime.Now;
			DateTime futureGoalTime = lastRVWatchedTime.AddSeconds (rvConfig.rvReplenishTime);

			TimeSpan timeDifference = futureGoalTime.Subtract(currentTime);

			string finalString = GetRemainingTimeStringFormat (timeDifference);

				
			return finalString;
		}

		string GetRemainingTimeStringFormat(TimeSpan timeDif)
		{
			string hours = timeDif.Hours >= 10 ? timeDif.Hours.ToString () : "0" + timeDif.Hours.ToString ();
			string minutes = timeDif.Minutes >= 10 ? timeDif.Minutes.ToString () : "0" + timeDif.Minutes.ToString ();
			string seconds = timeDif.Seconds >= 10 ? timeDif.Seconds.ToString () : "0" + timeDif.Seconds.ToString ();

			return hours + ":" + minutes + ":" + seconds;
		}

		void ResolveResult(bool val)
		{
			if (val) {
				SetCurrentRVWatchedTime ();
				RewardSuccess ();
			}
			AnnounceRVStatus (IsRvAvailable () && IsCooldownFinished());
		}

		void RewardSuccess()
		{
			//CurrencyService.Instance.AddCurrency (rvConfig.currency, rvConfig.currencyReward);
		}

	
	}

}