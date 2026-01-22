using Actor;
using ComponentModule;
using Contents.Grid;
using Data;
using Manager;
using Manager.Contents;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;
using Utils;
using Utils.Defines;

namespace Coordinator
{
    public class TowerCoordinator : MonoBehaviour
    {
        private SkillData _skillData;
        private TowerData _data;
        private CooldownComponentModule _module;
        private VictimCoordinator _victimCoordinator;
        private Facing _facing;
        private GridManager _gridManager;
        private ProjectileManager _projMgr;
        private bool _isAttackState = false;
        private int _attackableLayer = 0;
        private Vector2Int _placedPos; //이건 GridPosComponentModule로 처리하까도 생각했는데, 나중에 생각해보죠. 근데, 그건 이동시스템을 위해 만든건데, 이동시스템이 아직 없으니까 넣는건 너무 섯부른 판단일듯
        private LookActor _lookActor;
        private CrackCoordinator _crackCoordinator;
        private AttackActor _attackActor;

        private void Awake()
        {
            Animator anim = GetComponent<Animator>();
            _victimCoordinator = gameObject.GetOrAddComponent<VictimCoordinator>();
            _gridManager = FindAnyObjectByType<GridManager>();
            _projMgr = FindAnyObjectByType<ProjectileManager>();
            _lookActor = new LookActor(GetComponent<SpriteRenderer>(), transform);
            _crackCoordinator = GetComponentInChildren<CrackCoordinator>();
            _attackActor = new AttackActor(anim);
        }

        public void OnPlaced(bool result)
        {
            if(result)
            {
                _lookActor.Look(_facing);
            }
        }

        public ValueTuple<TowerData, int, SkillData> GetData()
        {
            return (_data, _victimCoordinator.GetHP(), _skillData);
        }

        private void Start()
        {
            SubscribeOnDead(OnDead);
        }

        public void Init(TowerData data, Facing facing, Vector2Int placedPos)
        {
            var animController = Managers.Instance.ResourceManager.Load<AnimatorOverrideController>(data.AnimationClipName);
            _data = data;
            _facing = facing;
            _victimCoordinator.InitVictim(data.InvincibilityTime, data.TowerHP, data.HitSound, animController);
            _skillData = Managers.Instance.DataManager.SkillDic[data.SkillId];
            _placedPos = placedPos;
            _module = Managers.Instance.CooldownManager.GetCooldownModule(_skillData.Cooldown);
            GetComponent<SpriteRenderer>().sprite = Managers.Instance.ResourceManager.Load<Sprite>(data.TowerImgName);
            _attackableLayer = 0;
            _crackCoordinator.Init(data.TowerHP, data.CrackSpriteNameBase);
            _attackActor.Init(animController);

            for(int i = 0; i < _skillData.AttackableLayers.Count; i++)
            {
                _attackableLayer |= _skillData.AttackableLayers[i];
            } 
        }

        public void RetrieveTower()
        {
            Managers.Instance.CurrencyManager.AddCurrency(_data.DemendedCurrency);
            _victimCoordinator.TakeDamage(_data.TowerHP);
        }


        /// <summary>
        /// 공격 가능한 위치(attackPos), 설치된 위치(placedPos), 보는 방향(facing)
        /// </summary>
        /// <returns></returns>
        public ValueTuple<IReadOnlyList<Vector2Int>, Vector2Int, Facing> GetAttackRangeArgs()
        {
            return (_skillData.AttackPos, _placedPos, _facing);
        }

        private void OnDisable()
        {
            Managers.Instance.CooldownManager.ReturnModule(_module);
        }

        public void Act()
        {
            if(_module is null)
            {
                return;
            }
            if(_isAttackState && _module.IsCooldownEnded())
            {
                bool res = false;
                for (int i = 0; i < _skillData.AttackPos.Count; i++)
                {
                    Vector2Int endPos = _placedPos + AreaUtils.CalculateRotation(_skillData.AttackPos[i],_facing);

                    //if(_skillData.CanAttackMultiple == false && (_projMgr.IsTargetIn(_attackableLayer,endPos) == false))
                    if((_skillData.CanAttackMultiple || _projMgr.IsTargetIn(_attackableLayer,endPos)) == false)
                    {
                        continue;
                    }

                    _projMgr.CreateProjectile(_skillData, _placedPos, endPos);
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



        public void UpdateState()
        {
            _isAttackState = CheckMobIsInRange();
        }

        private bool CheckMobIsInRange()
        {
            for(int i = 0; i < _skillData.AttackPos.Count; i++)
            {
                if (_gridManager.TryGetReadonlyVictimList(_placedPos + AreaUtils.CalculateRotation(_skillData.AttackPos[i], _facing), out var list) && list.Count > 1)
                {
                    return true;
                }
            }

            return false;
        }

        private void OnDead()
        {
            _gridManager.RequestPieceUpdate(new PieceUpdateArgs() { command = Utils.Defines.PieceCommandTypes.UNPLACE, commandStatusCallback = null, instance = null, pos = _placedPos});
        }

        public bool IsDead()
        {
            return _victimCoordinator.IsDead();
        }

        public void SubscribeOnDead(Action onDeadCallback)
        {
            _victimCoordinator.SubscribeOnDead(onDeadCallback);
        }
    }
}