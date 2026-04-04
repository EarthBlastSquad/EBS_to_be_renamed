using DG.Tweening;
using Manager;
using System.Collections;
using TMPro;
using UI.Popup;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;
namespace Controller
{
    public class TutorialButtonBlocker : MonoBehaviour, ICanvasRaycastFilter
    {
        private RectTransform _allowedBox;
        private RectTransform _helper;
        private RectTransform _canvas;
        private TextMeshProUGUI _textMeshProUGUI;
        private float _cellSize;
        //private Image _image;
        private int _step = 0;
        [SerializeField] private InputActionReference _clickAction;
        private bool _isClicked = false;
        private bool _isDrag = false;
        private bool _cancel = false;
        private const string DRAG_ID = "DragLoop";
        private Grid _grid;
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
            _textMeshProUGUI = GetComponentInChildren<TextMeshProUGUI>();

            UI_Shop uis = FindAnyObjectByType<UI_Shop>(FindObjectsInactive.Include);
            uis.BuyAction -= DragEnd;
            uis.BuyAction += DragEnd;
            //_image=GetComponent<Image>();
            _grid=FindAnyObjectByType<Grid>();
            _canvas = FindObject("@UI_Root");
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
            if(_cancel==true)
            {
                return true;
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
                    _textMeshProUGUI.text = "이 버튼이 확장입니다.";
                    Managers.Instance.GameManager.IsGamePaused = true;
                    Time.timeScale = 0;
                    break;
                case 1:
                    SetGrid(new Vector2Int(6,3));
                    _textMeshProUGUI.text = "이 그리드를 클릭해주세요";
                    Managers.Instance.GameManager.IsGamePaused = false;
                    Managers.Instance.GameManager.IsDragging = true;
                    break;
                case 2:
                    SetStep(FindObject("Slot_0"));
                    _textMeshProUGUI.text = "이 타워를 클릭해주세요.\n클릭 시 정보창이 열람됩니다.";
                    Managers.Instance.GameManager.IsDragging = true;
                    break;
                case 3:
                    SetStep(FindObject("TowerDatas_0"));
                    _textMeshProUGUI.text = "이곳은 정보창입니다.";
                    Managers.Instance.GameManager.IsDragging = true;
                    break;
                case 4:
                    SetObjectStep(GameObject.Find("UI_SelectTower").GetComponent<RectTransform>());
                    _textMeshProUGUI.text = "각 설치, 회전, 삭제 입니다.";
                    _cancel = true;
                    Managers.Instance.GameManager.IsDragging = true;
                    break;
                case 5:
                    SetObjectStep(GameObject.Find("UI_SelectTower").GetChild<RectTransform>("Cancel_0").GetComponent<RectTransform>());
                    _textMeshProUGUI.text = "우선 취소를 눌러봅시다.";
                    _cancel = false;
                    Managers.Instance.GameManager.IsDragging = true;
                    break;
                case 6:
                    Time.timeScale = 0.1f;
                    _isDrag = true;
                    SetStep(FindObject("Slot_0"));
                    SetDrag(new Vector2Int(6, 3));
                    _textMeshProUGUI.text = "드래그하여 끌어 놓아 봅시다.";
                    Managers.Instance.GameManager.IsDragging = true;
                    break;
                //case 7:
                //    SetSpriteRenderer(GameObject.Find("StoneBackplate").transform);
                //    _textMeshProUGUI.text = "HP입니다.";
                //    break;
                case 7:
                    DOTween.Kill(DRAG_ID);
                    SetStep(FindObject("Pause_0"));
                    _textMeshProUGUI.text = "일시 정지 버튼입니다.";
                    Managers.Instance.GameManager.IsDragging = true;
                    break;
                case 8:
                    SetStep(FindObject("Back_0"));
                    _textMeshProUGUI.text = "게임으로 돌아가봅시다.";
                    Managers.Instance.GameManager.IsDragging = true;
                    break;
                case 9:
                    Managers.Instance.GameManager.IsDragging = false;
                    Managers.Instance.GameManager.IsGamePaused = false;
                    _isDrag = true;
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
            RectTransform[] all = transform.parent.GetComponentsInChildren<RectTransform>(true);
            foreach (RectTransform t in all)
            {
                if (t.gameObject.name == name)
                {
                    return t;
                }
            }
            return null;
        }
        private void SetSpriteRenderer(Transform target)
        {
            Vector3 worldPos = target.position;
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_allowedBox.parent as RectTransform, screenPos, null, out Vector2 localPos);

            _allowedBox.localPosition = localPos;
            _allowedBox.sizeDelta = new Vector2(_cellSize, _cellSize);
            if (_allowedBox.position.y < Screen.height / 2)
            {
                _helper.transform.position = _allowedBox.position + new Vector3(0, 200, 0);
            }
            else
            {
                _helper.transform.position = _allowedBox.position + new Vector3(0, -200, 0);
            }
        }
        private void SetObjectStep(RectTransform target)
        {
            Vector3 worldPos = target.transform.position;
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_allowedBox.parent as RectTransform , screenPos, null, out Vector2 localPos);
            _allowedBox.localPosition = localPos;
            _allowedBox.sizeDelta = target.sizeDelta*100;

            if (_allowedBox.position.y < Screen.height / 2)
            {
                _helper.transform.position = _allowedBox.position + new Vector3(0, 200, 0);
            }
            else
            {
                _helper.transform.position = _allowedBox.position + new Vector3(0, -200, 0);
            }
        }
        private void SetStep(RectTransform target)
        {
            _allowedBox.position = target.position;
            _allowedBox.sizeDelta = target.sizeDelta;

            if (target.position.y < Screen.height / 2)
            {
                _helper.transform.position = target.position + new Vector3(0, 200, 0);
            }
            else
            {
                _helper.transform.position = target.position + new Vector3(0, -200, 0);
            }
        }
        private void SetGrid(Vector2Int end)
        {
            Vector3 worldPos = _grid.CellToWorld(new Vector3Int(end.x,end.y, 0));
            Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas, screenPos,null,out Vector2 pos);
            _allowedBox.anchoredPosition = pos+new Vector2(51,51);
            _allowedBox.sizeDelta = new Vector2(_cellSize, _cellSize);
            if (pos.y < 0)
            {
                _helper.transform.position = _allowedBox.position + new Vector3(0, 200, 0);
            }
            else
            {
                _helper.transform.position = _allowedBox.position + new Vector3(0, -200, 0);
            }
        }

        private void SetDrag(Vector2Int end)
        {
            Vector3 worldPos = _grid.CellToWorld(new Vector3Int(end.x, end.y, 0));
            Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas, screenPos, null, out Vector2 pos);
            DOTween.Kill(DRAG_ID);
            _allowedBox.sizeDelta = new Vector2(_cellSize, _cellSize);
            Vector2 start = _allowedBox.anchoredPosition;
            void Play()
            {
                _allowedBox.anchoredPosition = start;

                _allowedBox.DOAnchorPos(pos + new Vector2(51, 51), 0.06f).SetEase(Ease.OutCubic).SetId(DRAG_ID).OnComplete(Play);
            }

            Play();
        }

    }
}

