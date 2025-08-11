using UnityEngine;
using LightItUp.Data;

namespace LightItUp.Game
{
    public class CelebrationFX : PooledObject {
        ParticleSystem ps;
        public ParticleSystem trail;

        void Awake()
        {
            ps = GetComponent<ParticleSystem>();
        }

        public void PlayBox(Transform parent, Vector3 scale)
        {
            Setup(parent, ParticleSystemShapeType.Box, scale, 0, null);
        }
        public void PlayCircle(Transform parent, float radius)
        {
            Setup(parent, ParticleSystemShapeType.Circle, Vector3.one, radius, null);
        }
        public void PlayPolygon(Transform parent, Mesh mesh) {
            Setup(parent, ParticleSystemShapeType.Mesh, Vector3.one, 0, mesh);
        }
        void Setup(Transform parent, ParticleSystemShapeType shapeType, Vector3 scale, float radius, Mesh mesh)
        {
            ps.Stop();
            trail.Stop(true);
            transform.SetParent(parent);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            var shape = ps.shape;
            var trailShape = trail.shape;
            var psMain = ps.main;
            var trailMain = trail.main;


            shape.shapeType = shapeType;
            trailShape.shapeType = shapeType;

            shape.mesh = mesh;
            shape.meshShapeType = ParticleSystemMeshShapeType.Triangle;
            shape.alignToDirection = false;
            shape.scale = scale*1.5f;
            shape.radius = radius*1.5f;

            trailShape.mesh = mesh;
            trailShape.meshShapeType = ParticleSystemMeshShapeType.Triangle;
            trailShape.alignToDirection = false;
            trailShape.scale = scale*1.5f;
            trailShape.radius = radius*1.5f;

            ps.Play();
            ps.Emit(120);
            trail.Play();
            trail.Emit(30);
            enabled = true;

        }
        private void Update()
        {
            if (!ps.isPlaying)
            {
                enabled = false;
                ObjectPool.ReturnCelebrationFX(this);
            }
        }

    }
}
