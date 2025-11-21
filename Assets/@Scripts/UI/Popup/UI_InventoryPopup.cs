using Manager;
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

        }
        enum Buttons
        {
            Slot_0,
            Slot_1, 
            Slot_2,
            Item_0,
            Item_1,
            Item_2,
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
            Itemset();
            return true;
        }
        #region 버튼 세팅
        private void Itemset()
        {
            for (int i = 0; i < 3; i++) //3으로 써놨지만 ui의 인피니티 풀 개수로 들어갈 예정
            {
                UnityEngine.UI.Button b = GetButton(i + (int)Buttons.Item_0);
                b.gameObject.BindUIEvent(SelectItem);
                //getbutton(i).image=; 샘플이 없네... 몰라 일단 텍스트
                b.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{Manager.Managers.Instance.GameManager.OwnedTowers[i].TowerData.TowerName}";
                //GetButton(i + (int)Buttons.Item_0).tag = $"{i}";
                b.transform.GetComponent<ItemCell>().TowerSet(Manager.Managers.Instance.GameManager.OwnedTowers[i]);
            }
        }
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
        #region 버튼전용 함수들
        private TextMeshProUGUI SelectedSlot; //string으로는 옅은 복사가 안되는 것 같음
        private sbyte SelectedSlotIndex;
        protected void SelectSlot(PointerEventData _)
        {
            SelectedSlot = _.pointerPress.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            SelectedSlotIndex=_.pointerPress.GetComponent<SlotCell>().SlotIndex;
            SelectedSlot.text = "";
        }
        protected void SelectItem(PointerEventData _)
        {
            Contents.Tower.Tower t = _.pointerPress.GetComponent<ItemCell>().ICTower;
            Managers.Instance.GameManager.EquipTower(SelectedSlotIndex, t);
            SelectedSlot.text = t.TowerData.TowerName;
        }
        #endregion
    }
}

