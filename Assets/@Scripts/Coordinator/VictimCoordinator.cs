using ComponentModule;
using UnityEngine;
using Utils;

namespace Coordinator
{
    public class VictimCoordinator : MonoBehaviour
    {
        private CooldownComponentModule _cooldown;

        private void Awake()
        {
            _cooldown = gameObject.GetOrAddComponent<CooldownComponentModule>();
        }

        public void InitVictim(float invincibilityTime)
        {
            _cooldown.InitCooldown(invincibilityTime);
        }
    }
}