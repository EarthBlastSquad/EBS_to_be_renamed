using ComponentModule;
using System;
using UnityEngine;

namespace Coordinator
{
    public class HPCoordinator : MonoBehaviour
    {
        private HPComponentModule _HPComponentModule;
        private bool _isDead = false;
        public event Action<int, int, int> OnHPChanged;
        public event Action OnDead;
        
        private void Awake()
        {
            _HPComponentModule = GetComponent<HPComponentModule>();
            if(_HPComponentModule is null)
            {
#if DEBUG
                Debug.LogError("hpComponentModule찾지 못함");
#endif
            }

        }

        private void OnDestroy()
        {
            OnHPChanged = null;
            OnDead = null;
        }

        public int GetMaxHP()
        {
            return _HPComponentModule.GetMaxHP();
        }

        public int GetHP()
        {
            return _HPComponentModule.GetHP();
        }

        public void TakeDamage(int damage)
        {
            if(_isDead)
            {
                return;
            }

            int oldHP = _HPComponentModule.GetHP();
            _HPComponentModule.TakeDamage(damage);
            OnHPChanged?.Invoke(oldHP, _HPComponentModule.GetHP(), _HPComponentModule.GetMaxHP());

            if(_HPComponentModule.GetHP() <= 0)
            {
                _isDead = true;
                OnDead?.Invoke();
            }
        }

        public void InitHP(int maxHP)
        {
            int oldHP = _HPComponentModule.GetHP();
            _HPComponentModule.InitHP(maxHP);
            OnHPChanged?.Invoke(oldHP, _HPComponentModule.GetHP(), _HPComponentModule.GetMaxHP());

            if(_HPComponentModule.GetHP() > 0)
            {
                _isDead = false;
            }
        }

        public bool IsDead()
        {
            return _isDead;
        }
    }
}