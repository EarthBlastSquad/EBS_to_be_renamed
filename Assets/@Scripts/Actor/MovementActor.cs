using Manager.Contents;
using UnityEngine;

namespace Actor
{
    public class MovementActor
    {
        private GridManager _gridMgr;
        private Transform _thisObj;

        public MovementActor(GridManager gridMgr, Transform thisObj)
        {
            _gridMgr = gridMgr;
            _thisObj = thisObj;
        }

        public void Move(Vector2Int pos)
        {
            _gridMgr.MoveTo(pos, _thisObj);
        }
    }
}