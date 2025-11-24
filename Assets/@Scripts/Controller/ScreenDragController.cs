using UnityEngine;

namespace Controller
{
    public class ScreenDragController : MonoBehaviour
    {
        private Vector3 _camOrigin;
        private float _clickedXOrigin;
        private Camera _cam;

        private void Awake()
        {
            _cam = GetComponent<Camera>();

            if(_cam is null)
            {
                Debug.LogError("camera not found");
            }
        }

        private void Update()
        {
            if (Input.GetMouseButton(0) == false)
            {
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                _camOrigin = _cam.transform.position;
                _clickedXOrigin = _cam.ScreenToWorldPoint(Input.mousePosition).x;
            }

            _cam.transform.position = _camOrigin;
            float xPos = _cam.ScreenToWorldPoint(Input.mousePosition).x;
            xPos -= _clickedXOrigin;

            _cam.transform.position = new Vector3(_camOrigin.x - xPos, _camOrigin.y, _camOrigin.z);
        }
    }
}