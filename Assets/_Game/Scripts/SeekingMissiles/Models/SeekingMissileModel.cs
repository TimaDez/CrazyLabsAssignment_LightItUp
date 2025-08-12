using UnityEngine;

namespace _Game.Scripts.SeekingMissiles.Models
{
    [CreateAssetMenu(fileName = "SeekingMissileModel", menuName = "Seeking Missile/SeekingMissileModel")]
    public class SeekingMissileModel : ScriptableObject
    {
        [Header("Prefab & Visuals")]
        [SerializeField] private SeekingMissileBall _missilePrefab;
        [SerializeField] private Color _color = Color.white;

        [Header("Motion")]
        [SerializeField, Min(0f)] private float _speed = 8f;
        [SerializeField, Min(0f)] private float _turnRateDeg = 360f;
        [SerializeField, Min(0.05f)] private float _maxLifetime = 6f;

        [Header("Avoidance")]
        [SerializeField] private bool _avoidEnabled = true;
        [SerializeField] private LayerMask _obstacleMask;
        [SerializeField, Min(0f)] private float _avoidRayDistance = 1.2f;
        [SerializeField, Range(0f, 1f)] private float _avoidStrength = 0.6f;

        [Header("Hit Effect")]
        [SerializeField, Min(0f)] private float _hitForce = 2.5f;
        [SerializeField] private ForceMode2D _hitForceMode = ForceMode2D.Impulse;

        // Public getters (read-only from code)
        public SeekingMissileBall MissilePrefab => _missilePrefab;
        public Color Color => _color;

        public float Speed => _speed;
        public float TurnRateDeg => _turnRateDeg;
        public float MaxLifetime => _maxLifetime;

        public bool AvoidEnabled => _avoidEnabled;
        public LayerMask ObstacleMask => _obstacleMask;
        public float AvoidRayDistance => _avoidRayDistance;
        public float AvoidStrength => _avoidStrength;

        public float HitForce => _hitForce;
        public ForceMode2D HitForceMode => _hitForceMode;
    }
}