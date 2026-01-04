using Actor;
using ComponentModule;
using Manager.Contents;
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
            var gridMgr = FindAnyObjectByType<GridManager>();
            var pathMgr = FindAnyObjectByType<MovementPathManager>();

            if( gridMgr is null || pathMgr is null )
            {
#if UNITY_EDITOR
                Debug.LogError("GridManager 또는 MovementPathManager가 존제하지 않습니다.");
#endif
            }

            if(LayerMask.NameToLayer("AirMovementMob") == gameObject.layer)
            {
                _posMoudle = new AirPosComponentModule(pathMgr);
            }
            else
            {
                _posMoudle = new GroundPosComponentModule(pathMgr);
            }

            _actor = new MovementActor(gridMgr, gameObject.transform);
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