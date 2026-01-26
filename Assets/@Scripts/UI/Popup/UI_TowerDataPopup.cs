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
    public class UI_TowerDataPopup : UIPopup
    {
        enum Images
        {
            TowerImage_0
        }
        enum Buttons
        {
            Cancel_0,
            Change_0
        }
        enum Texts
        {
            TowerDescription_0,
            TowerName_0,
            TowerAtk_0,
            TowerRange_0,
        }

        public override bool Init()
        {
            if(base.Init()==false)
                return false;
            BindImage(typeof(Images));
            BindButton(typeof(Buttons));
            BindText(typeof(Texts));
            GetButton((int)Buttons.Cancel_0).gameObject.BindUIEvent(Cancel);
            GetButton((int)Buttons.Change_0).gameObject.BindUIEvent(Change);
            _uI_IP=transform.parent.GetComponent<UI_InventoryPopup>();
            if(_tower is not null)
            {
                GetText((int)Texts.TowerName_0).text = _tower.TowerData.TowerName;
                GetText((int)Texts.TowerAtk_0).text = Managers.Instance.DataManager.SkillDic[_tower.TowerData.SkillId].Damage.ToString();
                GetText((int)Texts.TowerDescription_0).text = _tower.TowerData.TowerDescription;
            }
            return true;
        }

        public void OpenTowerData(Contents.Tower.Tower t)
        {
            _tower = t;
            GetText((int)Texts.TowerName_0).text = t.TowerData.TowerName;
            GetImage((int)Images.TowerImage_0).sprite = Managers.Instance.ResourceManager.Load<Sprite>(t.TowerData.TowerImgName);
            GetText((int)Texts.TowerAtk_0).text = $"공격력 : {Managers.Instance.DataManager.SkillDic[_tower.TowerData.SkillId].Damage}";
            GetText((int)Texts.TowerDescription_0).text=t.TowerData.TowerDescription;
        }


        #region UI전용 함수들
        private Contents.Tower.Tower _tower;
        private UI_InventoryPopup _uI_IP;
        protected void Cancel(PointerEventData _)
        {
            _uI_IP.CloseTDP();
        }
        protected void Change(PointerEventData _)
        {
            Managers.Instance.GameManager.EquipTower(_uI_IP.SelectedSlotIndex, _tower);
            _uI_IP.SelectedSlot.text = _tower.TowerData.TowerName;
            _uI_IP.CloseTDP();
        }
        #endregion
    }
}

