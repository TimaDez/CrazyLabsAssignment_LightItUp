using UnityEngine;
using LightItUp.Data;

namespace LightItUp.Game
{
    public class LitArea : PooledObject
    {
        public string clipName;
        Animator anim;
        private void Awake()
        {
            anim = GetComponent<Animator>();
        }
        public void Play()
        {
            anim.Play(clipName);
        }
        void AnimationComplete()
        {
            ObjectPool.ReturnLitArea(this);
        }
    }
}
