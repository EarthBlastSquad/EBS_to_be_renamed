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
        private GridMovementCoordinator _gridMovementCoordinator;
        private VictimCoordinator _victim;
        private CooldownComponentModule _module;
        private MonsterData _monsterData;
        private SkillData _skillData;
        private int _id;
        private void Awake()
        {
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
            GetComponent<SpriteRenderer>().sprite = Managers.Instance.ResourceManager.Load<Sprite>(data.MonsterImgName);
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
            if(_gridMovementCoordinator.Move() == Utils.Defines.MovementReturnTypes.CANT_GO && _module.IsCooldownEnded())
            {
                //여기에 공격 로직 작성할 것. 아직 공격 담당 클래스 작성 안됨

                _module.StartCooldown();
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