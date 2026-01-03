using Coordinator;
using System.Collections.Generic;
using UnityEngine;

namespace Manager.Contents
{
    public class UnitManager : MonoBehaviour
    {
        private List<UnitCoordinator> _unitCoordinators = new List<UnitCoordinator>(128);
        private Queue<UnitCoordinator> _unitBuffer = new Queue<UnitCoordinator>(8);

        public void AddUnit(UnitCoordinator unit)
        {
            _unitBuffer.Enqueue(unit);
        }

        private void LateUpdate()
        {
            while(_unitBuffer.Count > 0)
            {
                _unitCoordinators.Add(_unitBuffer.Dequeue());
            }
        }


    }
}