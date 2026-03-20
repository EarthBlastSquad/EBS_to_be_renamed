using DG.Tweening;
using Manager;
using System.Collections;
using UI.Popup;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;
namespace Controller
{
    public class TutorialButtonBlocker : MonoBehaviour, ICanvasRaycastFilter
    {
        private RectTransform _allowedBox;
        private RectTransform _helper;
        private float _cellSize;
        //private Image _image;
        private int _step = 0;
        [SerializeField] private InputActionReference _clickAction;
        private bool _isClicked = false;
        private bool _isDrag = false;
        private const string DRAG_ID = "DragLoop";
        private void Start()
        {
            transform.SetParent(GameObject.Find("@UI_Root").transform, false);
            transform.SetAsLastSibling();
            if (_allowedBox is null)
            {
                GameObject go = Managers.Instance.ResourceManager.Instantiate("AllowedBox", transform.parent, false, false);
                go.transform.SetSiblingIndex(transform.GetSiblingIndex() - 1);
                _allowedBox = go.GetComponent<RectTransform>();
            }
            if (_helper is null)
            {
                _helper = gameObject.GetChild<RectTransform>("Helper");
            }
            _clickAction.action.performed -= OnClickStarted;
            _clickAction.action.performed += OnClickStarted;
            _clickAction.action.Enable();
            _cellSize = 100f;//_allowedBox.GetComponentInParent<Canvas>().scaleFactor;


            UI_Shop uis = FindAnyObjectByType<UI_Shop>(FindObjectsInactive.Include);
            uis.BuyAction -= DragEnd;
            uis.BuyAction += DragEnd;
            //_image=GetComponent<Image>();

            NextStep();
        }
        //private void OnEnable()
        //{
        //    _clickAction.action.started -= OnClickStarted;
        //    _clickAction.action.started += OnClickStarted;
        //}
        private void OnDisable()
        {
            _clickAction.action.started -= OnClickStarted;
            StopDrag();
        }
        private void StopDrag()
        {
            DOTween.Kill(DRAG_ID);
        }
        private void OnClickStarted(InputAction.CallbackContext context)
        {

            Vector2 screenPos = Pointer.current.position.ReadValue();
#if UNITY_EDITOR
            Debug.Log($"sp{screenPos}!");
#endif
            if (RectTransformUtility.RectangleContainsScreenPoint(_allowedBox, screenPos, null) && _isDrag==false)
            {
                _isClicked = true;
            }
        }
        public void DragEnd()
        {
            if (_isDrag == false)
            {
                return;
            }
            _isClicked = true;
            _isDrag = false;
        }
        public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera) //백엔드(유니티)에서 참조하여 갖다 쓰는중임
        {
            if(_isDrag==true)
            {
                return false;
            }
            bool inside = RectTransformUtility.RectangleContainsScreenPoint(_allowedBox, sp, eventCamera);
            return !inside;
        }
        private void LateUpdate()
        {
            if (_isClicked == true)
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
                    SetGrid(new Vector3(-150, -5, 0));
                    Managers.Instance.GameManager.IsGamePaused = false;
                    Managers.Instance.GameManager.IsDragging = true;
                    break;
                case 2:
                    SetStep(FindObject("Slot_0"));
                    Managers.Instance.GameManager.IsDragging = true;
                    break;
                case 3:
                    Time.timeScale = 0.1f;
                    _isDrag = true;
                    SetDrag(new Vector3(-150, -5, 0));
                    Managers.Instance.GameManager.IsDragging = true;
                    break;
                case 4:
                    Managers.Instance.GameManager.IsDragging = false;
                    _allowedBox.gameObject.SetActive(false);
                    gameObject.SetActive(false);
                    Time.timeScale = 1;
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
            if (target.position.y < Screen.height / 2)
            {
                _helper.transform.position = target.position + new Vector3(0, 150, 0);
            }
            else
            {
                _helper.transform.position = target.position + new Vector3(0, -150, 0);
            }

        }
        private void SetGrid(Vector2 pos)
        {
            _allowedBox.anchoredPosition = pos;
            _allowedBox.sizeDelta = new Vector2(_cellSize, _cellSize);
            if (pos.y < 0)
            {
                _helper.transform.position = _allowedBox.position + new Vector3(0, 150, 0);
            }
            else
            {
                _helper.transform.position = _allowedBox.position + new Vector3(0, -150, 0);
            }
        }

        private void SetDrag(Vector2 end)
        {
            _allowedBox.sizeDelta = new Vector2(_cellSize, _cellSize);
            DOTween.Kill(DRAG_ID);

            Vector2 start = _allowedBox.anchoredPosition;

            void Play()
            {
                _allowedBox.anchoredPosition = start;

                _allowedBox.DOAnchorPos(end, 0.06f).SetEase(Ease.OutCubic).SetId(DRAG_ID).OnComplete(Play);
            }

            Play();
        }
    }
}

