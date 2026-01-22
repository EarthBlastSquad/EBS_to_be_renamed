using Actor;
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
        private VictimActor _actor;
        private Vector2Int _calibrationPos;

        private void Awake()
        {
            _actor = new VictimActor(GetComponent<Animator>());
            _hpCoordinator = gameObject.GetOrAddComponent<HPCoordinator>();
            _hpCoordinator.OnDead += OnDead;
        }

        public void SetCalibrationPos(Vector2Int calibrationPos)
        {
            _calibrationPos = calibrationPos; 
        }

        private void OnDead()
        {
            Managers.Instance.CooldownManager.ReturnModule(_cooldown);
            _cooldown = null;
            _actor.ShowDieEffect(new Vector3(transform.position.x - _calibrationPos.x, transform.position.y - _calibrationPos.y, transform.position.z));
            _actor.OnDead();
        }

        private void OnDestroy()
        {
            Managers.Instance.CooldownManager.ReturnModule(_cooldown);
            _cooldown = null;
        }

        public void InitVictim(float invincibilityTime, int maxHP,string hitSFXName, AnimatorOverrideController animController, string deadParticleName)
        {
            ParticleSystem ps = Managers.Instance.ResourceManager.Instantiate(deadParticleName,pooling:true).GetComponent<ParticleSystem>();
            _actor.Init(animController, ps);
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
            _actor.ShowAttackEffect();
        }

        public bool IsDead()
        {
            return _hpCoordinator.IsDead(); 
        }

        public int GetHP()
        {
            return _hpCoordinator.GetHP(); 
        }
        public void SubscribeOnDead(Action onDeadCallback)
        {
            _hpCoordinator.OnDead -= onDeadCallback;
            _hpCoordinator.OnDead += onDeadCallback;
        }
    }
}