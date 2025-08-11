using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.PowerUp;
using UnityEngine;
using LightItUp.Game;
using LightItUp.Singletons;

namespace LightItUp.Data
{
    public enum ArrowType
    {
        Move, Rotate, Attract, Repel, 
        MoveRotate, MoveAttract, MoveRepel, 
        RotateAttract, RotateRepel,
        MoveRotateAttract, MoveRotateRepel
    }

    [Serializable]
    public class ArrowToPrefab
    {
        public ArrowType type;
        public GameObject prefab;
    
        public override string ToString()
        {
            return type.ToString();
        }
    }

    public class PrefabAssets : SingletonAsset<PrefabAssets>
    {

        public bool createDict = false;
        public List<BlockController> blocks;
        public Wall wall;
        public Transform starDummy;
        public ReviveZone reviveZone;
        public List<CollectibleController> collectibles;
        public EffectorController effector;
        
        public PlayerDummy playerDummy;
    
        [Header("Block arrows")]
        public Transform moveArrow;
        public Transform rotateArrow;
        public SpriteRenderer effectorArrow;

        public List<ArrowToPrefab> arrowToPrefabsList;

        //public Dictionary<ArrowType, GameObject> arrowToPrefabDict;

        [Header("Spikes")]
        public SpikeController spikes;
        public SpikeRoundController spikesRound;
    
        [Header("Explode")]
        public ExplodePartsController explodeParts;

        [Header("Pivot")] 
        public PivotRotator pivotRotator;
    
        public ShapeMesh shapeMesh;
        public LineMesher lineMesher;
        public GameLevel gameLevelClean;

		public GameObject auraParticles;

        [Header("Seeking Missiles")]
        public SeekingMissilesController SeekingMissilesPrefab;
        
        public GameObject ArrowToPrefab(ArrowType type)
        {
            var arrowToPrefab = arrowToPrefabsList.FirstOrDefault(x => x.type == type);

            if (arrowToPrefab != null)
            {
                return arrowToPrefab.prefab;
            }

            return null;
        }

        private void OnEnable()
        {
            ///arrowToPrefabDict.Clear();
            //arrowToPrefabDict = arrowToPrefabsList.ToDictionary(x => x.type, x => x.prefab);
        }

        public static BlockController GetBlock(BlockController.ShapeType shapeType) {
            var bd = Instance.blocks.FirstOrDefault(x => x.shape == shapeType);
            if (bd != null) return bd;
            Debug.Log("No block with shape: "+shapeType);
            return null;
        }
        public static CollectibleController GetCollectible(CollectibleEffect ce)
        {
            var bd = Instance.collectibles.FirstOrDefault(x => x.effect == ce);
            if (bd != null)
                return bd;
            Debug.Log("No collectible with effect: " + ce);
            return null;
        }
#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/Create/PrefabAssets")]
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