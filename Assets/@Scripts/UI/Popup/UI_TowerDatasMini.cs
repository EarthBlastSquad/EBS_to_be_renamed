using Manager.Contents;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;

namespace UI.Popup
{
    public class UI_TowerDatasMini : UIPopup
    {
        private TowerManager _tm;
        enum Buttons
        {
            Sell_0
        }
        enum Texts
        {
            TowerName_0,
            TowerAtk_0,
            TowerSpeed_0
        }

        public override bool Init()
        {
            if (base.Init() == false)
                return false;
            BindButton(typeof(Buttons));
            BindText(typeof(Texts));
            GetButton((int)Buttons.Sell_0).gameObject.BindUIEvent(Sell);
            _tm=FindAnyObjectByType<TowerManager>();
            return true;
        }
        private Vector2Int _v;
        public void Set(string name, int damage, float cool,Vector2Int v)
        {
            gameObject.SetActive(true);
            GetText((int)Texts.TowerName_0).text = name;
            GetText((int)Texts.TowerAtk_0).text = $"ATK:{damage}";
            GetText((int)Texts.TowerSpeed_0).text = $"AttackSpeed:{cool}";
            _v = v;
        }

        protected void Sell(PointerEventData _)
        {
            _tm.RetrieveTower(_v);
            gameObject.SetActive(false);
        }
    }
}

