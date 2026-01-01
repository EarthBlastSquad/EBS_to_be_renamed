using Actor;
using ComponentModule;
using UnityEngine;
using Utils.Defines;

namespace Coordinator
{
    public class GridMovementCoordinator : MonoBehaviour
    {
        private GridPosComponentModule _posMoudle;
        private MovementActor _actor;
        private float _movementSpeed = 5;
        private float _lastCalledTime = 0;

        private void Awake()
        {
            //getoradd로 할지 상의 필요
            _posMoudle = GetComponent<GridPosComponentModule>();
            _actor = GetComponent<MovementActor>();
            if( _posMoudle is null || _actor is null)
            {
                Debug.LogError("GridPosComponentModule 또는 MovementActor또는 그들을 상속받은 클래스가 없습니다");
            }
        }

        public void Init(float speed, Vector2Int initialPos)
        {
            _movementSpeed = speed;
            _lastCalledTime = Time.time;
            _posMoudle.Init(initialPos);
            _actor.Move(initialPos);
        }

        public MovementReturnTypes Move()
        {
            if(Time.time - _lastCalledTime < _movementSpeed )
            {
                return MovementReturnTypes.COOLDOWN_FAILED;
            }

            _lastCalledTime = Time.time;
            Vector2Int pos;
            MovementReturnTypes retType = _posMoudle.TryMove(out pos);
            if (retType == MovementReturnTypes.CANT_GO)
            {
                return MovementReturnTypes.CANT_GO;
            }

            _actor.Move(pos);

            return retType;
        }
    }
}