
using Coordinator;
using Data;
using InputHandler;
using Manager;
using Manager.Contents;
using Scenes;
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
        #region Enum
        enum GameObjects
        {
            Panel_0,
            ESC_0,
            Shop_0
        }

        enum Buttons
        {
            Pause_0
        }

        enum Texts
        {
            Waves_0,
            Timer_0,
            BCK_0
        }

        enum Images
        {
            Background_0
        }
        #endregion

        public override bool Init()
        {
            if (base.Init() == false)
                return false;
            // 오브젝트 바인딩
            BindObject(typeof(GameObjects));
            BindButton(typeof(Buttons));
            BindText(typeof(Texts));
            BindImage(typeof(Images));

            _gs = GameObject.Find("GameScene").GetComponent<GameScene>();
            _gs.OnWaveChanged -= WaveUI;
            _gs.OnWaveChanged += WaveUI;
            Managers.Instance.CurrencyManager.OnCurrencyChangedEvent -= CurrencyUI;
            Managers.Instance.CurrencyManager.OnCurrencyChangedEvent += CurrencyUI;
            GetButton((int)Buttons.Pause_0).gameObject.BindUIEvent(PauseButton);

            GridInputHandler gh = FindAnyObjectByType<GridInputHandler>();
            gh.mouseUpSubscriberEvent -= ShopUI;
            gh.mouseUpSubscriberEvent += ShopUI;

            GetObject((int)GameObjects.Panel_0).gameObject.SetActive(false);
            GetObject((int)GameObjects.ESC_0).gameObject.SetActive(false);
            GetObject((int)GameObjects.Shop_0).gameObject.SetActive(false);
#if UNITY_EDITOR
            Managers.Instance.CurrencyManager.AddCurrency(0404);
#endif
            _gm = FindAnyObjectByType<GridManager>();
            _tm = FindAnyObjectByType<TowerManager>();
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
            if (_gm.GetLastSelectedPos() == new Vector2(-1, -1))
            {
                
                GetObject((int)GameObjects.Shop_0).gameObject.SetActive(false);
                return;
            }
            else if(_gm.TryGetPlacedPiece(_gm.GetLastSelectedPos(),out GameObject outTower)&&outTower.TryGetComponent<TowerCoordinator>(out TowerCoordinator tc))
            {
                
            }
            else
            {
                GetObject((int)GameObjects.Shop_0).gameObject.SetActive(true);
            }
        }

        #region 타이머
        private int _m=0, _s=0;
        private float _time = 0;
        private void Update()
        {
            _time += Time.deltaTime;
            TimerUI();
        }
        protected void TimerUI()
        {
            _m = (int)_time / 60;
            if(_s== (int)_time % 60)
            {
                return;
            }
            _s = (int)_time % 60;

            GetText((int)Texts.Timer_0).text = $"{(_m / 10 == 0 ? "0" : "")}{_m}:{(_s/10==0 ? "0" : "")}{_s}";
        }
        #endregion
        #endregion
        private void Awake()
        {
            Init();
        }

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