using Manager;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace InputHandler
{
    public class GridInputHandler : MonoBehaviour
    {
        public event Action<Vector3> mouseDownGridEvent;
        public event Action<Vector3> mouseUpGridEvent;
        public event Action<Vector3> mouseDownSubscriberEvent;
        public event Action<Vector3> mouseUpSubscriberEvent;
        [SerializeField] private InputActionReference clickAction;
        private bool _clickDownRequested, _clickUpRequested, _blockedByUI;
        private Vector3 _cachedPos;
        private void OnEnable()
        {
            clickAction.action.started += OnClickStarted;
            clickAction.action.canceled += OnClickCanceled;
            clickAction.action.Enable();
        }

        private void OnDisable()
        {
            clickAction.action.started -= OnClickStarted;
            clickAction.action.canceled -= OnClickCanceled;
            clickAction.action.Disable();
        }

        private void OnClickStarted(InputAction.CallbackContext context)
        {
            Vector2 screenPos = Pointer.current.position.ReadValue();
            _cachedPos = new Vector3(screenPos.x, screenPos.y, 0f);
            _clickDownRequested = true;
        }

        private void OnClickCanceled(InputAction.CallbackContext context)
        {
            Vector2 screenPos = Pointer.current.position.ReadValue();
            _cachedPos = new Vector3(screenPos.x, screenPos.y, 0f);
            _clickUpRequested = true;
        }
        private void Update()
        {
            if (Managers.Instance.UIManager.IsPopupUIOn || Managers.Instance.GameManager.IsGamePaused)
            {
                return;
            }
            if (_clickDownRequested==true)
            {
                _clickDownRequested = false;

                if (EventSystem.current != null &&
                    EventSystem.current.IsPointerOverGameObject())
                {
                    _blockedByUI = true;
                    return;
                }

                _blockedByUI = false;

                mouseDownGridEvent?.Invoke(_cachedPos);
                mouseDownSubscriberEvent?.Invoke(_cachedPos);
            }
            if (_clickUpRequested==true)
            {
                _clickUpRequested = false;

                if (_blockedByUI)
                {
                    _blockedByUI = false;
                    return;
                }

                mouseUpGridEvent?.Invoke(_cachedPos);
                mouseUpSubscriberEvent?.Invoke(_cachedPos);
            }
        }
        public Vector3 GetCurrentVector3() //이벤트 상시 유지하거나 달았다 뗐다 하기 어려운 상황들을 위한 별도의 함수
        {
            Vector2 screenPos = Pointer.current.position.ReadValue();
            return new Vector3(screenPos.x, screenPos.y, 0f);
        }
        //void Update()
        //{
        //    if(Managers.Instance.UIManager.IsPopupUIOn || Manager.Managers.Instance.GameManager.IsGamePaused)
        //    {
        //        return; 
        //    }
        //    //gamemanager에서 pause상태 읽어서 업데이트 정지하는 로직 짜기
        //    if (Input.GetMouseButtonDown(0))
        //    {
        //        mouseDownGridEvent?.Invoke(Input.mousePosition);
        //        mouseDownSubscriberEvent?.Invoke(Input.mousePosition);
        //    }

        //    if(Input.GetMouseButtonUp(0))
        //    {
        //        mouseUpGridEvent?.Invoke(Input.mousePosition);
        //        mouseUpSubscriberEvent?.Invoke(Input.mousePosition);
        //    }
        //}
    }
}