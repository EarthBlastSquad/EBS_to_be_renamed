using ComponentModule;
using Manager;
using System;
using UnityEngine;
using Utils;

namespace Coordinator
{
    public class VictimCoordinator : MonoBehaviour
    {
        private CooldownComponentModule _cooldown;
        private HPCoordinator _hpCoordinator;
        private string _hitSFXName;

        private void Awake()
        {
            _hpCoordinator = gameObject.GetOrAddComponent<HPCoordinator>();
            _hpCoordinator.OnDead += OnDead;
        }

        private void OnDead()
        {
            Managers.Instance.CooldownManager.ReturnModule(_cooldown);
            _cooldown = null;
        }

        private void OnDestroy()
        {
            Managers.Instance.CooldownManager.ReturnModule(_cooldown);
            _cooldown = null;
        }

        public void InitVictim(float invincibilityTime, int maxHP,string hitSFXName)
        {
            _hitSFXName= hitSFXName;
            _cooldown = Managers.Instance.CooldownManager.GetCooldownModule(invincibilityTime);
            _hpCoordinator.InitHP(maxHP);
        }

        public bool CanAttack()
        {
            if(_cooldown is null)
            {
                return false;
            }
            return _cooldown.IsCooldownEnded() && (_hpCoordinator.IsDead() == false);
        }

        public void StartCooldown()
        {
            if(_cooldown is not null)
            {
                _cooldown.StartCooldown();
            }
        }

        public void TakeDamage(int damage)
        {
            Managers.Instance.SoundManager.Play(Utils.Defines.SoundChannels.EFFECT_0, _hitSFXName, false);
            _hpCoordinator.TakeDamage(damage);
        }

        public bool IsDead()
        {
            return _hpCoordinator.IsDead(); 
        }

        public void SubscribeOnDead(Action onDeadCallback)
        {
            _hpCoordinator.OnDead -= onDeadCallback;
            _hpCoordinator.OnDead += onDeadCallback;
        }
    }
}