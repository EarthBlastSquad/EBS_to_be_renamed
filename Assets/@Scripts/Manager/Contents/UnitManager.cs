using Coordinator;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Manager.Contents
{
    public class UnitManager : MonoBehaviour
    {
        private List<UnitCoordinator> _unitCoordinators = new List<UnitCoordinator>(128);
        private Queue<UnitCoordinator> _unitBuffer = new Queue<UnitCoordinator>(8);
        private bool _deadFlag = false;
        public event Action OnUnitDeadEvent;


        public int GetNowUnitCnt()
        {
            return _unitCoordinators.Count + _unitBuffer.Count; 
        }

        private void OnUnitDead()
        {
            _deadFlag = true;
        }

        public void AddUnit(UnitCoordinator unit)
        {
            _unitBuffer.Enqueue(unit);
        }

        private void Update()
        {
            if(Managers.Instance.GameManager.IsGamePaused)
            {
                return; 
            }

            for (int i = 0; i < _unitCoordinators.Count; i++)
            {
                if (_unitCoordinators[i].IsDead())
                {
                    continue;
                }
                _unitCoordinators[i].Act();
            }
        }

        private void LateUpdate()
        {
            if (Managers.Instance.GameManager.IsGamePaused)
            {
                return;
            }

            if (_deadFlag)
            {
                for(int i = _unitCoordinators.Count-1;  i >= 0; i--)
                {
                    if (_unitCoordinators[i].IsDead())
                    {
                        Managers.Instance.ResourceManager.Destroy(_unitCoordinators[i].gameObject);
                        _unitCoordinators.RemoveAt(i);
                    }
                }

                OnUnitDeadEvent?.Invoke();
                _deadFlag = false;
            }

            while(_unitBuffer.Count > 0)
            {
                var tmp = _unitBuffer.Dequeue();
                tmp.SubscribeOnDead(OnUnitDead);
                _unitCoordinators.Add(tmp);
            }
        }


    }
}