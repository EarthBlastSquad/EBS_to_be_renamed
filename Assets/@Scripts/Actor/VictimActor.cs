using Manager;
using UnityEditor.Animations;
using UnityEngine;

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
            Managers.Instance.ResourceManager.Destroy(_particleSystem.gameObject);
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
            _anim.SetTrigger("Die");
        }
    }
}