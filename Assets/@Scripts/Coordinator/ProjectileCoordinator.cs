using Actor;
using ComponentModule;
using Data;
using Manager;
using Manager.Contents;
using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using Utils.Defines;

namespace Coordinator
{
    public class ProjectileCoordinator : MonoBehaviour
    {
        private IReadOnlyList<VictimCoordinator> _victims;
        private ProjectilePosComponentModule _posModule = new ProjectilePosComponentModule();
        private ProjectileMovementActor _actor;
        private BaseSkillCoordinator _skill;
        private float _accumulatedTime = 0;
        private AttackManager _attackMgr;
        private float _totalTime = 0;
        private bool _isArrived = false;
        private float _delayTime = 1;
        private SkillData _data;
        public event Action OnProjectileArrived;
        

        private void Awake()
        {
            _skill = GetComponent<BaseSkillCoordinator>();

#if UNITY_EDITOR
            if(_skill is null)
            {
                Debug.LogError("스킬이 없습니다.");
            }
#endif
            _actor = gameObject.GetOrAddComponent<ProjectileMovementActor>();
        }

        public void Init(SkillData data, Vector2Int gridCnt, Vector3 startPos, Vector3 endPos,IReadOnlyList<VictimCoordinator> victims , AttackManager attackMgr, float delayTime)
        {
            _data = data;
            enabled = true;
            _delayTime = delayTime;
            _isArrived = false;
            OnProjectileArrived = null;
            _victims = victims;
            _totalTime = data.SpeedPerCell * (gridCnt.x + gridCnt.y);
            _posModule.Init(startPos, endPos, _totalTime);
            _actor.Init(Managers.Instance.ResourceManager.Load<AnimatorOverrideController>(data.SkillAnimationControllerName), data.SkillLaunchParticleName, data.SkillBOOMParticleName,startPos,endPos);//이부분은 런타임 성능이 너무 떨어진다 싶으면 그때 캐싱으로 바꿔보죠
            _accumulatedTime = 0;
            _attackMgr = attackMgr;
            _skill.Init(data);
        }

        public void Act(float dt)
        {
            _accumulatedTime += dt;

            if(_isArrived)
            {
                if(_accumulatedTime >= _totalTime)
                {
                    OnProjectileArrived?.Invoke();
                    enabled = false;
                }

                return;
            }

            Vector3 pos;
            var result = _posModule.GetPos(_accumulatedTime, out pos);
            _actor.Move(pos, result);

            if(result == Utils.Defines.MovementReturnTypes.CANT_GO)
            {
                _isArrived = true;
                _totalTime += _delayTime;
                Managers.Instance.SoundManager.Play(SoundChannels.EFFECT_0, _data.SFXName, false);
                for (int i = 0; i < _victims.Count; i++)
                {
                    if (_skill.CanAttack(1 << _victims[i].gameObject.layer) == false)
                    {
                        continue;
                    }

                    _attackMgr.RequestAttack((_skill, _victims[i]));

                    if(_skill.CanAttackMultiple() == false)
                    {
                        break;
                    }
                }
            }
        }
    }
}