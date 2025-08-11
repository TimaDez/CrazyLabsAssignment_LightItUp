using System.Collections.Generic;
using UnityEngine;
using LightItUp.Data;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LightItUp.Game
{
    public class SpikeRoundController : MonoBehaviour {
        public PolygonCollider2D p;
        public float radius = 1;
        public BlockController.SpikeSetup spikes;
        private void OnValidate()
        {
			#if UNITY_EDITOR
			if (!EditorApplication.isPlayingOrWillChangePlaymode)
			{
				
            SetMesh(radius, spikes);
			}
			#endif
        }
        public void SetMesh(float radius, BlockController.SpikeSetup spikeSetup)
        {
            float min = spikeSetup.minRange;
            float max = spikeSetup.maxRange;
            int partitions = GameSettings.Block.circlePartitions;
            List<Vector2> verts = new List<Vector2>();
            float pr = 1f / partitions;
            bool lastWasSpike = false;
            //verts.Add(Vector3.zero);
            List<int> reverse = new List<int>();
            for (int i = 0; i < partitions+1; i++)
            {
                float a = i * pr;
                Vector2 v = new Vector2(Mathf.Sin(a * 360 * Mathf.Deg2Rad), Mathf.Cos(a * 360 * Mathf.Deg2Rad));

                bool isSpikes = a >= min && a < max;
                if (isSpikes||lastWasSpike)
                {
                    verts.Add(v * (radius + GameSettings.Block.spikeThickness * 0.5f));
                    reverse.Add(i);
                }
                lastWasSpike = isSpikes;
            }
        
            for (int i = reverse.Count - 1; i >= 0; i--)
            {
                float a = reverse[i] * pr;
                Vector2 v = new Vector2(Mathf.Sin(a * 360 * Mathf.Deg2Rad), Mathf.Cos(a * 360 * Mathf.Deg2Rad));
                verts.Add(v * (radius + GameSettings.Block.spikeThickness * 0.5f - GameSettings.Block.spikeColliderThickness));
            }
            p.SetPath(0, verts.ToArray());

        }
    }
}
