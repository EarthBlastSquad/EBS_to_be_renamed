using Manager.Contents;
using UnityEngine;
using Utils.Defines;

namespace ComponentModule
{
    public class AirPosComponentModule : GridPosComponentModule
    {
        public AirPosComponentModule(MovementPathManager movementPathMgr)
        {
            _movementPathMgr = movementPathMgr;
        }

        public override Vector2Int GetNextPos()
        {
            return new Vector2Int(_gridPos.x-1, _gridPos.y);
        }

        public override MovementReturnTypes TryMove(out Vector2Int outNextPos)
        {
            outNextPos = _gridPos;

            if(_movementPathMgr.CanMoveInAir(_gridPos) == false)
            {
                return MovementReturnTypes.CANT_GO;
            }

            outNextPos.x--;
            _gridPos.x--;
            Vector2Int futurePos = _gridPos;
            futurePos.x--;

            if (_movementPathMgr.CanMoveInAir(_gridPos) == false)
            {
                return MovementReturnTypes.SUCCESS_AND_BLOCKED;
            }

            return MovementReturnTypes.SUCCESS;
        }
        //
    }
}
