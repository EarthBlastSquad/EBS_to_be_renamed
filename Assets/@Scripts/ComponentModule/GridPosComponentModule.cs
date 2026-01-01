using Manager.Contents;
using UnityEngine;
using Utils.Defines;

namespace ComponentModule
{
    public class GridPosComponentModule : MonoBehaviour
    {
        private Vector2Int _gridPos;
        private MovementPathManager _movementPathMgr;
        private void Awake()
        {
            _movementPathMgr = FindAnyObjectByType<MovementPathManager>();
        }

        public void Init(Vector2Int initialPos)
        {
            _gridPos = initialPos;
        }

        public bool TryMove(out Vector2Int outNextPos)
        {
            outNextPos = _movementPathMgr.GetNextPos(_gridPos);
            if(outNextPos.x == (int)ControlValue.INVALID || _movementPathMgr.CanMoveTo(outNextPos) == false)
            {
                return false;
            }
            _gridPos = outNextPos;
            return true;
        }
    }
}