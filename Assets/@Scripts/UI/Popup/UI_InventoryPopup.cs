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
        private UI_ItemInfPool _infPool;
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
            _infPool.Slide((int)slider.value);
        }

        #endregion
    }
}

