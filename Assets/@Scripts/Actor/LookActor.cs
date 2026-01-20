using UnityEngine;
using Utils.Defines;

namespace Actor
{
    public class LookActor
    {
        private SpriteRenderer _renderer;
        private Transform _transform;
        public LookActor(SpriteRenderer renderer, Transform transform)
        {
            _renderer = renderer;
            _transform = transform;
        }

        public void Look(Facing facing)
        {
            _renderer.flipX = false;
            _renderer.flipY = false;
            _transform.rotation = Quaternion.identity;
            var pos = _transform.position;

            if(facing == Facing.LEFT)
            {
                _renderer.flipX= true;
                pos.x += 1;
            }
            else if(facing == Facing.UP)
            {
                _transform.Rotate(new Vector3(0,0,(float)facing));
                pos.x += 1;
            }
            else if(facing == Facing.DOWN)
            {
                _transform.Rotate(new Vector3(0, 0, (float)facing));
                pos.y += 1;
            }

            _transform.position = pos;
        }
    }
}