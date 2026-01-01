using Manager.Contents;
using UnityEngine;

namespace Actor
{
    public class MovementActor : MonoBehaviour
    {
        private GridManager _gridMgr;

        private void Awake()
        {
            _gridMgr = FindAnyObjectByType<GridManager>();
        }
        public void Move(Vector2Int pos)
        {
            _gridMgr.MoveTo(pos, gameObject.transform);
        }
    }
}