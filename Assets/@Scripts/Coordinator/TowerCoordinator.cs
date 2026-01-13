using ComponentModule;
using Data;
using Manager;
using Manager.Contents;
using UnityEngine;
using Utils;

namespace Coordinator
{
    public class TowerCoordinator : MonoBehaviour
    {
        private SkillData _skillData;
        private TowerData _data;
        private CooldownComponentModule _module;
        private VictimCoordinator _victimCoordinator;
        private Vector2Int _facing;
        private GridManager _gridManager;
        private bool _isAttackState = false;

        private Vector2Int _placedPos; //이건 GridPosComponentModule로 처리하까도 생각했는데, 나중에 생각해보죠. 근데, 그건 이동시스템을 위해 만든건데, 이동시스템이 아직 없으니까 넣는건 너무 섯부른 판단일듯

        private void Awake()
        {
            _victimCoordinator = gameObject.GetOrAddComponent<VictimCoordinator>();
            _gridManager = FindAnyObjectByType<GridManager>();
        }

        public void Init(TowerData data, Vector2Int facing, Vector2Int placedPos)
        {
            _facing = facing;
            _victimCoordinator.InitVictim(data.InvincibilityTime, data.TowerHP);
            _skillData = Managers.Instance.DataManager.SkillDic[data.SkillId];
            _placedPos = placedPos;
            _module = Managers.Instance.CooldownManager.GetCooldownModule(_skillData.Cooldown);
        }

        private void OnDisable()
        {
            Managers.Instance.CooldownManager.ReturnModule(_module);
        }

        public void Act()
        {
            if(_isAttackState && _module.IsCooldownEnded())
            {
                //공격 로직 짜기

                _module.StartCooldown();
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
                if (_gridManager.TryGetReadonlyVictimList(_placedPos + (_skillData.AttackPos[i]*_facing), out var list) && list.Count > 0)
                {
                    return true;
                }
            }

            return false;
        }
    }
}