using UnityEngine;

namespace LightItUp.Game
{
	public class FollowObject : MonoBehaviour
	{
		public Transform target;
		public Vector3 offset;
		int evenCounter;
		// Use this for initialization
		void Start () {
		
		}
	
		// Update is called once per frame
		void Update ()
		{
			evenCounter++;
			if(evenCounter % 2 == 0)
			{

			}
			Vector3 newPos = new Vector3(target.position.x+ offset.x, target.position.y + offset.y, target.position.z + offset.z);
			transform.position = newPos;
		}
	}
}
