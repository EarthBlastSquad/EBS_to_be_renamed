using Data;
using Manager;
using UnityEngine;
using Utils.Defines;

namespace Coordinator
{
    public abstract class BaseSkillCoordinator : MonoBehaviour
    {
        protected SkillData _skillData;
        protected int _attackableLayers;

        public void Init(SkillData skillData)
        {
            _skillData = skillData;
            _attackableLayers = 0;
            for(int i = 0; i < _skillData.AttackableLayers.Count; i++)
            {
                _attackableLayers |= (1<<_skillData.AttackableLayers[i]);
            }

            Managers.Instance.SoundManager.Play(SoundChannels.EFFECT_1, _skillData.FiringSFX, false);
        }

        public bool CanAttack(int bitmaskedLayer)
        {
            return (_attackableLayers & bitmaskedLayer) != 0;
        }
        public bool CanAttackMultiple()
        {
            return _skillData.CanAttackMultiple;
        }

        public abstract bool Act(VictimCoordinator victim);
    }
}
