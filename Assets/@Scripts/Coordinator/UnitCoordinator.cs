using Actor;
using ComponentModule;
using Data;
using Manager;
using Manager.Contents;
using System;
using UnityEngine;
using Utils;
using Utils.Defines;

namespace Coordinator
{
    public class UnitCoordinator : MonoBehaviour
    {
        private GridMovementCoordinator _gridMovementCoordinator;
        private VictimCoordinator _victim;
        private CooldownComponentModule _module;
        private MonsterData _monsterData;
        private SkillData _skillData;
        private ProjectileManager _projMgr;
        private int _id;
        private int _attackableLayer = 0;
        private LookActor _lookActor;
        private Facing _facing;
        private AttackActor _attackActor;

        private void Awake()
        {
            Animator anim = GetComponent<Animator>();
            _victim = gameObject.GetOrAddComponent<VictimCoordinator>();
            _gridMovementCoordinator = gameObject.GetOrAddComponent<GridMovementCoordinator>();
            _projMgr = FindAnyObjectByType<ProjectileManager>();
            _lookActor = new LookActor(GetComponent<SpriteRenderer>(), transform);
            _attackActor = new AttackActor(anim);
            
        }

        private void Start()
        {
            SubscribeOnDead(OnDead);
        }

        public void Init(MonsterData data, Vector2Int initialPos)
        {
            var animController = Managers.Instance.ResourceManager.Load<AnimatorOverrideController>(data.AnimatorControllerName);
            _victim.InitVictim(data.InvincibilityTime, data.MonsterHP, data.HitSound, animController);
            _gridMovementCoordinator.Init(data.MonsterSpeed,initialPos);
            _id = data.MonsterId;
            _monsterData = data;
            _skillData = Managers.Instance.DataManager.SkillDic[data.SkillID];
            _module = Managers.Instance.CooldownManager.GetCooldownModule(_skillData.Cooldown);
            GetComponent<SpriteRenderer>().sprite = Managers.Instance.ResourceManager.Load<Sprite>(data.MonsterImgName);
            _attackableLayer = 0;
            _attackActor.Init(animController);

            for(int i =0; i < _skillData.AttackableLayers.Count; i++)
            {
                _attackableLayer |= _skillData.AttackableLayers[i];
            }
        }

        private void OnDead()
        {
            Managers.Instance.CurrencyManager.AddCurrency(_monsterData.RewardCurrency);
        }

        private void OnDisable()
        {
            Managers.Instance.CooldownManager.ReturnModule(_module);
        }

        public int GetID()
        {
            return _id; 
        }

        public bool IsDead()
        {
            return _victim.IsDead();
        }

        public void Act()
        {
            if(_module is null)
            {
                return;
            }
            
            var movResult = _gridMovementCoordinator.Move();

            if(movResult == MovementReturnTypes.SUCCESS || movResult == MovementReturnTypes.SUCCESS_AND_BLOCKED)
            {
                var facing = GetFacing(_gridMovementCoordinator.GetNextPos());
                _lookActor.Look(facing);
                _facing = facing;
            }
            else if (movResult == Utils.Defines.MovementReturnTypes.CANT_GO && _module.IsCooldownEnded())
            {
                Vector2Int nowPos = _gridMovementCoordinator.GetNowPos();
                
                bool res = false;
                for(int i = 0; i < _skillData.AttackPos.Count; i++)
                {
                    Vector2Int endPos = nowPos + AreaUtils.CalculateRotation(_skillData.AttackPos[i], _facing);
                    //Vector2Int endPos = nowPos + (_skillData.AttackPos[i] * facing);
                    //if (_skillData.CanAttackMultiple == false && (_projMgr.IsTargetIn(_attackableLayer,endPos) == false))
                    if ((_skillData.CanAttackMultiple || _projMgr.IsTargetIn(_attackableLayer,endPos)) == false)
                    {
                        continue;
                    }

                    _projMgr.CreateProjectile(_skillData,nowPos, endPos);
                    _attackActor.ShowAttackEffect();
                    res = true;

                    if(_skillData.CanAttackMultiple == false)
                    {
                        break;
                    }
                }

                if(res)
                {
                    _module.StartCooldown();
                }
            }
        }

        private Facing GetFacing(Vector2Int next)
        {
            var now = _gridMovementCoordinator.GetNowPos();

            if(next.y < now.y)
            {
                return Facing.DOWN;
            }
            
            if(next.y > now.y)
            {
                return Facing.UP; 
            }

            if(next.x < now.x)
            {
                return Facing.LEFT;
            }

            return Facing.RIGHT;
        }

        public void SubscribeOnDead(Action onDeadCallback)
        {
            _victim.SubscribeOnDead(onDeadCallback);
        }
    }
}