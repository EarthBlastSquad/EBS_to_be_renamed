using Manager;
using Manager.Contents;
using UI.Scene;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;
using Utils.Defines;

namespace UI.Popup
{
    public class UI_TowerDatasMini : UIPopup
    {
        private TowerManager _tm;
        private bool _isFieldTower;
        private UI_GameScene _uiGameScene;
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
            _uiGameScene=FindAnyObjectByType<UI_GameScene>();
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
            _isFieldTower = true;
        }
        public void DontSellSet(string name, int damage, float cool)
        {
            gameObject.SetActive(true);
            GetText((int)Texts.TowerName_0).text = name;
            GetText((int)Texts.TowerAtk_0).text = $"ATK:{damage}";
            GetText((int)Texts.TowerSpeed_0).text = $"AttackSpeed:{cool}";
            _isFieldTower = false;
        }
        protected void Sell(PointerEventData _)
        {
            Managers.Instance.SoundManager.Play(SoundChannels.EFFECT_0, "ButtonPress", false);
            if(_isFieldTower==false)
            {
                _uiGameScene.ShopOpen();
                gameObject.SetActive(false);
                return;
            }
            _tm.RetrieveTower(_v);
            gameObject.SetActive(false);
        }
    }
}

