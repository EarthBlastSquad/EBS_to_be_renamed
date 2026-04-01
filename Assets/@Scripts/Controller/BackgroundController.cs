using UnityEngine;
using System.Collections.Generic;
namespace Controller
{
    public class BackgroundController : MonoBehaviour
    {
        private LinkedList<Transform> _backgrounds = new LinkedList<Transform>();
        private float _lastX = 0f;
        private Transform _camera;
        private void Start()
        {
            Camera c = Camera.main;
            _camera = c.transform;
            transform.position = new Vector3(_camera.position.x,transform.position.y, transform.position.z);
            _lastX = transform.localPosition.x;
            foreach (Transform t in transform)
            {
                _backgrounds.AddLast(t);
#if UNITY_EDITOR
                Debug.Log(t.name);
#endif
            }
        }
        private void LateUpdate()
        {
            Transform t;
            float clf = _camera.transform.position.x - _lastX;
            if (-19.59f < clf&& clf < 19.59f)
            {
                return;
            }
            else if (clf >= 19.59f)
            {
                t = _backgrounds.First.Value;
                t.transform.position += new Vector3(19.59f * 3, 0, 0);
                _backgrounds.RemoveFirst();
                _backgrounds.AddLast(t);
                _lastX += 19.59f;
            }
            else
            {
                t = _backgrounds.Last.Value;
                t.transform.position -= new Vector3(19.59f*3, 0, 0);
                _backgrounds.RemoveLast();
                _backgrounds.AddFirst(t);
                _lastX -= 19.59f;
            }
        }
    }
}