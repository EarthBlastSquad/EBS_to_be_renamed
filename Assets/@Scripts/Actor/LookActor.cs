using UnityEngine;
using Utils.Defines;

namespace Actor
{
    public class LookActor
    {
        private Transform _transform;
        public LookActor(Transform transform)
        {
            _transform = transform;
        }

        public Vector2Int Look(Facing facing)
        {
            _transform.rotation = Quaternion.identity;
            var pos = _transform.position;
            Vector2Int calibrationPos = Vector2Int.zero;

            if(facing == Facing.LEFT)
            {
                _transform.Rotate(new Vector3(0,0,(float)facing));
                pos.x += 1;
                pos.y += 1;
                calibrationPos.x = 1;
                calibrationPos.y = 1;
            }
            else if(facing == Facing.UP)
            {
                _transform.Rotate(new Vector3(0,0,(float)facing));
                pos.x += 1;
                calibrationPos.x = 1;
            }
            else if(facing == Facing.DOWN)
            {
                _transform.Rotate(new Vector3(0, 0, (float)facing));
                pos.y += 1;
                calibrationPos.y = 1;
            }

            _transform.position = pos;
            return calibrationPos;
        }
    }
}