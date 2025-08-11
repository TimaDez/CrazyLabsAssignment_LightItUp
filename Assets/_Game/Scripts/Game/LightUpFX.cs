using UnityEngine;
using LightItUp.Data;

namespace LightItUp.Game
{
    public class LightUpFX : PooledObject {
        ParticleSystem ps;
        public ParticleSystem glow;
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
            transform.SetParent(parent);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            var shape = ps.shape;
            var trailShape = trail.shape;
            var psMain = ps.main;
            var trailMain = trail.main;
            var glowMain = glow.main;


            shape.shapeType = shapeType;
            trailShape.shapeType = shapeType;

            shape.mesh = mesh;
            shape.meshShapeType = ParticleSystemMeshShapeType.Triangle;
            shape.alignToDirection = false;
            shape.scale = scale;
            shape.radius = radius;

            trailShape.mesh = mesh;
            trailShape.meshShapeType = ParticleSystemMeshShapeType.Triangle;
            trailShape.alignToDirection = false;
            trailShape.scale = scale;
            trailShape.radius = radius;
            psMain.startColor = parent.GetComponent<BlockController>().color;
            trailMain.startColor = parent.GetComponent<BlockController>().color;
            glowMain.startColor = parent.GetComponent<BlockController>().color;
            ps.Play();
            ps.Emit(80);
            trail.Emit(20);
            glow.Emit(1);
            enabled = true;

        }
        private void Update()
        {
            if (!ps.isPlaying)
            {
                enabled = false;
                ObjectPool.ReturnLightUpFX(this);
            }
        }

    }
}
