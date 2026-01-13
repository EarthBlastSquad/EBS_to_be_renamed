using ComponentModule;
using Data;
using Manager;
using System;
using UnityEngine;
using Utils;

namespace Coordinator
{
    public class UnitCoordinator : MonoBehaviour
    {
        private HPCoordinator _hpCoordinator;
        private GridMovementCoordinator _gridMovementCoordinator;
        private VictimCoordinator _victim;
        private CooldownComponentModule _module;
        private MonsterData _monsterData;
        private SkillData _skillData;
        private int _id;
        private void Awake()
        {
            _hpCoordinator = gameObject.GetOrAddComponent<HPCoordinator>();
            _victim = gameObject.GetOrAddComponent<VictimCoordinator>();
            _gridMovementCoordinator = gameObject.GetOrAddComponent<GridMovementCoordinator>();
        }

        public void Init(MonsterData data, Vector2Int initialPos)
        {
            _victim.InitVictim(data.InvincibilityTime, data.MonsterHP);
            _gridMovementCoordinator.Init(data.MonsterSpeed,initialPos);
            _id = data.MonsterId;
            _monsterData = data;
            _skillData = Managers.Instance.DataManager.SkillDic[data.SkillID];
            _module = Managers.Instance.CooldownManager.GetCooldownModule(_skillData.Cooldown);
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
            return _hpCoordinator.IsDead();
        }

        public void Act()
        {
            if(_gridMovementCoordinator.Move() == Utils.Defines.MovementReturnTypes.CANT_GO)
            {
                //여기에 공격 로직 작성할 것. 아직 공격 담당 클래스 작성 안됨
            }
        }

        public void SubscribeOnDead(Action onDeadCallback)
        {
            _hpCoordinator.OnDead -= onDeadCallback;
            _hpCoordinator.OnDead += onDeadCallback;
        }
    }
}