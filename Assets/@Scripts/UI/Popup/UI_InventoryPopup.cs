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
            Inventorys_0,
            TowerDataPopUp_0,
            Background_0
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
            int i = 0;
            foreach (Contents.Tower.Tower t in Manager.Managers.Instance.GameManager.EquippedTowers)
            {
                UnityEngine.UI.Button b = GetButton(i++ + (int)Buttons.Slot_0);
                b.gameObject.BindUIEvent(SelectSlot);
                b.GetComponent<SlotCell>().Init();

                string s;
                if(t.TowerData==null)
                {
                    s = "";
                }
                else
                {
                    s = t.TowerData.TowerName;
                }
                b.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{s}";
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
        public void OpenTDP(Contents.Tower.Tower t)
        {
            GetObject((int)GameObjects.TowerDataPopUp_0).SetActive(true);
            GetObject((int)GameObjects.Background_0).SetActive(true);
            GetObject((int)GameObjects.TowerDataPopUp_0).GetComponent<UI_TowerDataPopup>().OpenTowerData(t);
        }
        public void CloseTDP()
        {
            GetObject((int)GameObjects.TowerDataPopUp_0).SetActive(false);
            GetObject((int)GameObjects.Background_0).SetActive(false);
        }
    }
}

