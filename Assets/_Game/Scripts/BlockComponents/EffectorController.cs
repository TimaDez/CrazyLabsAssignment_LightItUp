using UnityEngine;

namespace LightItUp.Game
{
	public class EffectorController : MonoBehaviour {
		public CircleCollider2D col;
		public PointEffector2D effector;
		public SpriteRenderer sr;
		public ParticleSystem attractParticles;
		public ParticleSystem deflectParticles;
		public void Setup (float radius, float power, Color color, float damper) {
			effector.forceMagnitude = power;
			effector.drag = damper;
			col.radius = radius;
			name = power < 0 ? "Attractor" : "Deflector";
			//sr.sprite = (power < 0) ? SpriteAssets.Instance.effectorAttractArrow : SpriteAssets.Instance.effectorDeflectArrow;
			attractParticles.gameObject.SetActive(power < 0);
			deflectParticles.gameObject.SetActive(power >= 0);
			var s = attractParticles.shape;
			var m = attractParticles.main;
			m.startSize = radius;
			m.startColor = color;
			var dm = deflectParticles.main;
			dm.startSize = radius;
			dm.startColor = color;

			s.radius = radius;
		}

	}
}
