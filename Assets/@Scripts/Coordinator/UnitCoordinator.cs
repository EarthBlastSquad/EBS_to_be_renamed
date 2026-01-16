using ComponentModule;
using Data;
using Manager;
using Manager.Contents;
using System;
using UnityEngine;
using Utils;

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
        private void Awake()
        {
            _victim = gameObject.GetOrAddComponent<VictimCoordinator>();
            _gridMovementCoordinator = gameObject.GetOrAddComponent<GridMovementCoordinator>();
            _projMgr = FindAnyObjectByType<ProjectileManager>();
            
        }

        private void Start()
        {
            SubscribeOnDead(OnDead);
        }

        public void Init(MonsterData data, Vector2Int initialPos)
        {
            _victim.InitVictim(data.InvincibilityTime, data.MonsterHP);
            _gridMovementCoordinator.Init(data.MonsterSpeed,initialPos);
            _id = data.MonsterId;
            _monsterData = data;
            _skillData = Managers.Instance.DataManager.SkillDic[data.SkillID];
            _module = Managers.Instance.CooldownManager.GetCooldownModule(_skillData.Cooldown);
            GetComponent<SpriteRenderer>().sprite = Managers.Instance.ResourceManager.Load<Sprite>(data.MonsterImgName);
            _attackableLayer = 0;
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
            if(_gridMovementCoordinator.Move() == Utils.Defines.MovementReturnTypes.CANT_GO && _module.IsCooldownEnded())
            {
                Vector2Int nowPos = _gridMovementCoordinator.GetNowPos();
                Vector2Int facing = _gridMovementCoordinator.GetNextPos();
                GetFacing(ref facing);
                bool res = false;
                for(int i = 0; i < _skillData.AttackPos.Count; i++)
                {
                    Vector2Int endPos = nowPos + (_skillData.AttackPos[i] * facing);
                    //if (_skillData.CanAttackMultiple == false && (_projMgr.IsTargetIn(_attackableLayer,endPos) == false))
                    if ((_skillData.CanAttackMultiple || _projMgr.IsTargetIn(_attackableLayer,endPos)) == false)
                    {
                        continue;
                    }

                    _projMgr.CreateProjectile(_skillData,nowPos, endPos);
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

        private void GetFacing(ref Vector2Int next)
        {
            var now = _gridMovementCoordinator.GetNowPos();
            next.x = next.x < now.x ? -1 : 1;
            next.y = next.y < now.y ? -1 : 1;
        }

        public void SubscribeOnDead(Action onDeadCallback)
        {
            _victim.SubscribeOnDead(onDeadCallback);
        }
    }
}