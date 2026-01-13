using ComponentModule;
using Manager;
using UnityEngine;
using Utils;

namespace Coordinator
{
    public class VictimCoordinator : MonoBehaviour
    {
        private CooldownComponentModule _cooldown;
        private HPCoordinator _hpCoordinator;

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

        public void InitVictim(float invincibilityTime, int maxHP)
        {
            _cooldown = Managers.Instance.CooldownManager.GetCooldownModule(invincibilityTime);
            _hpCoordinator.InitHP(maxHP);
        }

        public bool CanAttack()
        {
            return _cooldown.IsCooldownEnded() && (_hpCoordinator.IsDead() == false);
        }

        public void StartCooldown()
        {
            _cooldown.StartCooldown(); 
        }

        public void TakeDamage(int damage)
        {
            _hpCoordinator.TakeDamage(damage);
        }
    }
}