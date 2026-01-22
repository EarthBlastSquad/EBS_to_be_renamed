using UnityEditor.Animations;
using UnityEngine;

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

        public void ShowAttackEffect()
        {
            _anim.SetTrigger("Hit");
        }

        public void ShowDieEffect()
        {
            _particleSystem.Play();
            _anim.SetTrigger("Die");
        }
    }
}