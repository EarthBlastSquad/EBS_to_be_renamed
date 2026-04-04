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
            Vector2 mouse = Pointer.current.position.ReadValue();
            Vector3 mouse3 = new Vector3(mouse.x, mouse.y, -_cam.transform.position.z);
            if (nowCamPos.x < _screenLeftBoundary || nowCamPos.x > _screenRightBoundary)
            {
                nowCamPos.x = Mathf.Clamp(nowCamPos.x, _screenLeftBoundary, _screenRightBoundary);
                _cam.transform.position = nowCamPos;
                _camOrigin = nowCamPos;
                _clickedXOrigin = _cam.ScreenToWorldPoint(mouse3).x;
                return;
            }

            if (Pointer.current.press.isPressed == false)
            {
                return;
            }

            if (Pointer.current.press.wasPressedThisFrame)
            {
                _camOrigin = nowCamPos;
                _clickedXOrigin = _cam.ScreenToWorldPoint(mouse3).x;
            }

            _cam.transform.position = _camOrigin;
            float xPos = _cam.ScreenToWorldPoint(mouse3).x;
            xPos -= _clickedXOrigin;
            _cam.transform.position = new Vector3(_camOrigin.x - xPos, _camOrigin.y, -10);
        }
    }
}