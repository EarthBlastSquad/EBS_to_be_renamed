using ComponentModule;
using System;
using UnityEngine;

namespace Coordinator
{
    public class HPCoordinator : MonoBehaviour
    {
        private HPComponentModule _hpComponentModule;
        public event Action<int, int> OnHPChanged;
        
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
    }
}