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

        public int GetID()
        {
            return _id; 
        }

        public bool IsDead()
        {
            return _hpCoordinaotr.IsDead();
        }

        public void Act()
        {
            if(_gridMovementCoordinator.Move() == Utils.Defines.MovementReturnTypes.CANT_GO)
            {
                //여기에 공격 로직 작성할 것. 아직 공격 담당 클래스 작성 안됨
            }
        }
    }
}