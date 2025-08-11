using UnityEngine;

namespace LightItUp
{
	public class MirrorBG : MonoBehaviour {
		public SpriteRenderer target;
		SpriteRenderer sr;

		public void UpdateBG ()
		{
			if (sr == null) sr = GetComponent<SpriteRenderer>();
			transform.position = new Vector3(target.transform.position.x,target.transform.position.y,+5f);
			sr.size = target.size;
			transform.localScale = Vector3.one;
			transform.localScale = transform.InverseTransformVector(Vector3.one);
		}

	}
}
