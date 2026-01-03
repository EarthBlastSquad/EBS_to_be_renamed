using Data;
using UnityEngine;
using Utils;

namespace Coordinator
{
    public class UnitCoordinator : MonoBehaviour
    {
        private HPCoordinator _hpCoordinaotr;
        private GridMovementCoordinator _gridMovementCoordinator;
        private int _id;
        private void Awake()
        {
            _hpCoordinaotr = gameObject.GetOrAddComponent<HPCoordinator>();
            _gridMovementCoordinator = gameObject.GetOrAddComponent<GridMovementCoordinator>();
        }

        public void Init(MonsterData data, Vector2Int initialPos)
        {
            _hpCoordinaotr.InitHP(data.MonsterHP);
            _gridMovementCoordinator.Init(data.MonsterSpeed,initialPos);
            _id = data.MonsterId;
        }
    }
}