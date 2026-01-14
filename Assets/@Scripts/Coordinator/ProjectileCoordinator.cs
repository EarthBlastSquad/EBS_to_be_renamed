using Actor;
using ComponentModule;
using Data;
using Manager;
using Manager.Contents;
using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Coordinator
{
    public class ProjectileCoordinator : MonoBehaviour
    {
        private IReadOnlyList<VictimCoordinator> _victims;
        private VictimCoordinator _victimTower;//이거는 나중에 성능 보고 합쳐보도록 하죠
        private ProjectilePosComponentModule _posModule = new ProjectilePosComponentModule();
        private ProjectileMovementActor _actor;
        private BaseSkillCoordinator _skill;
        private float _accumulatedTime = 0;
        private AttackManager _attackMgr;

        private void Awake()
        {
            _skill = GetComponent<BaseSkillCoordinator>();

#if UNITY_EDITOR
            Debug.LogError("스킬이 없습니다.");
#endif
            _actor = gameObject.GetOrAddComponent<ProjectileMovementActor>();
        }

        public void Init(SkillData data, Vector2Int gridCnt, Vector3 startPos, Vector3 endPos, IReadOnlyList<VictimCoordinator> victims, VictimCoordinator victim, AttackManager attackMgr)
        {
            _victims = victims;
            _victimTower = victim;
            _posModule.Init(startPos, endPos, data.SpeedPerCell*(gridCnt.x + gridCnt.y));
            _actor.Init(Managers.Instance.ResourceManager.Load<Sprite>(data.AttackEffectName), Managers.Instance.ResourceManager.Load<Sprite>(data.AttackObjectImgName));//이부분은 런타임 성능이 너무 떨어진다 싶으면 그때 캐싱으로 바꿔보죠
            _accumulatedTime = 0;
            _attackMgr = attackMgr;
        }

        public void Act(float dt)
        {
            _accumulatedTime += dt;
            Vector3 pos;
            var result = _posModule.GetPos(_accumulatedTime, out pos);
            _actor.Move(pos, result);

            if(result == Utils.Defines.MovementReturnTypes.CANT_GO)
            {
                for (int i = 0; i < _victims.Count; i++)
                {
                    if (_skill.CanAttack( 1 << _victims[i].gameObject.layer))
                    {
                        _attackMgr.RequestAttack((_skill,_victims[i]));
                    }
                }
                
                if(_victimTower is not null)
                {
                    if(_skill.CanAttack(1 << _victimTower.gameObject.layer))
                    {
                        _attackMgr.RequestAttack(new ValueTuple<BaseSkillCoordinator, VictimCoordinator>(_skill, _victimTower));
                    }
                }
            }
        }
    }
}
