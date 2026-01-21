using Actor;
using ComponentModule;
using UnityEngine;

namespace Coordinator
{
    public class CrackCoordinator : MonoBehaviour
    {
        private CrackActor _actor;
        private CrackComponentModule _module;
        private int _nowIdx;
        private void Awake()
        {
            _module = new CrackComponentModule();
            _actor = new CrackActor(GetComponent<SpriteRenderer>());
        }

        public void Init(int maxHP, string crackSpriteNameBase)
        {
            _nowIdx = -1;
            _module.Init(maxHP);
            _actor.Init(crackSpriteNameBase, _nowIdx);
        }

        public void OnHPChanged(int oldHP, int newHP, int maxHP)
        {
            if(_module.UpdateStage(newHP))
            {
                int stage = _module.GetNowStage();
                if(stage > _nowIdx)
                {
                    _nowIdx = stage;
                    _actor.UpdateCrack(_nowIdx);
                }
            }
        }
    }
}