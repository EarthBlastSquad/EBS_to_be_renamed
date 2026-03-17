using Manager;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;
using System.Collections;
namespace Controller
{
    public class TutorialButtonBlocker : MonoBehaviour, ICanvasRaycastFilter
    {
        private RectTransform _allowedBox;
        private RectTransform _helper;
        //private Image _image;
        private int _step = 0;
        [SerializeField] private InputActionReference _clickAction;
        private bool _isClicked = false;

        private void Start()
        {
            transform.SetParent(GameObject.Find("@UI_Root").transform, false);
            transform.SetAsLastSibling();
            if (_allowedBox is null)
            {
                GameObject go= Managers.Instance.ResourceManager.Instantiate("AllowedBox", transform.parent, false, false);
                go.transform.SetSiblingIndex(transform.GetSiblingIndex()-1);
                _allowedBox = go.GetComponent<RectTransform>();
            }
            if(_helper is null)
            {
                _helper=gameObject.GetChild<RectTransform>("Helper");
            }
            _clickAction.action.performed -= OnClickStarted;
            _clickAction.action.performed += OnClickStarted;
            _clickAction.action.Enable();
            //_image=GetComponent<Image>();

            NextStep();
        }
        private void OnDestroy()
        {
            _clickAction.action.started -= OnClickStarted;
        }
        private void OnClickStarted(InputAction.CallbackContext context)
        {

            Vector2 screenPos = Pointer.current.position.ReadValue();
#if UNITY_EDITOR
            Debug.Log($"sp{screenPos}!");
#endif
            if (RectTransformUtility.RectangleContainsScreenPoint(_allowedBox, screenPos, null))
            {
                _isClicked = true;
            }
        }
        public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera) //백엔드(유니티)에서 참조하여 갖다 쓰는중임
        {
            bool inside = RectTransformUtility.RectangleContainsScreenPoint(_allowedBox, sp, eventCamera);
            return !inside;
        }
        private void LateUpdate()
        {
            if (_isClicked==true)
            {
                _isClicked = false;
                StartCoroutine(NextStepNextFrame());
            }
        }
        private IEnumerator NextStepNextFrame()
        {
            yield return new WaitUntil(() => !Pointer.current.press.isPressed);

            NextStep();
        }
        public void NextStep()
        {

            switch (_step++)
            {
                case 0:
                    SetStep(FindObject("Unlock_0")); //임시
                    Managers.Instance.GameManager.IsGamePaused = true;
                    Time.timeScale = 0;
                    break;
                case 1:
                    SetGrid(new Vector3(-150, -0.05f, 0));
                    Managers.Instance.GameManager.IsGamePaused = false;
                    break;
                case 2:
                    SetStep(FindObject("Slot_0"));
                    break;

                case 3:
                    Time.timeScale = 1;
                    _allowedBox.gameObject.SetActive(false);
                    gameObject.SetActive(false);
                    break;
            }
#if UNITY_EDITOR
            Debug.Log($"{_step}step");
#endif
        }
        private RectTransform FindObject(string name)
        {
            UnityEngine.UI.Button[] all = transform.parent.GetComponentsInChildren<UnityEngine.UI.Button>(true);
            foreach (UnityEngine.UI.Button t in all)
            {
                if (t.name == name)
                {
                    return t.GetComponent<RectTransform>();
                }
            }
            return null;
        }
        private void SetStep(RectTransform target)
        {
            _allowedBox.position = target.position;
            _allowedBox.sizeDelta = target.sizeDelta;
            if(target.position.y< Screen.height / 2)
            {
                _helper.transform.position = target.position+new Vector3(0,150,0);
            }
            else
            {
                _helper.transform.position = target.position + new Vector3(0, -150, 0);
            }
                
        }
        private void SetGrid(Vector2 pos)
        {
            _allowedBox.anchoredPosition = pos;
            _allowedBox.sizeDelta = new Vector2(100, 100);
            if (pos.y < Screen.height / 2)
            {
                _helper.transform.position = _allowedBox.position + new Vector3(0, 150, 0);
            }
            else
            {
                _helper.transform.position = _allowedBox.position + new Vector3(0, -150, 0);
            }
        }
    }
}

