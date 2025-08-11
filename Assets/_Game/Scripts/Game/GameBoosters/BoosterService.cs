using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightItUp.Singletons;
using LightItUp.Data;
using System;

namespace LightItUp
{
	


	public enum BoosterType
	{
		MagicHat,
		SpringShoes,
		MetalBoots
	}


	public class BoosterService : SingletonCreate<BoosterService> {
		

		public BoostersConfiguration boosterConfig;

		private List<BoosterType> currentActiveBoosters;

		public Action OnBoosterAmountChange = () => { };

		private List<BoosterType> selectedBoosters;
		
		/// <summary>
		/// Determines whether this level is the specific level when the booster is unlocked.
		/// </summary>
		/// <param name="booster">Booster.</param>
		/// <param name="currentUnlockedLevel">Current unlocked level.</param>
		/// 
		//
		public bool IsBoosterUnlockLevel(BoosterType booster, int currentUnlockedLevel)
		{
			bool isBoosterUnlocked = GameData.PlayerData.IsBoosterUnlocked (booster);
			if (!isBoosterUnlocked && IsLevelReachedForBooster (booster, currentUnlockedLevel)) {
				return true;
			} else {
				return false;
			}
		}

		public string GetBoosterNameByType(BoosterType boosterType)
		{
			for (int i = 0; i < boosterConfig.boosters.Count; i++) {
				if (boosterConfig.boosters[i].boosterType == boosterType) {
					return boosterConfig.boosters [i].BoosterName;
				}
			}
			return "";
		}

		public bool IsAnyBoosterPurchasedOrUnlocked()
		{
			for (int i = 0; i < boosterConfig.boosters.Count; i++) {
				if (IsBoosterUnlocked(boosterConfig.boosters[i].boosterType)) {
					return true;
				}
			}
			return false;
		}
		public string GetBoosterDescriptionByType(BoosterType boosterType)
		{
			for (int i = 0; i < boosterConfig.boosters.Count; i++) {
				if (boosterConfig.boosters[i].boosterType == boosterType) {
					return boosterConfig.boosters [i].BoosterDescription;
				}
			}
			return "";
		}
		public bool ShouldShowBoosterUnlockedPopup(int currentLevelIndex)
		{
			for (int i = 0; i < boosterConfig.boosters.Count; i++) {
				if (IsBoosterUnlockLevel(boosterConfig.boosters[i].boosterType,currentLevelIndex)) {
					return true;
				}
			}
			return false;
		}

		public bool ShouldShowBoosterPurchasePopup()
		{
			return true;
		}

		public BoosterType GetCurrentUnlockedBooster(int currentLevelIndex)
		{
			for (int i = 0; i < boosterConfig.boosters.Count; i++) {
				if (IsBoosterUnlockLevel(boosterConfig.boosters[i].boosterType,currentLevelIndex)) {
					return boosterConfig.boosters[i].boosterType;
				}
			}
			return BoosterType.MagicHat;
		}
			

		public bool IsLevelReachedForBooster(BoosterType booster, int currentUnlockedLevel)
		{
			return currentUnlockedLevel >= boosterConfig.GetBoosterUnlockLevel (booster);

		}

		/// <summary>
		///  Called when booster is unlocked.
		/// </summary>
		/// <param name="booster">Booster.</param>
		public void UnlockBooster(BoosterType booster)
		{
			GameData.PlayerData.SetBoosterUnlock (booster, true);
		}

		public void SetBoosterPurchased(BoosterType booster)
		{
			GameData.PlayerData.SetBoosterPurchase (booster, true);
		}

		public int GetBoosterUnlockLevel(BoosterType booster)
		{
			return boosterConfig.GetBoosterUnlockLevel (booster);
		}
		public void AddMandatoryBoosterCharges(BoosterType booster, int value)
		{
			GameData.PlayerData.AddMandatoryBoosterCharges (booster, value);
			AddSelectedBooster(booster);
			OnBoosterAmountChange();
		}
		public void AddRegularBoosterCharges(BoosterType booster, int value)
		{
			
			GameData.PlayerData.AddRegularBoosterCharges (booster, value);
			OnBoosterAmountChange();
        }
		
		
		
