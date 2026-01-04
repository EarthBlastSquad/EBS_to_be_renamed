using Manager;
using System;
using UnityEngine;

namespace InputHandler
{
    public class GridInputHandler : MonoBehaviour
    {
        public event Action<Vector3> mouseDownGridEvent;
        public event Action<Vector3> mouseUpGridEvent;
        public event Action<Vector3> mouseDownSubscriberEvent;
        public event Action<Vector3> mouseUpSubscriberEvent;

        void Update()
        {
            if(Managers.Instance.UIManager.IsPopupUIOn || Manager.Managers.Instance.GameManager.IsGamePaused)
            {
                return; 
            }
            //gamemanager에서 pause상태 읽어서 업데이트 정지하는 로직 짜기
            if (Input.GetMouseButtonDown(0))
            {
                mouseDownGridEvent?.Invoke(Input.mousePosition);
                mouseDownSubscriberEvent?.Invoke(Input.mousePosition);
            }
            
            if(Input.GetMouseButtonUp(0))
            {
                mouseUpGridEvent?.Invoke(Input.mousePosition);
                mouseUpSubscriberEvent?.Invoke(Input.mousePosition);
            }
        }
    }
}