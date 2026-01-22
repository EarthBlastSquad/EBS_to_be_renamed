using UnityEditor.Animations;
using UnityEngine;
using Utils.Defines;

namespace Actor
{
    public class ProjectileMovementActor : MonoBehaviour
    {
        private Animator _anim;

        private void Awake()
        {
            _anim = GetComponent<Animator>();
        }

        public void Init(AnimatorOverrideController controller)
        {
            _anim.runtimeAnimatorController = controller;
        }

        public void Move(Vector3 pos, MovementReturnTypes result)
        {
            if(result == MovementReturnTypes.CANT_GO)
            {
                //_renderer.sprite = _arrivedImg;
                _anim.SetTrigger("Boom");
                transform.rotation = Quaternion.identity;
                return;
            }

            transform.position = pos;
        }
    }
}