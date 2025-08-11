using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.Scripts.PowerUp.Models
{
    [Serializable]
    public class SeekingMissile
    {
        public GameObject MissilePrefab;
        public float Speed;
        public Color Color;
    }
    
    [CreateAssetMenu(fileName = "SeekingMissilesModel", menuName = "PowerUp/SeekingMissilesModel")]
    public class SeekingMissilesModel : ScriptableObject
    {
        #region Editor
        
        [SerializeField] private int _missilesCount = 3;
        [SerializeField] private List<SeekingMissile> _seekingMissiles;

        #endregion

        #region Methods

        public List<SeekingMissile> GetSeekingMissiles()
        {
            var missiles = new List<SeekingMissile>();
            for (int i = 0; i < _missilesCount; i++)
            {
                if (i < _seekingMissiles.Count)
                {
                    missiles.Add(_seekingMissiles[i]);
                }
                else
                {
                    missiles.Add(_seekingMissiles[UnityEngine.Random.Range(0, _seekingMissiles.Count)]);
                }
            }
            return missiles;
        }

        #endregion
    }
}