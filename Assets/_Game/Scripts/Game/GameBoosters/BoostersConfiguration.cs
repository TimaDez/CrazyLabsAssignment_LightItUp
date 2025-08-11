using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightItUp.Singletons;

namespace LightItUp.Data
{     
	public class BoostersConfiguration : SingletonAsset<BoostersConfiguration>
	{
		public List<BoosterConfiguration> boosters;
	

		[Header("Spring Shoes")]
		public int springShoesJumpCount;

		[Header("Magic Hat Radius")]
		public float magicHatRadius;

		public int initialBoosterUnlockCharges = 3;

		public BoosterConfiguration GetSpecificBoosterConfigByType(BoosterType boosterType)
		{
			for (int i = 0; i < boosters.Count; i++) {
				if (boosters[i].boosterType == boosterType) {
					return boosters [i];
				}
			}
			return null;
		}


		public int GetBoosterUnlockLevel(BoosterType booster)
		{
			for (int i = 0; i < boosters.Count; i++) {
				if (boosters[i].boosterType == booster ) {
					return boosters [i].BoosterUnlockLevel;
				}
			}
			return -1;
		}

		#if UNITY_EDITOR
		[UnityEditor.MenuItem("Assets/Create/BoosterConfiguration")]
		public static void CreatePrefabAssets()
		{
			CreateAsset();
		}

		public override void Init()
		{
		}
		#endif
	}

}