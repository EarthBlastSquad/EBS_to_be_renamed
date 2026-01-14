using UnityEngine;
using Utils.Defines;

namespace Actor
{
    public class ProjectileMovementActor : MonoBehaviour
    {
        private Sprite _arrivedImg;
        private SpriteRenderer _renderer;

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
        }

        public void Init(Sprite arrivedImg)
        {
            _arrivedImg = arrivedImg;
        }

        public void Move(Vector3 pos, MovementReturnTypes result)
        {
            if(result == MovementReturnTypes.CANT_GO)
            {
                _renderer.sprite = _arrivedImg;
                transform.rotation = Quaternion.identity;
                return;
            }

            transform.position = pos;
        }
    }
}