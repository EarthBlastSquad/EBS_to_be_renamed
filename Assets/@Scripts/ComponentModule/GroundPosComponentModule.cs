using Manager.Contents;
using UnityEngine;
using Utils.Defines;

namespace ComponentModule
{
    public class GroundPosComponentModule : GridPosComponentModule
    {
        public GroundPosComponentModule(MovementPathManager movementPathMgr)
        {
            _movementPathMgr = movementPathMgr;
        }
        public override MovementReturnTypes TryMove(out Vector2Int outNextPos)
        {
            outNextPos = _movementPathMgr.GetNextPos(_gridPos);
            if (_movementPathMgr.CanMoveTo(outNextPos) == false)
            {
                return MovementReturnTypes.CANT_GO;
            }
            _gridPos = outNextPos;

            if (_movementPathMgr.CanMoveTo(_movementPathMgr.GetNextPos(_gridPos)) == false)
            {
                return MovementReturnTypes.SUCCESS_AND_BLOCKED;
            }

            return MovementReturnTypes.SUCCESS;
        }
    }
}