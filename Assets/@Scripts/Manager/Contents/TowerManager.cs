using Coordinator;
using System.Collections.Generic;
using UnityEngine;

namespace Manager.Contents
{
    public class TowerManager : MonoBehaviour
    {
        private List<TowerCoordinator> _towerCoordinators = new List<TowerCoordinator>(128);
        private Queue<TowerCoordinator> _towerBuffer = new Queue<TowerCoordinator>(8);
        private bool _deadFlag = false;
        private GridManager _gridMgr;
        private bool _rangeDirty = false;

        private void Awake()
        {
            _gridMgr = FindAnyObjectByType<GridManager>();
            _gridMgr.OnMobMovementEvent += OnMobMove;
        }

        private void OnMobMove()
        {
            _rangeDirty = true;
        }

        private void OnTowerDead()
        {
            _deadFlag = true;
        }

        public void AddTower(TowerCoordinator Tower)
        {
            _towerBuffer.Enqueue(Tower);
        }

        private void Update()
        {
            if (Managers.Instance.GameManager.IsGamePaused)
            {
                return;
            }

            for (int i = 0; i < _towerCoordinators.Count; i++)
            {
                if (_towerCoordinators[i].IsDead())
                {
                    continue;
                }

                if (_rangeDirty)
                {
                    _towerCoordinators[i].UpdateState();
                }

                _towerCoordinators[i].Act();
            }
            _rangeDirty = false;
        }

        private void LateUpdate()
        {
            if (Managers.Instance.GameManager.IsGamePaused)
            {
                return;
            }

            if (_deadFlag)
            {
                for (int i = _towerCoordinators.Count - 1; i >= 0; i--)
                {
                    if (_towerCoordinators[i].IsDead())
                    {
                        Managers.Instance.ResourceManager.Destroy(_towerCoordinators[i].gameObject);
                        _towerCoordinators.RemoveAt(i);
                    }
                }

                _deadFlag = false;
            }

            while (_towerCoordinators.Count > 0)
            {
                var tmp = _towerBuffer.Dequeue();
                tmp.SubscribeOnDead(OnTowerDead);
                _towerCoordinators.Add(tmp);
            }
        }
    }
}