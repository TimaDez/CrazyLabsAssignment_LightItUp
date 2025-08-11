using System.Linq;
using LightItUp.Data;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LightItUp.Game
{
    public class ReviveZone : MonoBehaviour
    {
        public enum Mode
        {
            SafeZone, NoRevive
        }
        
        [System.Serializable]
        public class ReviveZoneData {
            public string name;
            public Vector2[] positions;
            public int mode;
            public Vector2 safeSpotPosition;
        }
        
        public PolygonCollider2D collider;
        public Mode mode;
        public Transform safeSpot;
        public GameObject safeBox;
        
        public bool safeSpotFollowsCollider;
        
        public ReviveZoneData GetData() {
            return new ReviveZoneData {
                name = name,
                positions = collider.points,
                mode = (int)mode,
                safeSpotPosition = safeSpot.position
            };
        }
    
        void Awake () {
            Setup();

        }
        private void OnValidate()
        {
			#if UNITY_EDITOR
			if (!EditorApplication.isPlayingOrWillChangePlaymode)
			{
				
            Setup();
			}
			#endif
        }

        void Setup () {
           
            safeSpot.gameObject.SetActive(mode == Mode.SafeZone);

            if (safeSpotFollowsCollider && safeSpot.gameObject.activeInHierarchy)
            {
                safeSpot.position = collider.bounds.center;
                safeSpotFollowsCollider = false;
            }
        }

        public void Load(ReviveZoneData data)
        {
            name = data.name?? "ReviveZone";
            Setup();
            
            if (data.positions != null && data.positions.Length > 0)
                collider.points = data.positions;
            mode = (Mode) data.mode;
            safeSpot.position = data.safeSpotPosition;            
        }
        
        private void OnDrawGizmos()
        {
            if (collider == null)
                return;

            if (GameSettings.Level.drawZonesHighlights)
            {
                Color color = mode == Mode.SafeZone?GameSettings.Level.safeColor:GameSettings.Level.unsafeColor;
            
#if UNITY_EDITOR

                var points = collider.points.Select(x => new Vector3(x.x, x.y, 0)).ToArray();

                UnityEditor.Handles.color = color;
            
                UnityEditor.Handles.DrawAAConvexPolygon(points);
            
#endif
            }
 }
        
//        T EnsureColliderType<T>() where T : Collider2D
//        {
//            if (collider == null)
//            {
//                col = GetComponent<Collider2D>();
//            }
//            var b = col as T;
//            if (b == null)
//            {
//                List<Collider2D> allColliders = new List<Collider2D>();
//                allColliders.AddRange(GetComponents<Collider2D>());
//                if (allColliders.Count > 0)
//                {
//#if UNITY_EDITOR
//                    UnityEditor.EditorApplication.delayCall += () =>
//                    {
//                        foreach (var v in allColliders)
//                        {
//                            DestroyImmediate(v);
//                        }                    
//                    };
//#else
//                foreach (var v in allColliders)
//                {
//                    Destroy(v);
//                }
//#endif
//                }
//                b = gameObject.AddComponent<T>();
//                col = b;
//            }
//            return b;
//        }
    }
}