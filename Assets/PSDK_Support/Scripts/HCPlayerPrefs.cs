using UnityEngine;
using System;

namespace HyperCasual
{
	public static class HCPlayerPrefs
	{
		// extension methods
		public static void SetPlayerPref(this int value, string key)
		{
			SetInt(key, value);						
		}
		
		public static void SetPlayerPref(this bool value, string key)
		{
			SetBool(key, value);						
		}
		
		public static void SetPlayerPref(this string value, string key)
		{
			SetString(key, value);						
		}
		
		public static void SetPlayerPref(this float value, string key)
		{
			SetFloat(key, value);						
		}
		
		public static void SetPlayerPref(this DateTime value, string key)
		{
			SetDateTime(key, value);						
		}
		
		public static void SetInt(string name, int value)
		{
			PlayerPrefs.SetInt(name, value);
		}

		public static void SetFloat(string name, float value)
		{
			PlayerPrefs.SetFloat(name, value);
		}

		public static void SetBool(string name, bool value)
		{
			PlayerPrefs.SetInt(name, value ? 1 : 0);
		}

		public static void SetString(string name, string value)
		{
			PlayerPrefs.SetString(name, value);
		}

		public static int GetInt(string name, int defaultValue = 0)
		{
			return PlayerPrefs.GetInt(name, defaultValue);
		}

		public static float GetFloat(string name, float defaultValue = 0f)
		{
			return PlayerPrefs.GetFloat(name, defaultValue);
		}
		
		public static bool GetBool(string name, bool defaultValue = false)
		{
			return PlayerPrefs.GetInt(name, defaultValue ? 1 : 0) != 0;
		}

		public static string GetString(string name, string defaultValue = "")
		{
			return PlayerPrefs.GetString(name, defaultValue);
		}

		public static void DeleteAll()
		{
			PlayerPrefs.DeleteAll();
		}

		public static void Save()
		{
			PlayerPrefs.Save();
		}
		
		public static void SetDateTime(string name, DateTime value)
		{						
			PlayerPrefs.SetString(name, value.ToBinary().ToString());		
		}

		public static DateTime GetDateTime(string name, DateTime defaultValue)
		{
			string dateTimeString = PlayerPrefs.GetString(name, "");

			if (string.IsNullOrEmpty(dateTimeString))
				return defaultValue;
			try
			{
				long binaryDate = Convert.ToInt64(dateTimeString);
				return DateTime.FromBinary(binaryDate);
			}
			catch (Exception e)
			{
				Debug.LogException(e);
				return defaultValue;
			}						
		}
	}
}