using Manager;
using UnityEngine;
using Utils.Defines;

namespace Actor
{
    public class ProjectileMovementActor : MonoBehaviour
    {
        private Animator _anim;
        private ParticleSystem _launchPS;
        private ParticleSystem _boomPS;

        private void Awake()
        {
            _anim = GetComponent<Animator>();
        }

        public void Init(AnimatorOverrideController controller, string launchPSName, string boomPSName)
        {
            if(_launchPS is null)
            {
                _launchPS = Managers.Instance.ResourceManager.Load<GameObject>(launchPSName).GetComponent<ParticleSystem>();
            }

            if(_boomPS is null)
            {
                _boomPS = Managers.Instance.ResourceManager.Load<GameObject>(boomPSName).GetComponent<ParticleSystem>();
            }
            _boomPS.Clear();
            _launchPS.Clear();
            _launchPS.Play();
            _anim.runtimeAnimatorController = controller;
        }

        public void Move(Vector3 pos, MovementReturnTypes result)
        {
            if(result == MovementReturnTypes.CANT_GO)
            {
                //_renderer.sprite = _arrivedImg;
                _boomPS.Play();
                _anim.SetTrigger("Boom");
                transform.rotation = Quaternion.identity;
                return;
            }

            transform.position = pos;
        }
    }
}