using UnityEngine;
using LightItUp.Data;

namespace LightItUp.Game
{
    public class ParticleFXBasic : PooledObject
    {
        System.Action<ParticleFXBasic> returnAction;
        public void Init(System.Action<ParticleFXBasic> returnAction)
        {
            this.returnAction = returnAction;
        }
        ParticleSystem ps;
        public void Play()
        {
            ps = GetComponent<ParticleSystem>();
            ps.Clear();
            ps.Play();
        }
        private void Update()
        {
            if (!ps.isPlaying)
            {
                enabled = false;
                returnAction(this);
            }
        }
    }
}