using System.Collections.Generic;
using UnityEngine;
using LightItUp.Singletons;

namespace LightItUp.Game
{
    public class CameraFollow : SingletonScene<CameraFollow> {
        public Transform target;
        BlockController lastTouched;
        BlockController nextBlock;
        protected override void Awake()
        {
            if (target == null)
            {
                target = FindObjectOfType<PlayerController>().transform;
            }
        }
        private void Start()
        {
            nextBlock = GetNextTarget();

            Rect area = GetRect();
            Vector3 wantedPos = area.center;
            wantedPos.z = transform.position.z;
            transform.position = wantedPos;
        }
        public void PlayerTouched(BlockController bl) {
            if (bl.IsLit) {
                lastTouched = bl;
            }
            nextBlock = GetNextTarget();
        }
        BlockController GetNextTarget() {
        
            List<BlockController> l = new List<BlockController>();
            l.AddRange(FindObjectsOfType<BlockController>());
            l.RemoveAll(x => x.IsLit);

            Debug.Log(l.Count);
            BlockController bl = null;
            float dist = float.MaxValue;
            foreach (var b in l)
            {
                Vector2 t = target != null ? target.position : Vector3.zero;
                Vector2 bp = b.transform.position;
                var d = (t - bp).magnitude;
                if (d < dist)
                {
                    dist = d;
                    bl = b;
                }
            }
            return bl;
        }
        Rect GetRect() {
            Rect area = new Rect(target.position.x, target.position.y, 0, 0);
            if (nextBlock != null)
            {
                area = AddPointToRect(area, nextBlock.transform.position);
            }
            if (lastTouched != null)
            {
                area = AddPointToRect(area, lastTouched.transform.position);
            }
            Debug.DrawLine(area.min, new Vector3(area.xMin, area.yMax));
            Debug.DrawLine(area.min, new Vector3(area.xMax, area.yMin));

            Debug.DrawLine(area.max, new Vector3(area.xMin, area.yMax));
            Debug.DrawLine(area.max, new Vector3(area.xMax, area.yMin));
            return area;
        }
        void LateUpdate () {
            Rect area = GetRect();
            Vector3 wantedPos = area.center;
            wantedPos.z = transform.position.z;
            transform.position = Vector3.Lerp(transform.position, wantedPos, 0.01f);
        }
        Rect AddPointToRect(Rect area, Vector2 p) {
            area.xMax = Mathf.Max(area.xMax, p.x);
            area.xMin = Mathf.Min(area.xMin, p.x);
            area.yMax = Mathf.Max(area.yMax, p.y);
            area.yMin = Mathf.Min(area.yMin, p.y);
            return area;
        }
    }
}
