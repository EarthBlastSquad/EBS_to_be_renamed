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
    }
}