using Manager.Contents;
using UnityEngine;
using Utils.Defines;

namespace ComponentModule
{
    public abstract class GridPosComponentModule
    {
        protected Vector2Int _gridPos;
        protected MovementPathManager _movementPathMgr;

        public void Init(Vector2Int initialPos)
        {
            _gridPos = initialPos;
        }

        public Vector2Int GetNowPos()
        {
            return _gridPos; 
        }

        public abstract MovementReturnTypes TryMove(out Vector2Int outNextPos);
    }
}