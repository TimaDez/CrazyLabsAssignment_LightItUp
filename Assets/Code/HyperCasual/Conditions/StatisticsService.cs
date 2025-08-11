using System;
using System.Collections.Generic;
using System.Linq;
using HyperCasual.PsdkSupport;
using UnityEngine;
using UnityEngine.Profiling;

namespace HyperCasual
{
	public class StatState
	{
		public string id = "";

		public int valueInRun = 0;
		public int valueInRow = 0;
		public int valueTotal = 0;
		public int maxInRun = 0;
		public int maxInRow = 0;
		public int MaxInRun { get { return (maxInRun > valueInRun ? maxInRun : valueInRun); } }
		public int MaxInRow { get { return (maxInRow > valueInRow ? maxInRow : valueInRow); } }

		public StatState (string statId) { id = statId; }
		public override string ToString () { return "[Stat=" + id + " :" + " InRun=" + valueInRun + " InRow=" + valueInRow + " Total=" + valueTotal + " ]"; }
		public void DebugReset () { valueInRun = 0; valueInRow = 0; valueTotal = 0; maxInRun = 0; maxInRow = 0; }
	}

	public class StatisticsService : Singleton<StatisticsService>
	{
		public static event Action<string> StatUpdated = (statId) => {
		};



		private List<StatState> stats = new List<StatState> ();
		
		Dictionary<string, StatState> statsDict = new Dictionary<string, StatState> ();

		const string Stat_Total_ = "Stat_Total_";
		const string Stat_MaxInRun_ = "Stat_MaxInRun_";
		const string Stat_MaxInRow_ = "Stat_MaxInRow_";

		bool isInitialized = false;
		
		private static bool isInRun;

		public void Initialize ()
		{
			if (isInitialized)
				return;
			isInitialized = true;
			
			statsDict = stats.ToDictionary (arg => arg.id);
		}

		public static StatState GetStat (string statId)
		{
			StatState foundState = null;

			Instance.statsDict.TryGetValue (statId, out foundState);
			if (foundState != null)
				return foundState;

			var newStat = CreateStat(statId);
			return newStat;
		}

		private static StatState CreateStat(string statId)
		{
			StatState newStat = new StatState(statId);
			newStat.valueTotal = PlayerPrefs.GetInt(Stat_Total_ + newStat.id, 0);
			newStat.maxInRun = PlayerPrefs.GetInt(Stat_MaxInRun_ + newStat.id, 0);
			newStat.maxInRow = PlayerPrefs.GetInt(Stat_MaxInRow_ + newStat.id, 0);
			Instance.stats.Add(newStat);
			Instance.statsDict[statId] = newStat;
			return newStat;
		}

		public int GetStatValueInRun (string statId)
		{
			return GetStat (statId).valueInRun;
		}

		public int GetStatValueInRow (string statId)
		{
			return GetStat (statId).valueInRow;
		}
		public int GetStatValueTotal (string statId)
		{
			return GetStat (statId).valueTotal;
		}
		public int GetStatMaxInRun (string statId)
		{
			return GetStat (statId).MaxInRun;
		}
		public int GetStatMaxInRow (string statId)
		{
			return GetStat (statId).MaxInRow;
		}

		#region count stats

		public static void CountStat (string statId, int value)
		{
			StatState stat = GetStat (statId);

			if (isInRun) {
				stat.valueInRow += value;
				stat.valueInRun += value;
			}
			stat.valueTotal += value;
			if (!isInRun)
			{
				PlayerPrefs.SetInt(Stat_Total_ + stat.id, stat.valueTotal);
				PlayerPrefs.Save();
			}

			StatUpdated(stat.id);
		}

		/// <summary>
		/// used for statistics that require several things in a row, to reset the count
		///    Example - if need to collect 5 coins in a row and missed one)
		/// </summary>
		public void ResetStatInRow (string statId, int value = 0)
		{
			StatState stat = GetStat (statId);
			stat.valueInRow = value;
			StatUpdated(stat.id);
		}

		// used for events that require several things in a row, to reset the count
		//    Example - if need to collect 5 coins in a row and missed one)
		public void StopEventInRow (string statId, int value = 0)
		{
			//Debug.Log("Report stop Stat " + statId);
			StatState stat = GetStat (statId);
			stat.valueInRow = value;
			StatUpdated(stat.id);
			// handled timed - TODO
		}

		public static void SetStat (string statId, int value = 0)
		{
			StatState stat = GetStat (statId);
			stat.valueTotal = value;
			PlayerPrefs.SetInt (Stat_Total_ + stat.id, stat.valueTotal);
			PlayerPrefs.Save();
			StatUpdated(stat.id);
		}

		#endregion

		public static void StartRun ()
		{			
			// reset single-run events
			isInRun = true;
			foreach (StatState stat in Instance.stats) {
				stat.valueInRun = 0;
				stat.valueInRow = 0;
			}
		}

		public static void EndRun ()
		{			
			// To improve FPS, only storing values to player prefs on run exit
			foreach (StatState stat in Instance.stats) {
				if (stat.valueInRun > 0) {
					PlayerPrefs.SetInt (Stat_Total_ + stat.id, stat.valueTotal);
				}
				if (stat.valueInRun > stat.maxInRun) {
					stat.maxInRun = stat.valueInRun;
					PlayerPrefs.SetInt (Stat_MaxInRun_ + stat.id, stat.maxInRun);
				}
				if (stat.valueInRow > stat.maxInRow) {
					stat.maxInRow = stat.valueInRow;
					PlayerPrefs.SetInt (Stat_MaxInRow_ + stat.id, stat.maxInRow);
				}
			}
			PlayerPrefs.Save();
			isInRun = false;
		}

		public void DebugResetGame ()
		{
			foreach (StatState stat in stats) {
				stat.DebugReset ();
			}
		}
	}

}
