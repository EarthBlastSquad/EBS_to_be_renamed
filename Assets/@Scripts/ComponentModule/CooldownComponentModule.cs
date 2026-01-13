using Manager;
using UnityEngine;

namespace ComponentModule
{
    public class CooldownComponentModule
    {
        private float _cooldownTime=0;
        private float _accumulatedTime=0;
        private bool _isCooldownEnded;

        public int Index { get; set; }

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

        public void Tick(float dt)
        {
            if(_isCooldownEnded)
            {
                return;
            }

            _accumulatedTime -= dt;

            if(_accumulatedTime <= 0)
            {
                _isCooldownEnded = true;
            }
        }
    }
}