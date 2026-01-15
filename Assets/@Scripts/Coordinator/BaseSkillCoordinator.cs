using Data;
using UnityEngine;

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
