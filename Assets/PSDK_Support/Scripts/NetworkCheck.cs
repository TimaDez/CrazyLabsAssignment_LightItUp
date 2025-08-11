using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

namespace HyperCasual.PsdkSupport
{
	public class NetworkCheck 
		: MonoBehaviour
	{
		public event Action <bool> AnnounceNetworkStatus = (isConnected)=>{};

		public void Start()
		{
			StartCoroutine(TestConnectionCoro());
		}

		IEnumerator TestConnectionCoro() 
		{
			while(true)
			{
				Debug.Log("Network Check::Testing Connection");

			    var antiCacheRandomizer = string.Format("?p={0}", Random.Range(1, 100000000));
				var www = new WWW(_networkCheckUrl + antiCacheRandomizer);
				yield return www;

				_isConnected = (www.isDone && www.bytesDownloaded > 0);
				if (_isConnected != _prevConnectStatus)
				{
					_prevConnectStatus = _isConnected;
					AnnounceNetworkStatus(_isConnected);

				}

				
				// Original code sent events here for connection status changed
				yield return new WaitForSeconds(_checkDelayTime);
			}
		}

		public bool HasInternetConnection() 
		{
			return _isConnected;
		}

		private bool _isConnected = false;
		private bool _prevConnectStatus = false;
		private float _checkDelayTime = 5.0f;
		private static string _networkCheckUrl = "https://ping.ttpsdk.info/TabTale-Test";

		public void ShowNoInternetConnectionMessage()
		{
			NoInternetHUD.Show();
		}
	}
}
