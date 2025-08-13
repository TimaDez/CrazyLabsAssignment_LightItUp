using System;
using System.Collections.Generic;
using _Game.Scripts.SeekingMissiles.Models;
using Cysharp.Threading.Tasks;
using LightItUp;
using LightItUp.Data;
using LightItUp.Game;
using LightItUp.UI;
using UnityEngine;

namespace _Game.Scripts.SeekingMissiles
{
    public class SeekingMissilesController : MonoBehaviour
    {
        #region Editor

        [SerializeField] private SeekingMissilesModel _missilesModel;
        [SerializeField] private SeekingMissileModel _defaultMissileModel;

        #endregion

        #region Private members

        private bool _isUsedOnLevel = false;
        private List<BlockController> _blocks;
        private PlayerController _player;
        private UI_Game _uiGame;

        #endregion

        #region Methods

        private void Awake()
        {
            _uiGame = CanvasController.GetPanel<UI_Game>();
            InitObjectPool();
        }

        private void InitObjectPool()
        {
            var missiles = _missilesModel.GetSeekingMissiles();
            if (missiles == null || missiles.Count == 0)
            {
                Debug.LogError("[SeekingMissilesController] UseSeekingMissiles() No missiles available.");
                ObjectPool.Instance.SetSeekingMissilePrefab(_defaultMissileModel.MissilePrefab);
                return;
            }
            ObjectPool.Instance.SetSeekingMissilePrefab(missiles[0].MissilePrefab);
        }
        
        public void Init(PlayerController player, List<BlockController> blocks)
        {
            _isUsedOnLevel = false;
            _player = player;
            _blocks = blocks;

            _uiGame.SeekingMissilesButton.interactable = true;
            _uiGame.SeekingMissilesButton.onClick.AddListener(() => UseSeekingMissiles().Forget());
        }

        private async UniTaskVoid UseSeekingMissiles()
        {
            if (_isUsedOnLevel)
            {
                Debug.Log("[SeekingMissilesController] UseSeekingMissiles() Seeking missiles already used in this level.");
                return;
            }

            _isUsedOnLevel = true;
            _uiGame.SeekingMissilesButton.interactable = false;
            
            var candidates = new List<(BlockController block, float sqrDist)>(_blocks.Count);
            foreach (var block in _blocks)
            {
                if (block.IsLit)
                    continue;

                var sqr = SqrDistanceToBlock(_player.transform.position, block);
                if (float.IsInfinity(sqr))
                    continue;

                candidates.Add((block, sqr));
            }

            if (candidates.Count == 0)
            {
                Debug.Log("[SeekingMissilesController] UseSeekingMissiles() No unlit blocks to target.");
                return;
            }

            candidates.Sort((a, b) => a.sqrDist.CompareTo(b.sqrDist));

            var missiles = _missilesModel.GetSeekingMissiles();
            if (missiles == null || missiles.Count == 0)
            {
                Debug.LogWarning("[SeekingMissilesController] UseSeekingMissiles() No missiles available.");
                return;
            }
            
            var targetCount = Mathf.Min(missiles.Count, candidates.Count);
            for (var i = 0; i < targetCount && i < missiles.Count; i++)
            {
                await UniTask.WaitForSeconds(0.2f, cancellationToken: this.GetCancellationTokenOnDestroy());
                
                var targetBlock = candidates[i].block;
                var col = targetBlock.col ? targetBlock.col : targetBlock.GetComponent<Collider2D>();
                var aimPoint = col ? col.ClosestPoint(_player.transform.position) : (Vector2)targetBlock.transform.position;
                
                var missile = ObjectPool.GetSeekingMissile();
                missile.transform.position = _player.transform.position;
                
                missile.SetData(missiles[i], _player.camFocus);
                missile.LaunchTowards(aimPoint, targetBlock);
            }
        }
        
        private static float SqrDistanceToBlock(Vector2 fromPos, BlockController block)
        {
            var col = block.col ? block.col : block.GetComponent<Collider2D>();
            if (!col || !col.enabled)
                return float.PositiveInfinity;

            var cp = col.ClosestPoint(fromPos);
            var dx = fromPos.x - cp.x;
            var dy = fromPos.y - cp.y;
            return dx * dx + dy * dy;
        }

        #endregion
    }
}