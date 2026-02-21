using UnityEngine;
using Utils.Defines;
using UnityEngine.InputSystem;

namespace Controller
{
    public class ScreenDragController : MonoBehaviour
    {
        private Vector3 _camOrigin;
        private float _clickedXOrigin;
        private Camera _cam;
        private float _screenLeftBoundary;
        private float _screenRightBoundary;

        private void Awake()
        {
            _cam = GetComponent<Camera>();

            if(_cam is null)
            {
                Debug.LogError("camera not found");
            }
            float halfWidth = (_cam.orthographicSize) * _cam.aspect;
            _screenLeftBoundary = _cam.transform.position.x - halfWidth;
            _screenRightBoundary = (int)MapMaxCellCnt.MAX_WIDTH + _cam.transform.position.x + halfWidth;
        }

        private void Update()
        {
            if (Manager.Managers.Instance.GameManager.IsGamePaused|| Manager.Managers.Instance.GameManager.IsDragging)
            {
                return;
            }

            Vector3 nowCamPos = _cam.transform.position;

            if(nowCamPos.x < _screenLeftBoundary || nowCamPos.x > _screenRightBoundary)
            {
                nowCamPos.x = Mathf.Clamp(nowCamPos.x, _screenLeftBoundary, _screenRightBoundary);
                _cam.transform.position = nowCamPos;
                _camOrigin = nowCamPos;
                _clickedXOrigin = _cam.ScreenToWorldPoint(Input.mousePosition).x;
                return;
            }

            if (Input.GetMouseButton(0) == false)
            {
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                _camOrigin = nowCamPos;
                _clickedXOrigin = _cam.ScreenToWorldPoint(Input.mousePosition).x;
            }

            _cam.transform.position = _camOrigin;
            float xPos = _cam.ScreenToWorldPoint(Input.mousePosition).x;
            xPos -= _clickedXOrigin;
            _cam.transform.position = new Vector3(_camOrigin.x - xPos, _camOrigin.y, _camOrigin.z);
        }
    }
}