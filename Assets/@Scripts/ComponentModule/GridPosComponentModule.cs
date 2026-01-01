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

        public Vector2Int TryMove()
        {
            Vector2Int next = _movementPathMgr.GetNextPos(_gridPos);
            if(next.x == (int)ControlValue.INVALID)
            {
                return new Vector2Int((int)ControlValue.INVALID, (int)ControlValue.INVALID);
            }
            _gridPos = next;
            return next;
        }
    }
}