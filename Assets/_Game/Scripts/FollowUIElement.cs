using UnityEngine;

namespace LightItUp
{
	[ExecuteInEditMode]
	public class FollowUIElement : MonoBehaviour
	{
		public Camera cam;
		public GameObject target;
		Vector3 targetPosWS;
		public float zOffset= 0;
		void Start ()
		{
			
		}

		void LateUpdate ()
		{
			if (cam == null)
			{
				cam = Camera.main;				
			}
			
			//targetPosSS = Camera.main.WorldToScreenPoint(target.transform.position);
			targetPosWS = cam.ScreenToWorldPoint(target.transform.position);
			targetPosWS = target.transform.position;
			transform.position = new Vector3 (targetPosWS.x,targetPosWS.y, zOffset);
		}
	}
}
