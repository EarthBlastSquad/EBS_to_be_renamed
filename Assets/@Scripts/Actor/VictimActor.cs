using UnityEditor.Animations;
using UnityEngine;

namespace Actor
{
    public class VictimActor
    {
        private Animator _anim;
        public VictimActor(Animator anim)
        {
            _anim = anim;
        }

        public void Init(AnimatorController controller)
        {
            _anim.runtimeAnimatorController = controller;
        }

        public void ShowAttackEffect()
        {
            _anim.SetTrigger("Hit");
        }

        public void ShowDieEffect()
        {
            _anim.SetTrigger("Die");
        }
    }
}