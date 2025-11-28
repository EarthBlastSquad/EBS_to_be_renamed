using ComponentModule;
using System;
using UnityEngine;

namespace Coordinator
{
    public class HPCoordinator : MonoBehaviour
    {
        private HPComponentModule _hpComponentModule;
        public event Action<int, int, int> OnHPChanged;
        
        private void Awake()
        {
            _hpComponentModule = GetComponent<HPComponentModule>();
            if(_hpComponentModule is null)
            {
#if DEBUG
                Debug.LogError("hpComponentModule찾지 못함");
#endif
            }

        }

        private void OnDestroy()
        {
            OnHPChanged = null;
        }

        public int GetMaxHP()
        {
            return _hpComponentModule.GetMaxHP();
        }

        public int GetHP()
        {
            return _hpComponentModule.GetHP();
        }

        public void TakeDamage(int damage)
        {
            int oldHP = _hpComponentModule.GetHP();
            _hpComponentModule.TakeDamage(damage);
            OnHPChanged?.Invoke(oldHP, _hpComponentModule.GetHP(), _hpComponentModule.GetMaxHP());
        }

        public void InitHP(int maxHp)
        {
            int oldHP = _hpComponentModule.GetHP();
            _hpComponentModule.InitHP(maxHp);
            OnHPChanged?.Invoke(oldHP, _hpComponentModule.GetHP(), _hpComponentModule.GetMaxHP());
        }
    }
}