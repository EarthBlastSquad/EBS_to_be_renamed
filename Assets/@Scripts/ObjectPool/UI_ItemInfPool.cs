using Contents.Tower;
using Manager;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UI;
using UI.Popup;
using UI.Popup.Cell;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Utils;
using Utils.Defines;

namespace ObjectPool
{
    public class UI_ItemInfPool : UIPopup
    {
        private LinkedList<UnityEngine.UI.Button> Items = new LinkedList<UnityEngine.UI.Button>();
        private int _topIndex = 0;

        enum GameObjects
        {

        }
        enum Buttons
        {
            Inventory_0_0,
            Inventory_0_1,
            Inventory_0_2,
            Inventory_0_3,
            Inventory_0_4,
            Inventory_0_5,
            Inventory_0_6,
            Inventory_0_7,
            Inventory_0_8,
            Inventory_0_9,
            Inventory_0_10,
            Inventory_0_11,
            Inventory_0_12,
            Inventory_0_13,
            Inventory_0_14,
            Inventory_0_15,
            Inventory_0_16,
            Inventory_0_17,
            Inventory_0_18,
            Inventory_0_19,
        }
        enum Texts
        {

        }
        public override bool Init()
        {
            if (base.Init() == false)
                return false;
            BindObject(typeof(GameObjects));
            BindButton(typeof(Buttons));
            BindText(typeof(Texts));

            _rectTransform= GetComponent<RectTransform>();
            _uI_IP = transform.parent.GetComponent<UI_InventoryPopup>();
            _selectedItemSprite = transform.parent.gameObject.GetChild<Image>("SelectedItem_0");
            _slots[0] = _uI_IP.gameObject.GetChild<SlotCell>("Slot_0").GetComponent<RectTransform>();
            _slots[1] = _uI_IP.gameObject.GetChild<SlotCell>("Slot_1").GetComponent<RectTransform>();
            _slots[2] = _uI_IP.gameObject.GetChild<SlotCell>("Slot_2").GetComponent<RectTransform>();
            Itemset();
            return true;
        }

        private void Itemset()
        {
            int end = 20;
            int c = Manager.Managers.Instance.GameManager.OwnedTowers.Count();
            if (c<20)
            {
                end = c;
                transform.parent.GetComponentInChildren<UnityEngine.UI.Slider>().maxValue = end / 9;
            }

            for (int i = 0; i < end; i++) //3으로 써놨지만 ui의 인피니티 풀 개수로 들어갈 예정
            {
                //getbutton(i).image=; 샘플이 없네... 몰라 일단 텍스트
                var td = Managers.Instance.GameManager.OwnedTowers[i];
                UnityEngine.UI.Button b = GetButton(i + (int)Buttons.Inventory_0_0);
                ItemCell ic = b.transform.GetComponent<ItemCell>();
                ic.TowerSet(td);
                b.gameObject.BindUIEvent((_) => SelectItemDown(ic.ICTower, _), UIEventTypes.POINTER_DOWN);
                b.gameObject.BindUIEvent(SelectItemUp,UIEventTypes.POINTER_UP);
                b.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = td.TowerData.TowerName;
                //GetButton(i + (int)Buttons.Item_0).tag = $"{i}";

                Items.AddLast(b);
            }
            for(int j=end;j<20;j++)
            {
                GetButton(j + (int)Buttons.Inventory_0_0).gameObject.SetActive(false);
            }
        }
        private UI_InventoryPopup _uI_IP;
        private bool _isPointerDown=false,_isDragging = false;
        private float _pressTime, _dragThresholdTime = 0.3f;
        private UnityEngine.UI.Image _selectedItemSprite;
        private Contents.Tower.Tower _selectedTower;
        private RectTransform[] _slots=new RectTransform[3];
        protected void SelectItemDown(Contents.Tower.Tower t,PointerEventData _)
        {
            _isPointerDown = true;
            _isDragging = false;
            _selectedTower = t;
            _pressTime = Time.unscaledTime;
        }
        protected void SelectItemUp(PointerEventData _)
        {
            _isPointerDown = false;
            if (_isDragging==false)
            {
                _uI_IP.OpenTDP(_selectedTower);
            }
            else
            {
                _isDragging = false;
                _selectedItemSprite.enabled = false;
                Vector2 v=Pointer.current.position.ReadValue();
                sbyte sb=-1;
                if (RectTransformUtility.RectangleContainsScreenPoint(_slots[0], v))
                {
                    sb = 0;
                }
                else if(RectTransformUtility.RectangleContainsScreenPoint(_slots[1], v))
                {
                    sb = 1;
                }
                else if(RectTransformUtility.RectangleContainsScreenPoint(_slots[2], v))
                {
                    sb = 2;
                }
                if(sb==-1)
                {
                    return;
                }
                Managers.Instance.GameManager.EquipTower(sb, _selectedTower);
                _uI_IP.SlotChange(sb, _selectedTower.TowerData.TowerName);
            }
            //Managers.Instance.GameManager.EquipTower(_uI_IP.SelectedSlotIndex, t);
            //_uI_IP.SelectedSlot.text = t.TowerData.TowerName;
        }
        private void ItemDragging()
        {
            _selectedItemSprite.transform.position= Pointer.current.position.ReadValue();
        }
        private void FixedUpdate()
        {

            if (_isDragging==true)
            {
                ItemDragging();
            }
        }
        private void Update()
        {
            if (_isPointerDown==true&_isDragging == false)
            {
                if (Time.unscaledTime - _pressTime >= _dragThresholdTime)
                {
                    _isDragging = true;
                    _selectedItemSprite.enabled = true;
                    _selectedItemSprite.sprite = Managers.Instance.ResourceManager.Load<Sprite>(_selectedTower.TowerData.TowerImgName);
                }
            }
        }
        private RectTransform _rectTransform;
        public void Slide(int sliderValue)
        {
            int newIndex = sliderValue;
            int delta = newIndex - _topIndex;
            if (delta == 0)
            {
                return;
            }
            _topIndex = newIndex;
            _rectTransform.anchoredPosition += new Vector2(0, 150 * delta);

            if (delta > 0)
            {
                int moveCount = 5 * delta;

                for (int n = 0; n < moveCount; n++)
                {
                    UnityEngine.UI.Button btn = Items.First.Value;
                    var td = Managers.Instance.GameManager.OwnedTowers[(_topIndex * 5) + 15 + n];

                    btn.GetComponent<ItemCell>().TowerSet(td);
                    btn.GetComponentInChildren<TextMeshProUGUI>().text = td.TowerData.TowerName;
                    RectTransform rt = btn.GetComponent<RectTransform>();
                    rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, -450 - _rectTransform.anchoredPosition.y);
                    Items.RemoveFirst();
                    Items.AddLast(btn);
                }
            }
            else
            {
                int moveCount = 5 * (-delta);
                int c = 0;
                for (int n = moveCount - 1; n >= c; n--)
                {
                    UnityEngine.UI.Button btn = Items.Last.Value;
                    var td = Managers.Instance.GameManager.OwnedTowers[(_topIndex * 5) + 15 + n];
                    btn.GetComponent<ItemCell>().TowerSet(td);
                    btn.GetComponentInChildren<TextMeshProUGUI>().text = td.TowerData.TowerName;
                    RectTransform rt = btn.GetComponent<RectTransform>();
                    rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, -_rectTransform.anchoredPosition.y);
                    Items.RemoveLast();
                    Items.AddFirst(btn);
                }
            }

        }
        
    }
}

