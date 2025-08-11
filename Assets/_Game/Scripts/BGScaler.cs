using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
namespace LightItUp
{
	[ExecuteInEditMode]
	public class BGScaler : MonoBehaviour {
		public SpriteRenderer sr;
		public BoxCollider2D lCollider;
		public BoxCollider2D rCollider;
		public BoxCollider2D tCollider;
		public BoxCollider2D bCollider;
		public MirrorBG mirrorBG;
		private void Awake()
		{
			LateUpdate();
		}
		private void OnValidate()
		{
			#if UNITY_EDITOR
			if (!EditorApplication.isPlayingOrWillChangePlaymode)
			{
			LateUpdate();
			}
			#endif
		}
		void LateUpdate ()
		{
			if (Camera.main == null || Camera.main.orthographicSize <= 0 || float.IsNaN(Camera.main.orthographicSize) || Screen.width <= 0 || Screen.height <= 0)
			{
				sr.size = new Vector2(10, 10);
				return;
			}
			if (sr == null) sr = GetComponent<SpriteRenderer>();
			float h = Camera.main.orthographicSize * 2;
			float w = Screen.width;
			w /= Screen.height;
			w *= h;
			sr.size = new Vector2(w, h);
			mirrorBG.UpdateBG();
			if (lCollider != null)
			{
				lCollider.size = new Vector2(1, h);
				lCollider.offset = new Vector2((w / 2) + (lCollider.size.x / 2), 0);

				rCollider.size = new Vector2(1, h);
				rCollider.offset = new Vector2(-1 * ((w / 2) + (rCollider.size.x / 2)), 0);

				tCollider.size = new Vector2(w, 1);
				tCollider.offset = new Vector2(0, (h / 2) + (tCollider.size.y / 2));

				bCollider.size = new Vector2(w, 1);
				bCollider.offset = new Vector2(0, -1 * ((h / 2) + (bCollider.size.y / 2)));
			}



		}
	}
}
