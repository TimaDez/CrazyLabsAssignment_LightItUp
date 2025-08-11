using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightItUp.Data;
using LightItUp.Sound;

namespace LightItUp.Game
{
    public class CollectibleController : MonoBehaviour {
    
        public CollectibleEffect effect;
        Rigidbody2D rb2d;
        Collider2D col;
        Collider2D childCol;
        public AnimationCurve easeIn = AnimationCurve.Linear(0, 0, 1, 1);
        public Rect worldRect
        {
            get
            {
                if (col == null)
                    col = GetComponent<Collider2D>();
                if (childCol == null) {
                    childCol = transform.GetChild(0).GetComponent<Collider2D>();
                }
                return new Rect(transform.position - col.bounds.extents, col.bounds.size);
            }
        }

        Rigidbody2D target;
        Vector2 startPos;
        Vector2 travel;
        Vector2 vel;
        float t = 0;
        public void MoveToTransform(Rigidbody2D otherRb2d)
        {
            if (rb2d == null) rb2d = GetComponent<Rigidbody2D>();
            col.enabled = false;
            childCol.enabled = false;
            vel = rb2d.velocity;
            target = otherRb2d;
            startPos = rb2d.position;
            rb2d.bodyType = RigidbodyType2D.Kinematic;
        }
        void Update() {
            if (target != null) {
                float duration = .2f;
                t += Time.deltaTime/ duration;
                vel *= 0.9f;
                travel += vel * Time.fixedDeltaTime;            
                rb2d.transform.position = (Vector2.Lerp(startPos+ travel, target.transform.position, t));
                if (t >= 1)
                {
                    var starFX = ObjectPool.GetStarFX();
                    SoundManager.PlaySound("CollectStar");
                    starFX.transform.position = transform.position;
                    starFX.Play();
                    Destroy(gameObject);
                }
            }
        }
    }
    public enum CollectibleEffect
    {
        None,
        InfiniteJumps,
        HomingJumps,
        LightWave,
        Taser,
        Star
    }
}