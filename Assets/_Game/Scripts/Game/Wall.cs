using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LightItUp.Game
{
    public class Wall : MonoBehaviour {
        SpriteRenderer sr;
        [HideInInspector]
        public BoxCollider2D boxCol;
        public Vector2 size;

        public bool disableOnJump = false;
        
        public Rect worldRect
        {
            get
            {
                return new Rect(transform.position - sr.bounds.extents, sr.bounds.size);
            }
        }

        [System.Serializable]
        public class WallData {
            public string name;
            public Vector2 position;
            public Quaternion rotation;
            public Vector2 size;
            public bool disableOnJump;
        }
        public WallData GetWallData() {
            return new WallData {
                name = name,
                position = transform.localPosition,
                rotation = transform.localRotation,
                size = size,
                disableOnJump = disableOnJump
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
            if (sr == null) sr = GetComponent<SpriteRenderer>();
            if (boxCol == null) boxCol = GetComponent<BoxCollider2D>();
            sr.drawMode = SpriteDrawMode.Tiled;
            sr.size = size;
            boxCol.size = size;
        }

        public void Load(WallData data)
        {
            if (string.IsNullOrEmpty(data.name)) name = "Wall";
            else name = data.name;
            transform.localPosition = data.position;
            transform.localRotation = data.rotation;
            size = data.size;
            disableOnJump = data.disableOnJump;
            Setup();
        }
    }
}
