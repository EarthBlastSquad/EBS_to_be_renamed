using Controller;
using Data;
using DG.Tweening;
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
            Slot_0, 
            Slot_1, 
            Slot_2
        }
        public override bool Init()
        {
            if (base.Init() == false)
                return false;
            BindButton(typeof(Buttons));
            for (int i = 0; i < 3; i++)
            {
                _towerData[i] = Managers.Instance.GameManager.EquippedTowers[i].TowerData;
                GetButton((int)Buttons.Slot_0+i).gameObject.GetComponent<Image>().sprite=Managers.Instance.ResourceManager.Load<Sprite>(_towerData[i].TowerImgName);
                GetButton((int)Buttons.Slot_0 + i).gameObject.GetComponentInChildren<TextMeshProUGUI>().text = $"{_towerData[i].DemendedCurrency} BCK";
                _images[i]= GetButton((int)Buttons.Slot_0 + i).GetComponent<Image>();
            }

            GetButton((int)Buttons.Slot_0).gameObject.BindUIEvent(Slot0);
            GetButton((int)Buttons.Slot_1).gameObject.BindUIEvent(Slot1);
            GetButton((int)Buttons.Slot_2).gameObject.BindUIEvent(Slot2);
            _tm =FindAnyObjectByType<TowerManager>();
            _selectTower = GameObject.Find("SelectTower").transform.GetComponentInChildren<SpriteRenderer>();
            _uiSelectTower = GameObject.Find("UI_SelectTower").GetComponent<RectTransform>();

            _uiSelectTower.gameObject.GetChild<Button>("Buy_0").gameObject.BindUIEvent(Buy);
            _uiSelectTower.gameObject.GetChild<Button>("Rotate_0").gameObject.BindUIEvent(ChangeFacing);
            _uiSelectTower.gameObject.GetChild<Button>("Cancel_0").gameObject.BindUIEvent(Cancel);

            g = FindAnyObjectByType<GridController>();
            _rp = FindAnyObjectByType<RangePreview>();
            return true;
        }
        private void Awake()
        {
            Init();
        }

        public Vector2Int _sv { get; private set; }
        private TowerData[] _towerData=new TowerData[3];
        private TowerManager _tm;
        private Facing _facing;
        private SpriteRenderer _selectTower;
        private RectTransform _uiSelectTower;
        private GridController g;
        private RangePreview _rp;

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
            }
        }    
        protected void ChangeFacing(PointerEventData _)
        {
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
            _tm.PlaceTower(_towerData[(int)_s], _sv, _facing);
            ShopOpen = false;
            CloseSR();
            _rp.Hide();
        }

        protected void Cancel(PointerEventData _)
        {
            ShopOpen = false;
            _s = Slots.None;
            CloseSR();
            _rp.Hide();
            SelectedSlot();
        }

        private void Arrows()
        {

        }
        public void Set(Vector2Int p)
        {
            _sv = p;
            _selectTower.transform.parent.gameObject.SetActive(true);
            g.PlacePieceAt(new Vector3Int(p.x, p.y, 0), _selectTower.transform.parent);
            _selectTower.transform.parent.position += new Vector3(0.5f, 0.5f, 0);
            _uiSelectTower.position = _selectTower.transform.parent.position + new Vector3(0, -0.25f, 0);
            if (ShopOpen == false)
            {
                _selectTower.transform.parent.rotation = Quaternion.identity;
                _selectTower.DOFade(0.3f, 0.15f).SetLoops(-1, LoopType.Yoyo);
                SelectedSlot();
                _uiSelectTower.gameObject.SetActive(false);
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
            //그 뭐냐 벽? 그거 추가예정
        }
        private Slots _s=Slots.None;
        protected void Slot0(PointerEventData _)
        {
            if (_s==Slots.Slot0)
            {
                return;
            }
            _rp.Hide();
            _selectTower.sprite = Managers.Instance.ResourceManager.Load<Sprite>(_towerData[0].TowerImgName);
            _uiSelectTower.gameObject.SetActive(true);
            _rp.ShowAttackRange(Managers.Instance.DataManager.SkillDic[_towerData[0].SkillId].AttackPos, _sv, _facing);
            _s =Slots.Slot0;
            SelectedSlot();
        }
        protected void Slot1(PointerEventData _)
        {
            if (_s==Slots.Slot1)
            {
                return;
            }
            _rp.Hide();
            _selectTower.sprite = Managers.Instance.ResourceManager.Load<Sprite>(_towerData[1].TowerImgName);
            _uiSelectTower.gameObject.SetActive(true);
            _rp.ShowAttackRange(Managers.Instance.DataManager.SkillDic[_towerData[1].SkillId].AttackPos, _sv, _facing);
            _s = Slots.Slot1;
            SelectedSlot();
        }
        protected void Slot2(PointerEventData _)
        {
            if (_s==Slots.Slot2)
            {
                return;
            }
            _rp.Hide();
            _selectTower.sprite = Managers.Instance.ResourceManager.Load<Sprite>(_towerData[2].TowerImgName);
            _uiSelectTower.gameObject.SetActive(true);
            _rp.ShowAttackRange(Managers.Instance.DataManager.SkillDic[_towerData[2].SkillId].AttackPos, _sv, _facing);
            _s = Slots.Slot2;
            SelectedSlot();
        }
        private Image[] _images=new Image[3];
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