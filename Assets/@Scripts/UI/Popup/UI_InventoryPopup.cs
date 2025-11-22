using Manager;
using ObjectPool;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UI.Popup.Cell;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;

namespace UI.Popup
{
    public class UI_InventoryPopup : UIPopup
    {
        enum GameObjects
        {
            Slider_0,
            Inventorys_0
        }
        enum Buttons
        {
            Slot_0,
            Slot_1, 
            Slot_2,
        }
        enum Texts
        {

        }

        public override bool Init()
        {
            if(base.Init()==false)
                return false;
            BindObject(typeof(GameObjects));
            BindButton(typeof(Buttons));
            BindText(typeof(Texts));
            Slotset();
            GetObject((int)GameObjects.Slider_0).BindUIEvent(SlideInventory, Utils.Defines.UIEventTypes.DRAG);
            _infPool=GetObject((int)GameObjects.Inventorys_0).GetComponent<UI_ItemInfPool>();
            _rectTransform=GetObject((int)GameObjects.Inventorys_0).GetComponent<RectTransform>();
            return true;
        }
        #region 버튼 세팅

        private void Slotset()
        {
            for (int i = 0; i < 3; i++)
            {
                UnityEngine.UI.Button b = GetButton(i + (int)Buttons.Slot_0);
                b.gameObject.BindUIEvent(SelectSlot);
                b.GetComponent<SlotCell>().Init();
                if (Manager.Managers.Instance.GameManager.EquippedTowers[i] is default(Contents.Tower.Tower))
                {
                    continue;
                }
                b.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{Manager.Managers.Instance.GameManager.EquippedTowers[i].TowerData.TowerName}";
               
            }
        }
        #endregion
        #region UI전용 함수들


        private int _topIndex;

        private UI_ItemInfPool _infPool;
        private RectTransform _rectTransform;
        public TextMeshProUGUI SelectedSlot { get; private set; } //string으로는 옅은 복사가 안되는 것 같음
        public sbyte SelectedSlotIndex { get; private set; }
        protected void SelectSlot(PointerEventData _)
        {
            SelectedSlot = _.pointerPress.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            SelectedSlotIndex = _.pointerPress.GetComponent<SlotCell>().SlotIndex;
            SelectedSlot.text = "";
        }

        protected void SlideInventory(PointerEventData _)
        {
            UnityEngine.UI.Slider slider = _.pointerPress.GetComponent<UnityEngine.UI.Slider>();
            int newIndex = (int)slider.value;
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
                    UnityEngine.UI.Button btn = _infPool.Items.First.Value;
                    btn.GetComponent<ItemCell>().TowerSet(Manager.Managers.Instance.GameManager.OwnedTowers[(_topIndex * 5)+15 + n]);
                    btn.GetComponentInChildren<TextMeshProUGUI>().text = Manager.Managers.Instance.GameManager.OwnedTowers[(_topIndex * 5) + 15 + n].TowerData.TowerName;
                    RectTransform rt = btn.GetComponent<RectTransform>();
                    rt.anchoredPosition = new Vector2(rt.anchoredPosition.x,-450 - _rectTransform.anchoredPosition.y);
                    _infPool.Items.RemoveFirst();
                    _infPool.Items.AddLast(btn);
                }
            }
            else
            {
                int moveCount = 5 * (-delta);
                for (int n = moveCount-1; n >=0 ; n--)
                {
                    UnityEngine.UI.Button btn = _infPool.Items.Last.Value;
                    btn.GetComponent<ItemCell>().TowerSet(Manager.Managers.Instance.GameManager.OwnedTowers[(_topIndex * 5)+ n]);
                    btn.GetComponentInChildren<TextMeshProUGUI>().text = Manager.Managers.Instance.GameManager.OwnedTowers[(_topIndex * 5) + n].TowerData.TowerName;
                    RectTransform rt = btn.GetComponent<RectTransform>();
                    rt.anchoredPosition = new Vector2(rt.anchoredPosition.x,-_rectTransform.anchoredPosition.y);
                    _infPool.Items.RemoveLast();
                    _infPool.Items.AddFirst(btn);
                }
            }
        }

        #endregion
    }
}

