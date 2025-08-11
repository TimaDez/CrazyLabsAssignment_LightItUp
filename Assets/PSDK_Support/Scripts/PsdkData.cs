using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

namespace HyperCasual.PsdkSupport
{

	[Serializable]
	public class AchivementData
	{
		public string id;
		public string urlGP;
		public string urlIOS;
	}

	[Serializable]
	public class InappData
	{
		public string id;
		public string iapIdIOS;
		public string iapIdGP;
	}

	[CreateAssetMenu(fileName = "PsdkData", menuName = "[Psdk Support]/Psdk Data", order = 1)]
	public class PsdkData : ScriptableObject
	{
		[Header("URLs")] 
		public string appStoreUrlIOS;
		public string appStoreUrlGP;

		public string leaderboardIdIOS;
		public string leaderboardIdGP;

		[Header("Share")] 
		public string shareHeader;
		public string shareSubject;
		public string shareBody;

		[Header("Achivements")] 
		[SerializeField]
		public List<AchivementData> achievments;

		[Header("InApps")] 
		[SerializeField] public List<InappData> inApps;

		private string appStoreUrl;

		[Header("Flow")] 
		public int reviveMinScoreRequirement = 20;
		public int reviveBestScorePercentageRequirement = 70;
		public int reviveSegmentThresholdInEndless = 3;
		
		public string AppStoreUrl
		{
			get { return appStoreUrl; }
		}

		private string leaderboardId;

		public string LeaderboardId
		{
			get { return leaderboardId; }
		}


		private bool isGP;

		public bool IsGP
		{
			get { return isGP; }
		}

		private bool isIOS;

		public bool IsIOS
		{
			get { return isIOS; }
		}

		public void SetUrls()
		{
#if UNITY_IOS
		isGP = false;
		isIOS = true;
		#endif
#if AMAZON
		isGP = true;
		isIOS = false;
		#endif
#if UNITY_ANDROID
			isGP = true;
			isIOS = false;
#endif
			if (isIOS)
			{
				appStoreUrl = appStoreUrlIOS;
				leaderboardId = leaderboardIdIOS;
			}
			else if (isGP)
			{
				appStoreUrl = appStoreUrlGP;
				leaderboardId = leaderboardIdGP;
			}
			else // editor, amazon or bug
			{
				appStoreUrl = appStoreUrlGP;
				leaderboardId = leaderboardIdGP;
			}
		}

		public string GetAchivUrl(string achivId)
		{
			AchivementData achiv = achievments.First(ach => ach.id == achivId);
			if (achiv == null)
				return "";
			return (isGP ? achiv.urlGP : achiv.urlIOS);
		}

		public string GetInappStoreId(string id)
		{
			InappData inApp = inApps.First(iap => iap.id == id);
			if (inApp == null)
				return "";
			return (isGP ? inApp.iapIdGP : inApp.iapIdIOS);
		}

		public string GetItemIdOfIapId(string iapId)
		{
			InappData inApp = isGP ? inApps.First(iap => iap.iapIdGP == iapId) : inApps.First(iap => iap.iapIdIOS == iapId);
			if (inApp == null)
				return "";
			return inApp.id;
		}
	}
}

