using Controller;
using Data;
using DG.Tweening;
using InputHandler;
using Manager;
using Manager.Contents;
using ObjectPool;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;
using Utils.Defines;
namespace UI.Popup
{

    public class UI_Shop : UIPopup
    {
        public bool ShopOpen = false;

        enum Buttons
        {
            Slot_0=0, 
            Slot_1=1, 
            Slot_2=2,
            Slot_3=3,
        }
        public override bool Init()
        {
            if (base.Init() == false)
                return false;
            BindButton(typeof(Buttons));
            for (int i = 0; i < 3; i++)
            {
                _towerData[i] = Managers.Instance.GameManager.EquippedTowers[i].TowerData;
                GetButton((int)Buttons.Slot_0+i).gameObject.GetChildGameObject("PreviewImage").GetComponent<Image>().sprite=Managers.Instance.ResourceManager.Load<Sprite>(_towerData[i].TowerImgName);
                GetButton((int)Buttons.Slot_0 + i).gameObject.GetComponentInChildren<TextMeshProUGUI>().text = _towerData[i].DemendedCurrency.ToString();
                _images[i]= GetButton((int)Buttons.Slot_0 + i).GetComponent<Image>();
            }
            _towerData[3] = Managers.Instance.DataManager.TowerDic[0];
            GetButton((int)Buttons.Slot_3).gameObject.GetChildGameObject("PreviewImage").GetComponent<Image>().sprite = Managers.Instance.ResourceManager.Load<Sprite>(_towerData[3].TowerImgName);
            GetButton((int)Buttons.Slot_3).gameObject.GetComponentInChildren<TextMeshProUGUI>().text = _towerData[3].DemendedCurrency.ToString();
            _images[3] = GetButton((int)Buttons.Slot_3).GetComponent<Image>();

            GetButton((int)Buttons.Slot_0).gameObject.BindUIEvent((_) => SlotDown((int)Buttons.Slot_0, _),UIEventTypes.POINTER_DOWN);
            GetButton((int)Buttons.Slot_1).gameObject.BindUIEvent((_) => SlotDown((int)Buttons.Slot_1, _), UIEventTypes.POINTER_DOWN);
            GetButton((int)Buttons.Slot_2).gameObject.BindUIEvent((_) => SlotDown((int)Buttons.Slot_2, _), UIEventTypes.POINTER_DOWN);
            GetButton((int)Buttons.Slot_3).gameObject.BindUIEvent((_) => SlotDown((int)Buttons.Slot_3, _), UIEventTypes.POINTER_DOWN);
            GetButton((int)Buttons.Slot_0).gameObject.BindUIEvent(SlotUp, UIEventTypes.POINTER_UP);
            GetButton((int)Buttons.Slot_1).gameObject.BindUIEvent(SlotUp, UIEventTypes.POINTER_UP);
            GetButton((int)Buttons.Slot_2).gameObject.BindUIEvent(SlotUp, UIEventTypes.POINTER_UP);
            GetButton((int)Buttons.Slot_3).gameObject.BindUIEvent(SlotUp, UIEventTypes.POINTER_UP);
            _tm =FindAnyObjectByType<TowerManager>();
            _selectTower = GameObject.Find("SelectTower").transform.GetComponentInChildren<SpriteRenderer>();
            _uiSelectTower = GameObject.Find("UI_SelectTower").GetComponent<RectTransform>();

            _uiSelectTower.gameObject.GetChild<Button>("Buy_0").gameObject.BindUIEvent(Buy);
            _uiSelectTower.gameObject.GetChild<Button>("Rotate_0").gameObject.BindUIEvent(ChangeFacing);
            _uiSelectTower.gameObject.GetChild<Button>("Cancel_0").gameObject.BindUIEvent(Cancel);

            _gc = FindAnyObjectByType<GridController>();
            _rp = FindAnyObjectByType<RangePreview>();
            _gh = FindAnyObjectByType<GridInputHandler>();
            _g = _gh.GetComponent<Grid>();

            _sprites[0] = Managers.Instance.ResourceManager.Load<Sprite>(_towerData[0].TowerImgName);
            _sprites[1] = Managers.Instance.ResourceManager.Load<Sprite>(_towerData[1].TowerImgName);
            _sprites[2] = Managers.Instance.ResourceManager.Load<Sprite>(_towerData[2].TowerImgName);
            _sprites[3] = Managers.Instance.ResourceManager.Load<Sprite>(_towerData[3].TowerImgName);

            _selectTower.sprite = _sprites[0];

            return true;
        }
        private void Awake()
        {
            Init();
        }

