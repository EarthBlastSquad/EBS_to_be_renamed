using Manager;
using UnityEngine;

namespace ComponentModule
{
    public class CooldownComponentModule : MonoBehaviour
    {
        private float _cooldownTime=0;
        private float _accumulatedTime=0;
        private bool _isCooldownEnded;

        public void InitCooldown(float cooldownTime)
        {
            _cooldownTime = cooldownTime;
            _accumulatedTime = 0;
            _isCooldownEnded = true;
        }

        public void StartCooldown()
        {
            _accumulatedTime = _cooldownTime;
            _isCooldownEnded=false;
        }

        public bool IsCooldownEnded()
        {
            return _isCooldownEnded; 
        }

        private void Update()
        {
            if(Managers.Instance.GameManager.IsGamePaused || _accumulatedTime <= 0)
            {
                return;
            }

            _accumulatedTime -= Time.deltaTime;

            if(_accumulatedTime <= 0)
            {
                _isCooldownEnded = true;
            }
        }
    }
}