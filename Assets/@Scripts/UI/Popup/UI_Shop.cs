using Data;
using InputHandler;
using Manager;
using Manager.Contents;
using TMPro;
using UI.Scene;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;
using Utils.Defines;
namespace UI.Popup
{

    public class UI_Shop : UIPopup
    {
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
            }
            GetButton((int)Buttons.Slot_0).gameObject.BindUIEvent(Slot0);
            GetButton((int)Buttons.Slot_1).gameObject.BindUIEvent(Slot1);
            GetButton((int)Buttons.Slot_2).gameObject.BindUIEvent(Slot2);
            _tm =FindAnyObjectByType<TowerManager>();
            _gm= FindAnyObjectByType<GridManager>();

            return true;
        }
        private void Awake()
        {
            Init();
        }
        private void OnEnable()
        {
            _sv=_gm.GetLastSelectedPos();
        }
        private Vector2Int _sv;
        private TowerData[] _towerData=new TowerData[3];
        private TowerManager _tm;
        private GridManager _gm;

        protected void Slot0(PointerEventData _)
        {
            _tm.PlaceTower(_towerData[0], _sv, new Vector2Int(1, 0));
        }
        protected void Slot1(PointerEventData _)
        {
            _tm.PlaceTower(_towerData[1], _sv, new Vector2Int(1, 0));
        }
        protected void Slot2(PointerEventData _)
        {
            _tm.PlaceTower(_towerData[2], _sv, new Vector2Int(1, 0));
        }


    }
}