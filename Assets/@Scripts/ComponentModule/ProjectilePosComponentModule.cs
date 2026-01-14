using Unity.Mathematics;
using UnityEngine;
using Utils.Defines;

namespace ComponentModule
{
    public class ProjectilePosComponentModule
    {
        private Vector3 _startPos;
        private Vector3 _endPos;
        private float _totalTime;

        public void Init(Vector2Int start, Vector2Int end, float zPos, float speedPerTile)
        {
            _totalTime = speedPerTile*(math.abs(start.x - end.x) + math.abs(start.y - end.y));
            _startPos = new Vector3(start.x, start.y, zPos);
            _endPos = new Vector3(end.x, end.y, zPos);
        }

        public MovementReturnTypes GetPos(float accumulatedTime, out Vector3 pos)
        {
            pos = Vector3.Lerp(_startPos,_endPos, accumulatedTime/_totalTime);
            if(accumulatedTime >= _totalTime)
            {
                return MovementReturnTypes.CANT_GO;
            }

            return MovementReturnTypes.SUCCESS;
        }
    }
}
