using Data;
using DG.Tweening;
using Manager;
using Manager.Contents;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;
using Utils.Defines;


namespace UI.Testing
{
    public class UI_TestGameScene : UI.Scene.UIScene
    {
        private bool _Q = false;
        #region Enum
        enum GameObjects
        {
            ScrollRect
        }

        enum Buttons
        {
            Logs,
            Cheat,
            Faster,
            ResetFaster
        }

        enum Texts
        {

        }
        enum Images
        {
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
            GetButton((int)Buttons.Logs).gameObject.BindUIEvent(Logs);
            GetButton((int)Buttons.Cheat).gameObject.BindUIEvent(Cheat);
            GetButton((int)Buttons.Faster).gameObject.BindUIEvent(Faster);
            GetButton((int)Buttons.ResetFaster).gameObject.BindUIEvent(ResetFaster);
            _fast = GetButton((int)Buttons.Faster).GetComponentInChildren<TextMeshProUGUI>();
            GetObject((int)GameObjects.ScrollRect).SetActive(false);
            return true;
        }
        #region 팝업

        #endregion
        private TextMeshProUGUI _fast;
        #region 바인드용
        protected void Logs(PointerEventData _)
        {
            _Q = !_Q;
            GetObject((int)GameObjects.ScrollRect).SetActive(_Q);

        }
        protected void Cheat(PointerEventData _) //아직 생각나는게 돈복사버그뿐
        {
            Managers.Instance.CurrencyManager.AddCurrency(666775);
        }
        protected void Faster(PointerEventData _)
        {
            Time.timeScale += 1;
            _fast.text = $"X{Time.timeScale}";
        }
        protected void ResetFaster(PointerEventData _)
        {
            Time.timeScale = 1;
            _fast.text = $"X1";
        }
        #endregion
        private void Awake()
        {
            Init();
        }
        private void Start()
        {

        }
    }
}
