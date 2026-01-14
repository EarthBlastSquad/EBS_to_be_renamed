using Actor;
using ComponentModule;
using UnityEngine;
using Utils;

namespace Coordinator
{
    public class ProjectileCoordinator : MonoBehaviour
    {
        private ProjectilePosComponentModule _posModule = new ProjectilePosComponentModule();
        private ProjectileMovementActor _actor;
        private BaseSkillCoordinator _skill;
        private float _accumulatedTime = 0;

        private void Awake()
        {
            _skill = GetComponent<BaseSkillCoordinator>();

#if UNITY_EDITOR
            Debug.LogError("스킬이 없습니다.");
#endif
            _actor = gameObject.GetOrAddComponent<ProjectileMovementActor>();
        }
    }
}
