using UnityEngine;

namespace LightItUp.Game
{
    public class FollowTarget : MonoBehaviour
    {
        public Transform target;
        float elasicPower = 0.09f;
        private void Start()
        {
            if (target == null)
            {
                target = FindObjectOfType<PlayerController>().transform;
            }

            Vector3 p = target.position;
            p.z = transform.position.z;
            transform.position = p;
        }
        void LateUpdate () {

            Vector2 start = transform.position;
            Vector2 end = target.position;
            Vector3 p = Vector2.Lerp(start, end, elasicPower);
            p.z = transform.position.z;
            transform.position = p;

        }
    }
}
