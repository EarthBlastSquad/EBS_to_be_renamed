using UnityEngine;

namespace Actor
{
    public class AttackActor
    {
        private Animator _anim;
        public AttackActor(Animator anim)
        {
            _anim = anim;
        }

        public void Init(AnimatorOverrideController controller)
        {
            _anim.runtimeAnimatorController = controller;
        }

        public void ShowAttackEffect()
        {
            _anim.SetTrigger("Attack");
        }
    }
}