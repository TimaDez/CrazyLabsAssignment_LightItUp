using System.Collections.Generic;
using UnityEngine;

namespace LightItUp.Game
{
    public class CameraTriggerSizer : MonoBehaviour {
        public Collider2D col;

        public void SetBox(Vector2 size) {
            var col = EnsureColliderType<BoxCollider2D>();
            col.size = size;
        }
        public void SetCircle(float radius)
        {
            var col = EnsureColliderType<CircleCollider2D>();
            col.radius = radius;
        }
        private void LateUpdate()
        {
            transform.rotation = Quaternion.identity;
        }

        T EnsureColliderType<T>() where T : Collider2D
        {
            if (col == null)
            {
                col = GetComponent<Collider2D>();
            }
            var b = col as T;
            if (b == null)
            {
                List<Collider2D> allColliders = new List<Collider2D>();
                allColliders.AddRange(GetComponents<Collider2D>());
                if (allColliders.Count > 0)
                {
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.delayCall += () =>
                    {
                        foreach (var v in allColliders)
                        {
                            DestroyImmediate(v);
                        }
                    };
#else 
                foreach (var v in allColliders)
                {
                    Destroy(v);
                }
#endif
                }
                b = gameObject.AddComponent<T>();
                b.isTrigger = true;
                b.gameObject.layer = LayerMask.NameToLayer("CameraFocus");
                col = b;
            }
            return b;
        }
    }
}
