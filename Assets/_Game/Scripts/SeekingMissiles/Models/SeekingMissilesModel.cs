using System.Collections.Generic;
using UnityEngine;

namespace _Game.Scripts.SeekingMissiles.Models
{
    [CreateAssetMenu(fileName = "SeekingMissilesModel", menuName = "Seeking Missile/SeekingMissilesModel")]
    public class SeekingMissilesModel : ScriptableObject
    {
        #region Editor

        [SerializeField] private int _missilesAmount = 3;
        [SerializeField] private SeekingMissileModel[] _missileModels;

        #endregion
        
        #region Methods
        
        public List<SeekingMissileModel> GetSeekingMissiles()
        {
            var missiles = new List<SeekingMissileModel>();
            for (int i = 0; i < _missilesAmount; i++)
            {
                if (i < _missileModels.Length)
                {
                    missiles.Add(_missileModels[i]);
                }
                else
                {
                    missiles.Add(_missileModels[Random.Range(0, _missileModels.Length)]);
                }
            }
            
            return missiles;
        }
        #endregion
    }
}