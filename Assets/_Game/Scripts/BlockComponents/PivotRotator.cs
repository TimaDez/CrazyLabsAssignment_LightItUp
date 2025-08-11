using UnityEngine;

namespace LightItUp.Game
{
    [ExecuteInEditMode]
    public class PivotRotator : MonoBehaviour
    {
        public Transform pivot;
        public LineRenderer line;

        public float scaleX, scaleY;

        public Vector3 pivotPosition;
        
        private void OnValidate()
        {
            if (!Application.isPlaying)
                Refresh();
        }

        public void Refresh()
        {
            if (pivot.IsChildOf(transform))
                pivotPosition = pivot.localPosition;
            
            line.SetPositions(new[]
            {
                transform.localPosition, pivotPosition
            });
        }

        public void StartPlay()
        {
            pivot.parent = GetComponentInParent<GameLevel>().transform;
        }

        private void OnEnable()
        {
            if (!Application.isPlaying)
                if (transform.parent != null)
                {
                    pivot.name = transform.parent.name + "_pivot";
                }                       
        }

        private void OnDisable()
        {
            if (Application.isPlaying)
            {
                pivot.parent = transform;
                pivot.localPosition = pivotPosition;
            }
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                if (pivot.IsChildOf(transform))
                    pivotPosition = pivot.localPosition;
                
                line.SetPosition(1, pivotPosition);
               
            }
#endif
        }
    }
}