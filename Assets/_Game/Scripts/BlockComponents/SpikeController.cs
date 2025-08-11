using UnityEngine;

namespace LightItUp.Game
{
    public class SpikeController : MonoBehaviour {
        public SpriteRenderer sr;
        public BoxCollider2D col;

        public void Setup(Vector2 size) {
            col.size = size;
            sr.size = size;
            sr.enabled = false;
        }
    }
}