        public Vector2Int _sv { get; private set; }
        private TowerData[] _towerData=new TowerData[4];
        private TowerManager _tm;
        private Facing _facing;
        private SpriteRenderer _selectTower;
        private RectTransform _uiSelectTower;
        private GridController _gc;
        private Grid _g;
        private RangePreview _rp;
        private GridInputHandler _gh;
        private bool _isPointerDown;
        private float _pressTime, _dragThresholdTime = 0.3f;
        private Vector3Int _wp;
        public void CheckButton(Vector3 _)
        {
            bool b = false;
            for (int i = 0; i < 3; i++)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(GetButton((int)Buttons.Slot_0 + i).GetComponent<RectTransform>(), _))
                {
                    b = true;
                }
            }
            if(b==false)
            {
                ShopOpen = false;
                CloseSR();
                _rp.Hide();
                gameObject.SetActive(false);
                _selectTower.DOKill();
                _selectTower.color = Color.white;
            }
        }    
        protected void ChangeFacing(PointerEventData _)
        {
            Managers.Instance.SoundManager.Play(SoundChannels.EFFECT_0, "ButtonPress", false);
            _facing = (Facing)(((int)_facing + 90) % 360);
            _selectTower.transform.parent.DOKill();
            _selectTower.transform.parent.DORotate(Vector3.forward * 90f, 0.1f, RotateMode.LocalAxisAdd).SetEase(Ease.OutQuad);
            if(Managers.Instance.DataManager.SkillDic.TryGetValue(_towerData[(int)_s].SkillId, out SkillData sd)==true)
            {
                _rp.ShowAttackRange(sd.AttackPos, _sv, _facing);
            }
        }

        protected void Buy(PointerEventData _)
        {
            Managers.Instance.SoundManager.Play(SoundChannels.EFFECT_0, "ButtonPress", false);
            _tm.PlaceTower(_towerData[(int)_s], _sv, _facing);
            ShopOpen = false;
            CloseSR();
            _rp.Hide();
        }

        protected void Cancel(PointerEventData _)
        {
            Managers.Instance.SoundManager.Play(SoundChannels.EFFECT_0, "ButtonPress", false);
            ShopOpen = false;
         
            CloseSR();
            _rp.Hide();
            SelectedSlot();
        }

        private void Arrows()
        {

        }
        public void Set(Vector2Int p,Vector3 _)
        {
            _sv = p;
            _selectTower.transform.parent.gameObject.SetActive(true);
            _gc.PlacePieceAt(new Vector3Int(p.x, p.y, 0), _selectTower.transform.parent);
            _selectTower.transform.parent.position += new Vector3(0.5f, 0.5f, 0);
            float posY = 0.8f;
            if(_.y>Screen.height * 0.5f)
            {
                posY = -0.8f;
            }
            _uiSelectTower.position = _selectTower.transform.parent.position + new Vector3(0, -0.25f+posY, 0);
            if (ShopOpen == false)
            {
                _selectTower.DOKill();
                _selectTower.color = Color.white;
                _selectTower.transform.parent.rotation = Quaternion.identity;
                _selectTower.DOFade(0.3f, 0.15f).SetLoops(-1, LoopType.Yoyo);
                SelectedSlot();
                _uiSelectTower.gameObject.SetActive(true);
                if (_s == Slots.None)
                {
                    return;
                }
                _rp.ShowAttackRange(Managers.Instance.DataManager.SkillDic[_towerData[(int)_s].SkillId].AttackPos, _sv, _facing);
            }
            else
            {
                if(_s==Slots.None)
                {
                    return;
                }
                _uiSelectTower.gameObject.SetActive(true);
                _rp.ShowAttackRange(Managers.Instance.DataManager.SkillDic[_towerData[(int)_s].SkillId].AttackPos, _sv, _facing);
            }
        }
        public void CloseSR()
        {
            _selectTower.transform.parent.DOKill();
            _selectTower.transform.parent.gameObject.SetActive(false);
            _uiSelectTower.gameObject.SetActive(false);
        }
        enum Slots 
        {
            None=-675,
            Slot0=0,
            Slot1=1,
            Slot2=2,
            Slot3=3
            //그 뭐냐 벽? 그거 추가예정
        }
        private Slots _s=Slots.Slot0;
        private Sprite[] _sprites = new Sprite[4];
        private bool _drag=false;
        protected void SlotDown(int index,PointerEventData _)
        {
            Managers.Instance.SoundManager.Play(SoundChannels.EFFECT_0, "ButtonPress", false);
            _isPointerDown = true;
            Managers.Instance.GameManager.IsDragging = true;
            _pressTime = Time.unscaledTime;
            _rp.Hide();
            _selectTower.sprite = _sprites[index];
            _uiSelectTower.gameObject.SetActive(false);
            //_rp.ShowAttackRange(Managers.Instance.DataManager.SkillDic[_towerData[index].SkillId].AttackPos, _sv, _facing);
            _s =(Slots)index;
            SelectedSlot();
        }

        protected void SlotDragStart()
        {
            _drag = true;
            //드래그로 이동하는건 아직 미구현
#if UNITY_EDITOR
            Debug.Log("드래그 시작임 암튼 그럼");
#endif
            _selectTower.transform.parent.gameObject.SetActive(true);
        }

        private void TowerDrag()
        {

            Vector3Int cellPos = _g.WorldToCell(Camera.main.ScreenToWorldPoint(_gh.GetCurrentVector3()));
            Vector3 worldPos = _g.GetCellCenterWorld(cellPos);
            Vector3Int wp = Vector3Int.CeilToInt(worldPos);
            if (wp == _wp)
            {
                return;
            }
            _wp = wp;
            _sv = (Vector2Int)cellPos;
#if UNITY_EDITOR
            Debug.Log("드래그중임 암튼 그럼");
#endif
            _selectTower.transform.parent.position= wp-new Vector3(0.5f, 0.5f, 0);
            _gc.SetHighlightAt(cellPos);
            _rp.ShowAttackRange(Managers.Instance.DataManager.SkillDic[_towerData[(int)_s].SkillId].AttackPos, _sv, _facing);
        }

        private void HandleClick()
        {
            _uiSelectTower.gameObject.SetActive(true);
            _rp.ShowAttackRange(Managers.Instance.DataManager.SkillDic[_towerData[(int)_s].SkillId].AttackPos, _sv, _facing);
        }
        private void SlotUp(PointerEventData _)
        {
            Managers.Instance.GameManager.IsDragging = false;
            if (_drag == false)
            {
                HandleClick();
            }
            else
            {
                _drag = false;
#if UNITY_EDITOR
                Debug.Log("드래그 끝임 암튼 그럼");
#endif
                float posY = 0.8f;
                if (_wp.y > Screen.height * 0.5f)
                {
                    posY = -0.8f;
                }
                _uiSelectTower.position = _selectTower.transform.parent.position + new Vector3(0, -0.25f + posY, 0);
                Buy(default(PointerEventData));
            }
            _isPointerDown = false;
        }
        private void FixedUpdate()
        {

            if(_drag==true)
            {
                TowerDrag();
            }
        }
        private void Update()
        {
            if (_isPointerDown==true && _drag ==false)
            {
                if (Time.unscaledTime - _pressTime >= _dragThresholdTime)
                {
                    SlotDragStart();
                }
            }
        }
        private Image[] _images=new Image[4];
        private void SelectedSlot()
        {
            for(int i=0;i<3;i++)
            {
                if(i==(int)_s)
                {
                    _images[i].color = Color.gray;
                }
                else
                {
                    _images[i].color = Color.white;
                }
            }
            
        }

    }
}