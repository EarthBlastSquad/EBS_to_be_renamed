using UnityEngine;
using Coordinator;

namespace Contents.Skills
{
    public class TestAttack : BaseSkillCoordinator
    {
        public override bool Act(VictimCoordinator victim)
        {
            victim.TakeDamage(_skillData.Damage);
            return true;
        }
    }
}