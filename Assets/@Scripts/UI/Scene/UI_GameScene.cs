
using Coordinator;
using Data;
using InputHandler;
using Manager;
using Manager.Contents;
using ObjectPool;
using Scenes;
using TMPro;
using UI.Popup;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;
using Utils.Defines;

namespace UI.Scene
{
    public class UI_GameScene : UIScene
    {
        private GameScene _gs;
        private GridManager _gm;
        private UI_Shop _uis;
        private UI_TowerDatasMini _uitdm;
        private AreaUnlockManager _aum;
        private RangePreview _rp;
        private HPCoordinator _hpc;

        #region Enum
        enum GameObjects
        {
            Panel_0,
            ESC_0,
            Shop_0,
            TowerDatas_0
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
            GetObject((int)GameObjects.TowerDatas_0).gameObject.SetActive(false);
            GetButton((int)Buttons.Unlock_0).gameObject.SetActive(true);

            Managers.Instance.CurrencyManager.AddCurrency(0500);
            _gm = FindAnyObjectByType<GridManager>();
            _uis = GetObject((int)GameObjects.Shop_0).GetComponent<UI_Shop>();
            _uitdm = GetObject((int)GameObjects.TowerDatas_0).GetComponent<UI_TowerDatasMini>();
            _aum = FindAnyObjectByType<AreaUnlockManager>();
            _rp= FindAnyObjectByType<RangePreview>();
            _hpBar = GameObject.Find("HPBar").GetComponentInChildren<Slider>();
            _hpBarText = _hpBar.GetComponentInChildren<TextMeshProUGUI>();

            FindAnyObjectByType<TowerManager>().OnTowerDeadEvent += () =>
            {
                _hpBar.transform.parent.gameObject.SetActive(false);
                _rp.gameObject.SetActive(false);
            };

            if(Managers.Instance.StageManager.GetNowStageData().StageIdx == 0)
            {
                Managers.Instance.ResourceManager.Instantiate("TutorialHelper", transform, false, false);
            }
            return true;
        }

        #region HPBar
        private Slider _hpBar;
        private TextMeshProUGUI _hpBarText;
        public void SetHP(int c, int m)
        {
            _hpBar.value = Mathf.Clamp01(c / m);
            _hpBarText.text = c.ToString();
        }

        private void OnHPChangedCallback(int old,int now ,int max)
        {
            SetHP(now, max);
        }

        #endregion
        #region 팝업

        #endregion
        #region 바인드용
        protected void PauseButton(PointerEventData _)
        {
            Managers.Instance.SoundManager.Play(SoundChannels.EFFECT_0, "ButtonPress", false);
            Manager.Managers.Instance.GameManager.IsGamePaused = true;
            GetObject((int)GameObjects.Panel_0).gameObject.SetActive(true);
            GetObject((int)GameObjects.ESC_0).gameObject.SetActive(true);
            Time.timeScale = 0;

        }

        protected void UnlickButton(PointerEventData _)
        {
            Managers.Instance.SoundManager.Play(SoundChannels.EFFECT_0, "ButtonPress", false);

            _aum.TryUnlock();

            if (_aum.DoesReachedEnd())
            {
                GetButton((int)Buttons.Unlock_0).gameObject.SetActive(false);
            }
        }

        protected void WaveUI(WaveData wd)
        {
            GetText((int)Texts.Waves_0).text = $"{wd.WaveNumber}/9 Waves";
        }

        protected void CurrencyUI(int c, int cc)
        {
            GetText((int)Texts.BCK_0).text = $"{cc}";
        }

        protected void ShopUI(Vector3 _)
        {
            if(_hpc != null)
            {
                _hpc.OnHPChanged -= OnHPChangedCallback;
            }
            Vector2Int p = _gm.GetLastSelectedPos();
            if (p == new Vector2(-1, -1))
            {
                GetObject((int)GameObjects.TowerDatas_0).SetActive(false);
                _uis.CheckButton(_);
                _hpBar.transform.parent.gameObject.SetActive(false);
                return;
            }
            else if (_gm.TryGetPlacedPiece(p, out GameObject outTower) && outTower.TryGetComponent<TowerCoordinator>(out TowerCoordinator tc))
            {
                _hpBar.gameObject.SetActive(true);

                _uis.CheckButton(_);

                var pos = tc.GetAttackRangeArgs();
#if UNITY_EDITOR
                Debug.Log("사거리  표시 중?");
                Debug.Log(tc.GetData().Item3.AttackPos.Count);

#endif
                _rp.ShowAttackRange(pos.Item1, pos.Item2, pos.Item3);
                var datas=tc.GetData();
                //_tm.RetrieveTower(p);//삭제대신정보창
                _uitdm.Set(datas.Item1.TowerName,datas.Item3.Damage,datas.Item3.Cooldown,p);
                _hpBar.transform.parent.gameObject.SetActive(true);
                SetHP(datas.Item2,datas.Item1.TowerHP);
                _hpBar.transform.parent.position= ((Vector3)(p-new Vector2(6,2.55f)));

                if(outTower.TryGetComponent<HPCoordinator>(out _hpc))
                {
                    _hpc.OnHPChanged += OnHPChangedCallback;
                }
            }
            else
            {
                GetObject((int)GameObjects.TowerDatas_0).SetActive(false);
                _uis.Set(p,_);
                GetObject((int)GameObjects.Shop_0).gameObject.SetActive(true);
                _uis.ShopOpen = true;
                _hpBar.gameObject.SetActive(false);
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
            Managers.Instance.SoundManager.Play(SoundChannels.EFFECT_0, "ButtonPress", false);
            GetObject((int)GameObjects.Panel_0).gameObject.SetActive(false);
            GetObject((int)GameObjects.ESC_0).gameObject.SetActive(false);
            Time.timeScale = 1;
            Manager.Managers.Instance.GameManager.IsGamePaused = false;
        }


    }
}