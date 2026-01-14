using Actor;
using ComponentModule;
using Data;
using Manager;
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

        public void Init(SkillData data, Vector2Int gridCnt, Vector3 startPos, Vector3 endPos)
        {
            _posModule.Init(startPos, endPos, data.SpeedPerCell*(gridCnt.x + gridCnt.y));
            _actor.Init(Managers.Instance.ResourceManager.Load<Sprite>(data.AttackEffectName), Managers.Instance.ResourceManager.Load<Sprite>(data.AttackObjectImgName));
            _accumulatedTime = 0;
        }
    }
}
