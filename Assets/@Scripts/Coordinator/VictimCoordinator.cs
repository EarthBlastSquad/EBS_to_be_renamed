using ComponentModule;
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
            _cooldown = gameObject.GetOrAddComponent<CooldownComponentModule>();
            _hpCoordinator = gameObject.GetOrAddComponent<HPCoordinator>();
        }

        public void InitVictim(float invincibilityTime, int maxHP)
        {
            _cooldown.InitCooldown(invincibilityTime);
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