using Manager;
using UnityEditor.Animations;
using UnityEngine;
using Utils;
using Utils.Callback;

namespace Actor
{
    public class VictimActor
    {
        private ParticleSystem _particleSystem;
        private Animator _anim;
        private Transform _transform;
        public VictimActor(Animator anim)
        {
            _anim = anim;
        }

        public void Init(AnimatorOverrideController controller, ParticleSystem deadParticle, Transform transform)
        {
            _transform = transform;
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

        public void ShowDieEffect()
        {
            _particleSystem.transform.position = _transform.position;
            _particleSystem.Play();
            _particleSystem.gameObject.GetOrAddComponent<DelayedDestroy>().DestroyAfter(_particleSystem.main.duration);
            _anim.SetTrigger("Die");
        }
    }
}