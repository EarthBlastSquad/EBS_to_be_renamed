using Data;
using UnityEngine;

namespace Coordinator
{
    public abstract class BaseSkillCoordinator : MonoBehaviour
    {
        private SkillData _skillData;

        public void Init(SkillData skillData)
        {
            _skillData = skillData;
        }

        public abstract bool Act(VictimCoordinator victim);
    }
}
