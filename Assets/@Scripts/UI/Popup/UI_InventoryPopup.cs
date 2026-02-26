using Manager;
using ObjectPool;
using TMPro;
using UI.Popup.Cell;
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
            Slot_0=0,
            Slot_1=1, 
            Slot_2=2,
        }
        enum Texts
        {

        }
        private void Awake()
        {
            Init();
        }
        public override bool Init()
        {
            if(base.Init()==false)
                return false;
            BindObject(typeof(GameObjects));
            BindButton(typeof(Buttons));
            BindText(typeof(Texts));
            Slotset();
            UnityEngine.UI.Slider s = GetObject((int)GameObjects.Slider_0).GetComponent<UnityEngine.UI.Slider>();
            GetObject((int)GameObjects.Slider_0).BindUIEvent((_)=>SlideInventory(s,_), Utils.Defines.UIEventTypes.DRAG);
            _infPool=GetObject((int)GameObjects.Inventorys_0).GetComponent<UI_ItemInfPool>();
            return true;
        }
        #region 버튼 세팅

        private void Slotset()
        {
            int i = 0;
            foreach (Contents.Tower.Tower t in Manager.Managers.Instance.GameManager.EquippedTowers)
            {
                int j = i++ + (int)Buttons.Slot_0;
                UnityEngine.UI.Button b = GetButton(j);
                TextMeshProUGUI tmpt = b.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                b.gameObject.BindUIEvent((_)=>SelectSlot((sbyte)j,tmpt,_));
                b.GetComponent<SlotCell>().Init();

                string s;
                if(t.key == -1)
                {
                    s = "";
                }
                else
                {
                    s = t.TowerData.TowerName;
                }
                tmpt.text = $"{s}";
            }
        }
        #endregion
        #region UI전용 함수들
        private UI_ItemInfPool _infPool;
        public TextMeshProUGUI SelectedSlot { get; private set; } //string으로는 옅은 복사가 안되는 것 같음

        public void SlotChange(sbyte index,string s)
        {
            GetButton(index).GetComponentInChildren<TextMeshProUGUI>().text = s;
        }
        public sbyte SelectedSlotIndex { get; private set; }
        protected void SelectSlot(sbyte index, TextMeshProUGUI t, PointerEventData _)
        {
            SelectedSlot = t;
            SelectedSlotIndex = index;
            SelectedSlot.text = "";
        }

        protected void SlideInventory(UnityEngine.UI.Slider slider,PointerEventData _)
        {
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