		/// <summary>
		/// Locks all boosters for debugging.
		/// </summary>
		/// <param name="booster">Booster.</param>
		public void LockAllBoosters(BoosterType booster)
		{
			foreach (BoosterType item in Enum.GetValues(typeof(BoosterType))){
				GameData.PlayerData.SetBoosterUnlock (booster, false);
			}
		}

		/// <summary>
		/// Removes all booster charges, for debug.
		/// </summary>
		/// <param name="booster">Booster.</param>
		public void RemoveAllBoosters(BoosterType booster)
		{
			foreach (BoosterType item in Enum.GetValues(typeof(BoosterType))){
				GameData.PlayerData.SetBoosterCharges (booster, 0);
			}
		}

		/// <summary>
		/// Determines whether this booster is unlocked.
		/// </summary>
		/// <param name="booster">Booster.</param>
		public bool IsBoosterUnlocked(BoosterType booster)
		{
			return GameData.PlayerData.IsBoosterAllowed (booster);
		}

		/// <summary>
		/// Gets the booster available ammount.
		/// </summary>
		/// <returns>The booster available ammount.</returns>
		/// <param name="booster">Booster.</param>
		public int GetTotalBoosterAmmount(BoosterType booster)
		{
			return GameData.PlayerData.GetTotalBoosterAmmount (booster);
		}

		public int GetMandatoryBoosterAmmount(BoosterType booster)
		{
			return GameData.PlayerData.GetMandatoryBoosterAmmount (booster);
		}
		public int GetRegularBoosterAmmount(BoosterType booster)
		{
			return GameData.PlayerData.GetRegularBoosterAmmount (booster);
		}
		public bool ConsumeBooster(BoosterType booster, int amount)
		{
			return GameData.PlayerData.ConsumeBooster (booster, amount);
		}

		public bool AreBoostersActive()
		{
			return true;
		}


		/// <summary>
		/// Shoulds show booster popup, depending if any is unlocked.
		/// </summary>
		public bool ShouldShowBoosterPopup()
		{
			foreach (BoosterType item in Enum.GetValues(typeof(BoosterType))){
				if (GameData.PlayerData.IsBoosterUnlocked (item)) {
					return true;
				}
			}
			return false;
		}

		public bool PurchaseBooster(BoosterType booster)
		{
			
			if (GameData.PlayerData.ConsumeBooster(booster)) 
			{
				currentActiveBoosters.Add (booster);
				return true;
			}
			return false;
		}

		public void AddSelectedBooster(BoosterType booster)
		{
			Debug.Log("------------------ SELECTED");
			if (selectedBoosters == null)
			{
				selectedBoosters = new List<BoosterType>();
			}
            if(!selectedBoosters.Contains(booster))
            {
                selectedBoosters.Add(booster);
            }
			
		}
		public void RemoveSelectedBooster(BoosterType boosterType)
		{
			for (int i=0; i<selectedBoosters.Count; i++)
			{
				if (selectedBoosters[i] == boosterType)
				{
					selectedBoosters.RemoveAt(i);
				}
			}
		}
		public void ConsumeSelectedBoosters()
		{
			if (selectedBoosters != null && selectedBoosters.Count > 0)
			{
				foreach (BoosterType b in selectedBoosters)
				{
					PurchaseBooster(b);
				}
				selectedBoosters.Clear();
			}
			
		}
		
		public List<BoosterType> GetCurrentActiveBoosters()
		{
			return currentActiveBoosters;
		}


		public void ConsumeCurrentActiveBoostres()
		{
			currentActiveBoosters = new List<BoosterType> ();
		}

		public override void Awake()
		{
			base.Awake ();
			currentActiveBoosters = new List<BoosterType> ();
		}

	}
}
