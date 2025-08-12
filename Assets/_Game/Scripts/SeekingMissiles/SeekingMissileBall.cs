using _Game.Scripts.SeekingMissiles.Models;
using Cysharp.Threading.Tasks;
using LightItUp.Game;
using UnityEngine;

namespace _Game.Scripts.SeekingMissiles
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class SeekingMissileBall : MonoBehaviour
    {
        #region Editor

        [Header("References")]
        [SerializeField] private Rigidbody2D _rb;
        [SerializeField] private Collider2D _collider;
        [SerializeField] private SpriteRenderer _sprite;
        [SerializeField] private ParticleSystem _hitParticle;
        
        #endregion
        
        #region Private members

        private SeekingMissileModel _data;
        private BlockController _target;
        private Vector2 _fallbackAimPoint;
        private float _lifetime;
        private bool _launched;

        // Optional per-instance override (example)
        private float _speedMultiplier = 1f;

        #endregion

        #region Methods
        
        private void Awake()
        {
            if (!_rb)
            {
                Debug.Log($"[SeekingMissileBall] Awake() Rigidbody2D is missing");
                _rb = GetComponent<Rigidbody2D>();
            }
            
            if (!_sprite)
            {
                Debug.Log($"[SeekingMissileBall] Awake() SpriteRenderer is missing");
                _sprite = GetComponentInChildren<SpriteRenderer>();
            }

            if (!_collider)
            {
                Debug.Log($"[SeekingMissileBall] Awake() Collider2D is missing");
                _collider = GetComponent<Collider2D>();
                _collider.isTrigger = true;
            }

            // Apply initial visuals from data
            if (_data != null && _sprite)
                _sprite.color = _data.Color;
        }

        public void SetData(SeekingMissileModel data)
        {
            // Assign model reference and apply visuals
            _data = data;
            if (_data != null && _sprite) 
                _sprite.color = _data.Color;
        }

        public void SetSpeedMultiplier(float multiplier)
        {
            // Per-instance override for dynamic effects
            _speedMultiplier = Mathf.Max(0f, multiplier);
        }

        public void LaunchTowards(Vector2 aimPoint, BlockController target)
        {
            // Initialize runtime state
            _target = target;
            _fallbackAimPoint = aimPoint;
            _lifetime = 0f;
            _launched = true;

            // Reset physics
            _rb.velocity = Vector2.zero;
            _rb.angularVelocity = 0f;

            // Face initial direction
            var dir = (aimPoint - (Vector2)transform.position).normalized;
            if (dir.sqrMagnitude > 0.0001f)
            {
                var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
            }
        }

        private void OnEnable()
        {
            // Reset lifetime when enabled (useful with pooling)
            _lifetime = 0f;
        }

        private void Update()
        {
            if (!_launched || _data == null) 
                return;

            // Lifetime handling
            _lifetime += Time.deltaTime;
            if (_lifetime >= _data.MaxLifetime)
            {
                // Replace with pool return if you use pooling
                Destroy(gameObject);
            }
        }

        private void FixedUpdate()
        {
            if (!_launched || _data == null)
                return;

            // Compute current aim point each physics step
            var targetPoint = GetCurrentAimPoint();
            var desired = (targetPoint - (Vector2)transform.position);
            if (desired.sqrMagnitude < 0.0001f)
            {
                // Keep moving forward if we reached the aim point
                var forward = transform.up;
                _rb.velocity = (Vector2)forward * (_data.Speed * _speedMultiplier);
                return;
            }

            desired.Normalize();

            // Optional obstacle avoidance
            var steerDir = ApplyAvoidanceIfNeeded(desired);

            // Smoothly rotate towards steer direction
            var currentDir = transform.up; // assuming up is forward
            var maxRadians = _data.TurnRateDeg * Mathf.Deg2Rad * Time.fixedDeltaTime;
            var newDir = Vector3.RotateTowards(currentDir, steerDir, maxRadians, 0f);

            // Apply rotation and velocity
            var angle = Mathf.Atan2(newDir.y, newDir.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);

            _rb.velocity = (Vector2)newDir * (_data.Speed * _speedMultiplier);
        }

        private Vector2 GetCurrentAimPoint()
        {
            // If target is missing or already lit, fallback to last aim point
            if (_target == null || _target.IsLit)
                return _fallbackAimPoint;

            // Use closest point on target collider for accurate homing
            var col = _target.col ? _target.col : _target.GetComponent<Collider2D>();
            if (!col || !col.enabled)
                return _fallbackAimPoint;

            var cp = col.ClosestPoint(transform.position);
            _fallbackAimPoint = cp; // keep updating fallback with last closest point
            return cp;
        }

        private Vector2 ApplyAvoidanceIfNeeded(Vector2 desiredDir)
        {
            if (!_data.AvoidEnabled)
                return desiredDir;

            var pos = (Vector2)transform.position;
            var hit = Physics2D.Raycast(pos, desiredDir, _data.AvoidRayDistance, _data.ObstacleMask);
            if (!hit.collider)
                return desiredDir;

            // Lateral steer away from obstacle using surface normal
            var away = Vector2.Perpendicular(hit.normal);
            var left = away.normalized;
            var right = -left;
            var chooseLeft = Vector2.Dot(left, desiredDir) > Vector2.Dot(right, desiredDir);
            var side = chooseLeft ? left : right;

            var steer = Vector2.Lerp(desiredDir, side, _data.AvoidStrength).normalized;
            return steer;
        }

        private async void OnTriggerEnter2D(Collider2D other)
        {
            // Only react to blocks
            var block = other.GetComponent<BlockController>();
            if (!block) 
                return;

            // Ignore already lit blocks (optional)
            if (block.IsLit) 
                return;

            // Light up the block (adapt to your API)
            block.PlayerHit();
            _hitParticle.Play();
            
            // Apply a small push to the hit block if it has a Rigidbody2D
            var blockRb = block.rb2d ? block.rb2d : block.GetComponent<Rigidbody2D>();
            if (blockRb)
            {
                var dir = ((Vector2)other.ClosestPoint(transform.position) - (Vector2)transform.position).normalized;
                if (dir.sqrMagnitude < 0.01f) dir = transform.up;
                blockRb.AddForce(dir * _data.HitForce, _data.HitForceMode);
            }

            // Despawn the missile (replace with pool return if applicable)

            await UniTask.WaitForSeconds(0.2f, cancellationToken: this.GetCancellationTokenOnDestroy());
            Destroy(gameObject);
        }

        private void OnDisable()
        {
            // Clear transient state (helpful with pooling)
            _launched = false;
            _target = null;
            if (_rb)
                _rb.velocity = Vector2.zero;
        }
        
        #endregion
    }
}
