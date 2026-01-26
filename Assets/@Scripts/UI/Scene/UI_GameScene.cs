
using Coordinator;
using Data;
using InputHandler;
using Manager;
using Manager.Contents;
using Scenes;
using UI.Popup;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;
using static UnityEditor.PlayerSettings;

namespace UI.Scene
{
    public class UI_GameScene : UIScene
    {
        private GameScene _gs;
        private GridManager _gm;
        private TowerManager _tm;
        private UI_Shop _uis;
        private AreaUnlockManager _aum;
        #region Enum
        enum GameObjects
        {
            Panel_0,
            ESC_0,
            Shop_0
        }

        enum Buttons
        {
            Pause_0,
            Unlock_0
        }

        enum Texts
        {
            Waves_0,
            Timer_0,
            BCK_0
        }

        //enum Images
        //{
        //}
        #endregion

        public override bool Init()
        {
            if (base.Init() == false)
                return false;
            // 오브젝트 바인딩
            BindObject(typeof(GameObjects));
            BindButton(typeof(Buttons));
            BindText(typeof(Texts));
            //BindImage(typeof(Images));

            _gs = GameObject.Find("GameScene").GetComponent<GameScene>();
            _gs.OnWaveChanged -= WaveUI;
            _gs.OnWaveChanged += WaveUI;
            Managers.Instance.CurrencyManager.OnCurrencyChangedEvent -= CurrencyUI;
            Managers.Instance.CurrencyManager.OnCurrencyChangedEvent += CurrencyUI;
            GetButton((int)Buttons.Pause_0).gameObject.BindUIEvent(PauseButton);
            GetButton((int)Buttons.Unlock_0).gameObject.BindUIEvent(UnlickButton);

            GridInputHandler gh = FindAnyObjectByType<GridInputHandler>();
            gh.mouseUpSubscriberEvent -= ShopUI;
            gh.mouseUpSubscriberEvent += ShopUI;
            Managers.Instance.TimerManager.OnSecondChanged -= TimerUI;
            Managers.Instance.TimerManager.OnSecondChanged += TimerUI;

            GetObject((int)GameObjects.Panel_0).gameObject.SetActive(false);
            GetObject((int)GameObjects.ESC_0).gameObject.SetActive(false);
            GetObject((int)GameObjects.Shop_0).gameObject.SetActive(false);
            GetButton((int)Buttons.Unlock_0).gameObject.SetActive(true);
#if UNITY_EDITOR
            Managers.Instance.CurrencyManager.AddCurrency(0404);
            Debug.Log(GetButton((int)Buttons.Pause_0).gameObject.name);
#endif
            _gm = FindAnyObjectByType<GridManager>();
            _tm = FindAnyObjectByType<TowerManager>();
            _uis = GetObject((int)GameObjects.Shop_0).GetComponent<UI_Shop>();
            _aum = FindAnyObjectByType<AreaUnlockManager>();
            return true;
        }
        #region 팝업

        #endregion
        #region 바인드용
        protected void PauseButton(PointerEventData _)
        {
            Manager.Managers.Instance.GameManager.IsGamePaused = true;
            GetObject((int)GameObjects.Panel_0).gameObject.SetActive(true);
            GetObject((int)GameObjects.ESC_0).gameObject.SetActive(true);
            Time.timeScale = 0;

        }

        protected void UnlickButton(PointerEventData _)
        {
            if(_aum.TryUnlock())
            {
                GetButton((int)Buttons.Unlock_0).gameObject.SetActive(false);
            }
        }

        protected void WaveUI(WaveData wd)
        {
            GetText((int)Texts.Waves_0).text = $"{wd.WaveIdx}/10 Waves";
        }

        protected void CurrencyUI(int c, int cc)
        {
            GetText((int)Texts.BCK_0).text = $"{cc} BCK";
        }

        protected void ShopUI(Vector3 _)
        {
            Vector2Int p = _gm.GetLastSelectedPos();
            if (p == new Vector2(-1, -1))
            {
                _uis.CheckButton(_);
                return;
            }
            else if (_gm.TryGetPlacedPiece(p, out GameObject outTower) && outTower.TryGetComponent<TowerCoordinator>(out TowerCoordinator tc))
            {
                _tm.RetrieveTower(p);//삭제대신정보창
            }
            else if (_uis.ShopOpen == true&&_uis._sv==p)
            {
                _uis.ChangeFacing();
            }
            else
            {
                _uis.Set(p);
                GetObject((int)GameObjects.Shop_0).gameObject.SetActive(true);
                _uis.ShopOpen = true;
            }
        }

        #region 타이머
        private int _m=0, _s=0;

        protected void TimerUI(float totaltime, float time)
        {
            int i=(int)(time-totaltime);
            if(_s== i % 60)
            {
                return;
            }
            _m = i / 60;
            _s = i % 60;

            GetText((int)Texts.Timer_0).text = $"{(_m / 10 == 0 ? "0" : "")}{_m}:{(_s/10==0 ? "0" : "")}{_s}";
        }
        #endregion
        #endregion

        public void ESCClose()
        {
            GetObject((int)GameObjects.Panel_0).gameObject.SetActive(false);
            GetObject((int)GameObjects.ESC_0).gameObject.SetActive(false);
            Time.timeScale = 1;
            Manager.Managers.Instance.GameManager.IsGamePaused = false;
        }
#if UNITY_EDITOR

#endif

    }
}