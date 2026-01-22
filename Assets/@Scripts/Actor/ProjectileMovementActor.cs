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
        private Vector3 _end;

        private void Awake()
        {
            _anim = GetComponent<Animator>();
        }

        public void Init(AnimatorOverrideController controller, string launchPSName, string boomPSName, Vector3 start, Vector3 end)
        {
            if(_launchPS is null)
            {
                _launchPS = Managers.Instance.ResourceManager.Instantiate(launchPSName,pooling:true).GetComponent<ParticleSystem>();
            }

            if(_boomPS is null)
            {
                _boomPS = Managers.Instance.ResourceManager.Instantiate(boomPSName,pooling:true).GetComponent<ParticleSystem>();
            }
            _launchPS.transform.position = start;
            _end = end;
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
                _boomPS.transform.position = _end;
                _boomPS.Play();
                _anim.SetTrigger("Boom");
                transform.rotation = Quaternion.identity;
                return;
            }

            transform.position = pos;
        }
    }
}