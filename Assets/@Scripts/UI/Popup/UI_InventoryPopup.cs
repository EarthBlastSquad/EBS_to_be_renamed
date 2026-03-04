using Manager;
using ObjectPool;
using System;
using TMPro;
using UI.Popup.Cell;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;
using Utils.Defines;

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
                Image img = b.transform.GetChild(0).GetComponent<Image>();
                b.gameObject.BindUIEvent((_)=>SelectSlot((sbyte)j, img, _));
                b.GetComponent<SlotCell>().Init();

                if(t.key == -1)
                {
                    img.sprite = Managers.Instance.ResourceManager.Load<Sprite>("null_sprite");
                }
                else
                {
                    img.sprite = Managers.Instance.ResourceManager.Load<Sprite>(t.TowerData.TowerImgName);
                }
                
            }
        }
        #endregion
        #region UI전용 함수들
        private UI_ItemInfPool _infPool;
        public Image SelectedSlot { get; private set; } //string으로는 옅은 복사가 안되는 것 같음

        public void SlotChange(sbyte index,string s)
        {
            GetButton(index).transform.GetChild(0).GetComponent<Image>().sprite = Managers.Instance.ResourceManager.Load<Sprite>(s);
        }
        public sbyte SelectedSlotIndex { get; private set; }
        protected void SelectSlot(sbyte index, Image t, PointerEventData _)
        {
            Managers.Instance.SoundManager.Play(SoundChannels.EFFECT_0, "ButtonPress", false);

            if (SelectedSlot != null)
            {
                SelectedSlot.transform.parent.GetComponent<Image>().sprite = Managers.Instance.ResourceManager.Load<Sprite>("stone_backplate_short");
            }

            SelectedSlot = t;
            SelectedSlotIndex = index;
            SelectedSlot.sprite = Managers.Instance.ResourceManager.Load<Sprite>("null_sprite");
            SelectedSlot.transform.parent.GetComponent<Image>().sprite = Managers.Instance.ResourceManager.Load<Sprite>("stone_button_short_off");
        }

        protected void SlideInventory(UnityEngine.UI.Slider slider,PointerEventData _)
        {
            _infPool.Slide((int)slider.value);
        }

        #endregion
        public void OpenTDP(Contents.Tower.Tower t)
        {
            Managers.Instance.SoundManager.Play(SoundChannels.EFFECT_0, "ButtonPress", false);
            GetObject((int)GameObjects.TowerDataPopUp_0).SetActive(true);
            GetObject((int)GameObjects.Background_0).SetActive(true);
            GetObject((int)GameObjects.TowerDataPopUp_0).GetComponent<UI_TowerDataPopup>().OpenTowerData(t);
        }
        public void CloseTDP()
        {
            Managers.Instance.SoundManager.Play(SoundChannels.EFFECT_0, "ButtonPress", false);
            GetObject((int)GameObjects.TowerDataPopUp_0).SetActive(false);
            GetObject((int)GameObjects.Background_0).SetActive(false);
        }
    }
}

