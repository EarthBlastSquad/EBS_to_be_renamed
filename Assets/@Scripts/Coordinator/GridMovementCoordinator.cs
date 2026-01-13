using Actor;
using ComponentModule;
using Manager.Contents;
using UnityEngine;
using Utils;
using Utils.Defines;

namespace Coordinator
{
    public class GridMovementCoordinator : MonoBehaviour
    {
        private GridPosComponentModule _posMoudle;
        private MovementActor _actor;
        private float _movementSpeed = 5;
        private float _lastCalledTime = 0;
        private GridManager _gridMgr;
        private VictimCoordinator _victim;

        private void Awake()
        {
            _gridMgr = FindAnyObjectByType<GridManager>();
            var pathMgr = FindAnyObjectByType<MovementPathManager>();

            if(_gridMgr is null || pathMgr is null )
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

            _actor = new MovementActor(_gridMgr, gameObject.transform);
            _victim = gameObject.GetOrAddComponent<VictimCoordinator>();
            gameObject.GetOrAddComponent<HPCoordinator>().OnDead += OnDead;
        }

        public Vector2Int GetNextPos()
        {
            return _posMoudle.GetNextPos();
        }

        public Vector2Int GetNowPos()
        {
            return _posMoudle.GetNowPos();
        }

        public void Init(float speed, Vector2Int initialPos)
        {
            _movementSpeed = speed;
            _lastCalledTime = Time.time;
            _posMoudle.Init(initialPos);
            _actor.Move(initialPos);
        }

        private void OnDead()
        {
            _gridMgr.UnplaceMobAt(_posMoudle.GetNowPos(), _victim);
        }

        public MovementReturnTypes Move()
        {
            if(Time.time - _lastCalledTime < _movementSpeed )
            {
                return MovementReturnTypes.COOLDOWN_FAILED;
            }

            _lastCalledTime = Time.time;
            Vector2Int pos;
            Vector2Int oldPos = _posMoudle.GetNowPos();
            MovementReturnTypes retType = _posMoudle.TryMove(out pos);
            if (retType == MovementReturnTypes.CANT_GO)
            {
                return MovementReturnTypes.CANT_GO;
            }

            _actor.Move(pos);

            _gridMgr.UnplaceMobAt(oldPos,_victim);//방어로직의 필요성이 아직 없다
            _gridMgr.PlaceMobAt(pos,_victim);

            return retType;
        }
    }
}