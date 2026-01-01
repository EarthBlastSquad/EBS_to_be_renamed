using Manager.Contents;
using UnityEngine;
using Utils.Defines;

namespace ComponentModule
{
    public abstract class GridPosComponentModule : MonoBehaviour
    {
        protected Vector2Int _gridPos;
        protected MovementPathManager _movementPathMgr;
        private void Awake()
        {
            _movementPathMgr = FindAnyObjectByType<MovementPathManager>();
        }

        public void Init(Vector2Int initialPos)
        {
            _gridPos = initialPos;
        }

        public abstract MovementReturnTypes TryMove(out Vector2Int outNextPos);
    }
}