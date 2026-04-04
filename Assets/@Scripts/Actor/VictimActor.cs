using Manager;
using UnityEngine;
using Utils;
using Utils.Callback;

namespace Actor
{
    public class VictimActor
    {
        private ParticleSystem _particleSystem;
        private Animator _anim;
        public VictimActor(Animator anim)
        {
            _anim = anim;
        }

        public void Init(AnimatorOverrideController controller, ParticleSystem deadParticle)
        {
            deadParticle?.Clear();
            _particleSystem = deadParticle;
            _anim.runtimeAnimatorController = controller;
        }

        public void OnDead()
        {
            _particleSystem = null;
        }

        public void ShowAttackEffect()
        {
            _anim.SetTrigger("Hit");
        }

        public void ShowDieEffect(Vector3 pos)
        {
            _particleSystem.transform.position = pos;
            _particleSystem.Play();
            _particleSystem.gameObject.GetOrAddComponent<DelayedDestroy>().DestroyAfter(_particleSystem.main.duration);
            _anim.SetTrigger("Die");
        }
    }
}