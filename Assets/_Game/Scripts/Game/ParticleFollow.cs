using System.Collections.Generic;
using UnityEngine;

namespace LightItUp.Game
{
	[ExecuteInEditMode]
	public class ParticleFollow : MonoBehaviour
	{
		public Transform BlackHole;
		public Vector3 blackHoleOffset = new Vector3(0, 0, 0);
		public List<Transform> joints = new List<Transform>();
		public Material mat;
		public float offset = 2;
		public float followSpeed;
		//[Range(0f, 1f)]
		//public float moveBackCutoff = 0.75f;
		//public AnimationCurve moveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
		ParticleSystem PS;
		ParticleSystem.Particle[] particles;
		List<Vector4> customData;
		[HideInInspector]
		public Vector3 _blackHolePos;


		int aliveParticles;
		int changedCount;

		private void Awake()
		{
			customData = new List<Vector4>();
			PS = GetComponent<ParticleSystem>();
			particles = new ParticleSystem.Particle[PS.main.maxParticles];
		

		}
		void Start()
		{

		}
		void Update()
		{
			_blackHolePos = BlackHole.position; //- transform.position;
			if (PS.particleCount > 0)
			{
				aliveParticles = PS.GetParticles(particles);
				PS.GetCustomParticleData(customData, ParticleSystemCustomData.Custom1);
				changedCount = 0;
				for (int i = 0; i < aliveParticles; i++)
				{
					ParticleSystem.Particle par = particles[i];
					Vector4 d = customData[i];
					if(joints.Count > 0)
					{
						_blackHolePos = joints[(int)((float)i / (float)aliveParticles * (float)joints.Count)].position;
					}
					changedCount++;
					if (d.magnitude < 0.0001f)
					{
						d = par.position;
					}
					par.color = mat.color;
					_blackHolePos -= new Vector3(BlackHole.GetComponent<Rigidbody2D>().velocity.x,BlackHole.GetComponent<Rigidbody2D>().velocity.y,0).normalized*offset;
					par.velocity *= 0;
					par.position = Vector3.Lerp(d, _blackHolePos, Time.deltaTime * followSpeed);

					customData[i] = d;
					particles[i] = par;
				}

			}
			if (changedCount > 0)
			{
				PS.SetParticles(particles, aliveParticles);
				PS.SetCustomParticleData(customData, ParticleSystemCustomData.Custom1);
			}
		}

	}
}
