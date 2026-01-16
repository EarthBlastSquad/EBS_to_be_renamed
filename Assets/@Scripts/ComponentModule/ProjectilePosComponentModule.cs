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

        public void Init(Vector3 start, Vector3 end, float totalTime)
        {
            _startPos = start;
            _endPos = end;
            _totalTime = totalTime;
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
