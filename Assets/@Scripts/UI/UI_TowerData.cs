using Manager;
using ObjectPool;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;

namespace UI
{
    public class UI_TowerData : UIBase
    {
        private bool _isData = true;
        private RangePreview _rangePreview;
        private SpriteRenderer _spriteRenderer;
        enum Images
        {
            TowerImage_0,
            RangePreview_0
        }
        enum Buttons
        {
            Change_0,
            StateChange_0
        }
        enum Texts
        {
            TowerDescription_0,
            TowerName_0,
            TowerAtk_0,
            TowerHP_0,
            TowerBCK_0
        }

        private void Awake()
        {
            Init();
        }
        public override bool Init()
        {
            if(base.Init()==false)
                return false;
            BindImage(typeof(Images));
            BindButton(typeof(Buttons));
            BindText(typeof(Texts));

            _uI_IP=transform.parent.GetComponent<UI.Popup.UI_InventoryPopup>();
            _rangePreview = FindAnyObjectByType<RangePreview>();
            _spriteRenderer=_rangePreview.GetComponentInChildren<SpriteRenderer>();
            if (_tower is not null)
            {
                GetText((int)Texts.TowerName_0).text = _tower.TowerData.TowerName;
                GetText((int)Texts.TowerAtk_0).text = Managers.Instance.DataManager.SkillDic[_tower.TowerData.SkillId].Damage.ToString();
                GetText((int)Texts.TowerDescription_0).text = _tower.TowerData.TowerDescription;
            }
            //GetButton((int)Buttons.Cancel_0).gameObject.BindUIEvent(Cancel);
            GetButton((int)Buttons.Change_0).gameObject.BindUIEvent(Change);
            GetButton((int)Buttons.StateChange_0).gameObject.BindUIEvent(StateChange);
            GetImage((int)Images.RangePreview_0).gameObject.SetActive(false);
            _spriteRenderer.enabled = false;
            return true;
        }

        public void OpenTowerData(Contents.Tower.Tower t)
        {
            _tower = t;
            GetText((int)Texts.TowerName_0).text = _tower.TowerData.TowerName;
            Sprite sprite= Managers.Instance.ResourceManager.Load<Sprite>(_tower.TowerData.TowerImgName);
            GetImage((int)Images.TowerImage_0).sprite = sprite;
            _spriteRenderer.sprite= sprite;
            if (_isData == false)
            {
                _rangePreview.ShowAttackRange(Managers.Instance.DataManager.SkillDic[_tower.TowerData.SkillId].AttackPos, new Vector2Int(6, 3), Utils.Defines.Facing.RIGHT);
            }
            else
            {
                _rangePreview.Hide();
            }
            GetText((int)Texts.TowerAtk_0).text = $"공격력 : {Managers.Instance.DataManager.SkillDic[_tower.TowerData.SkillId].Damage}";
            GetText((int)Texts.TowerDescription_0).text=_tower.TowerData.TowerDescription;
            GetText((int)Texts.TowerHP_0).text = $"체력 : {_tower.TowerData.TowerHP}";
            GetText((int)Texts.TowerBCK_0).text = $"가격 : {_tower.TowerData.DemendedCurrency}";
        }


        #region UI전용 함수들
        private Contents.Tower.Tower _tower;
        private UI.Popup.UI_InventoryPopup _uI_IP;
        //protected void Cancel(PointerEventData _)
        //{
        //    _uI_IP.CloseTDP();
        //}
        protected void Change(PointerEventData _)
        {
            if(Managers.Instance.GameManager.EquipTower(_uI_IP.SelectedSlotIndex, _tower)==false)
            {
                _uI_IP.CancelToast();
                return;
            }
            if(_uI_IP.SelectedSlot != null)
            {
                _uI_IP.SelectedSlot.sprite = Managers.Instance.ResourceManager.Load<Sprite>(_tower.TowerData.TowerImgName);
            }
            
            //_uI_IP.CloseTDP();
        }
        protected void StateChange(PointerEventData _)
        {
            _isData = !_isData;

            //GetText((int)Texts.TowerName_0).gameObject.SetActive(_isData);
            //GetImage((int)Images.TowerImage_0).gameObject.SetActive(_isData);
            //GetText((int)Texts.TowerAtk_0).gameObject.SetActive(_isData);
            GetText((int)Texts.TowerDescription_0).gameObject.SetActive(_isData);
            //GetText((int)Texts.TowerHP_0).gameObject.SetActive(_isData);
            //GetText((int)Texts.TowerBCK_0).gameObject.SetActive(_isData);
            if (_isData==false)
            {
                _rangePreview.ShowAttackRange(Managers.Instance.DataManager.SkillDic[_tower.TowerData.SkillId].AttackPos, new Vector2Int(6,3), Utils.Defines.Facing.RIGHT);
            }
            else
            {
                _rangePreview.Hide();
            }
            GetImage((int)Images.RangePreview_0).gameObject.SetActive(!_isData);
            _spriteRenderer.enabled = !_isData;
        }
        #endregion
    }
}

