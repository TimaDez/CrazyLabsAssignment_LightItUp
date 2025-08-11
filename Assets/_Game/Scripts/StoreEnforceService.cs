using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightItUp.Game;
using LightItUp.Singletons;
using Cinemachine;
using HyperCasual.PsdkSupport;
using LightItUp.UI;
using HyperCasual;
using System;

namespace LightItUp
{
	
	public class StoreEnforceService : SingletonCreate<StoreEnforceService> 
	{
		public float elapsedTimeNeeded = 1f;
		private string firstStartTime = "firstStartTime";
		private string isStoreShown = "isStoreShown";


		public override void Awake()
		{
			base.Awake();
			//DEBUG ONLY
//			HCPlayerPrefs.SetBool (isStoreShown, false);
//			HCPlayerPrefs.SetString(firstStartTime,"");

			AnnounceFirstStart ();
			//Invoke ("IsConditionMet", 5f);
		}

		public void AnnounceFirstStart()
		{
			
			if (PlayerPrefs.GetInt (isStoreShown) == 1) {
				return;
			}

			//Grab the old time from the player prefs as a long
			string lastKnownString = PlayerPrefs.GetString(firstStartTime);

			if (lastKnownString != "") {
				return;
			}

			//Store the current time when it starts
			DateTime currentDate = System.DateTime.Now;
			PlayerPrefs.SetString(firstStartTime, currentDate.ToBinary().ToString());
		}

		public bool IsConditionMet()
		{
			if (PlayerPrefs.GetInt(isStoreShown) == 0) {
				DateTime firstSessionTime = DateTime.FromBinary(Convert.ToInt64(PlayerPrefs.GetString(firstStartTime)));
				DateTime currentTime = System.DateTime.Now;
				TimeSpan timeDifference = currentTime.Subtract(firstSessionTime);
				int elapsedMinutes = timeDifference.Minutes;
				int elapsedDays = timeDifference.Days;
				int elapsedHours = timeDifference.Hours;

				if (elapsedMinutes + elapsedDays * 24*60 + elapsedHours*60 >= elapsedTimeNeeded) {

					return true;
				}
			}
			return false;
		}

		public void SetStoreShown()
		{
			PlayerPrefs.SetInt (isStoreShown, 1);
		}
	}



}