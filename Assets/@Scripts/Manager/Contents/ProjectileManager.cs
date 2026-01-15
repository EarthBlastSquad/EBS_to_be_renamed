using Coordinator;
using Data;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Utils;

namespace Manager.Contents
{
    public class ProjectileManager : MonoBehaviour
    {
        private List<ProjectileCoordinator> _projectiles = new List<ProjectileCoordinator>(64);
        private GridManager _gridManager;
        private AttackManager _attackMgr;
        private bool _dirty = false;

        private void Awake()
        {
            _gridManager = FindAnyObjectByType<GridManager>();
            _attackMgr = FindAnyObjectByType<AttackManager>();
            if(_gridManager is null)
            {
#if UNITY_EDITOR
                Debug.LogError("그리드매니저 없음");
#endif
            }
            if(_attackMgr is null)
            {
#if UNITY_EDITOR
                Debug.LogError("공격매니저 없음");
#endif
            }
        }

        private void OnProjectileArrived()
        {
            _dirty = true;
        }

        public bool IsTargetIn(int attackableMask, Vector2Int end)
        {
            IReadOnlyList<VictimCoordinator> victims;
            if (_gridManager.TryGetReadonlyVictimList(end, out victims) == false)
            {
                return false;
            }

            for(int i = 0;  i < victims.Count; i++)
            {
                if ((victims[i].gameObject.layer & attackableMask) != 0)
                {
                    return true;
                }
            }

            return false;
        }

        public void CreateProjectile(SkillData skillData,Vector2Int start, Vector2Int end)
        {
            IReadOnlyList<VictimCoordinator> victims;
            if (_gridManager.TryGetReadonlyVictimList(end, out victims) == false)
            {
                return;
            }
            var go = Managers.Instance.ResourceManager.Instantiate(skillData.PrefabName, pooling: true);

            if(go is null)
            {
#if UNITY_EDITOR
                Debug.LogError($"{skillData.PrefabName} 인 공격 프리펩은 없음");
#endif
                return;
            }

            var projectile = go.GetOrAddComponent<ProjectileCoordinator>();

            projectile.Init(skillData,
                new Vector2Int(math.abs(start.x - end.x) , math.abs(start.y-end.y)),
                _gridManager.GetWorldPos(start,0),
                _gridManager.GetWorldPos(end,0),
                victims,
                _attackMgr,
                1
                );
            projectile.OnProjectileArrived += OnProjectileArrived;
            _projectiles.Add(projectile);
        }

        private void Update()
        {
            if(Managers.Instance.GameManager.IsGamePaused)
            {
                return;
            }

            float dt = Time.deltaTime;

            for(int i = 0;  i < _projectiles.Count; i++)
            {
                _projectiles[i].Act(dt);
            }
        }

        private void LateUpdate()
        {
            if (_dirty)
            {
                _dirty = false;
                for (int i = _projectiles.Count - 1; i >= 0; i--)
                {
                    if (_projectiles[i].enabled == false)
                    {
                        Managers.Instance.ResourceManager.Destroy(_projectiles[i].gameObject);
                        _projectiles.RemoveAt(i);
                    }
                }
            }
        }

    }
}
