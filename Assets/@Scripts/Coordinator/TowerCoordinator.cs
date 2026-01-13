using ComponentModule;
using Data;
using Manager;
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

        private void Awake()
        {
            _victimCoordinator = gameObject.GetOrAddComponent<VictimCoordinator>();
        }

        public void Init(TowerData data, Vector2Int facing)
        {
            _facing = facing;
            _victimCoordinator.InitVictim(data.InvincibilityTime, data.TowerHP);
            _skillData = Managers.Instance.DataManager.SkillDic[data.SkillId];
        }
    }
}