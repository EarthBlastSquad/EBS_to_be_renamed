using ComponentModule;
using UnityEngine;
using System.Collections.Generic;

namespace Manager.Contents
{
    public class CooldownManager : MonoBehaviour
    {
        private List<CooldownComponentModule> _cooldownObjects = new List<CooldownComponentModule>(128);
        private int _idx=0;

        private void Awake()
        {
            for(int i = 0; i < 64; i++)
            {
                _cooldownObjects.Add(new CooldownComponentModule());
            }
        }

        public CooldownComponentModule GetCooldownModule(float cooldownTime)
        {
            CooldownComponentModule tmp = null;

            if(_idx >= _cooldownObjects.Count)
            {
                _cooldownObjects.Add(new CooldownComponentModule());
            }

            tmp = _cooldownObjects[_idx];

            tmp.InitCooldown(cooldownTime,_idx);
            _idx++;
            return tmp;
        }

        public void ReturnModule(CooldownComponentModule module)
        {
            if(module is null || _idx <= 0 || module.Index < 0)
            {
                return;
            }

            _idx--;

            var last = _cooldownObjects[_idx];
            _cooldownObjects[_idx] = module;
            _cooldownObjects[module.Index] = last;

            last.Index = module.Index;

            module.DeinitCooldown();
        }

        public void Compact()
        {
            _cooldownObjects.RemoveRange(_idx, _cooldownObjects.Count);
        }

        private void Update()
        {
            if(Managers.Instance.GameManager.IsGamePaused)
            {
                return;
            }

            float dt = Time.deltaTime;
            for(int i = 0; i < _idx; i++)
            {
                _cooldownObjects[i].Tick(dt);
            }
        }

    }
}
