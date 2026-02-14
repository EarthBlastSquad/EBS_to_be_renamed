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


namespace UI.Scene
{
    public class UI_EndingScene : UIScene
    {
        private TextMeshProUGUI _textBar;
        private EndingSceneManager _esm;
        #region Enum
        enum GameObjects
        {

        }

        enum Buttons
        {
            TextBar_0
        }

        enum Texts
        {

        }
        enum Images
        {
            Image_0
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
            _textBar = GetButton((int)Buttons.TextBar_0).GetComponentInChildren<TextMeshProUGUI>();
            _esm=FindAnyObjectByType<EndingSceneManager>();
            GetButton((int)Buttons.TextBar_0).gameObject.BindUIEvent(NextText);
            return true;
        }

        private void GetContents()
        {
            string s = "";
            switch (_esm.GetNowContent(ref s))
            {
                case EndingContentType.TYPE_IMAGE:
                    GetImage((int)Images.Image_0).sprite = Managers.Instance.ResourceManager.Load<Sprite>(s) as Sprite;
                    GetContents();
                    break;
                case EndingContentType.TYPE_TEXT:
                    _textBar.text=s;
                    break;
                case EndingContentType.TYPE_SOUND:
                    Managers.Instance.SoundManager.Play(0, s, true, Managers.Instance.GameManager.SoundValue);
                    GetContents();
                    break;
                case EndingContentType.TYPE_INVALID:
                    Managers.Instance.SceneManagerEx.LoadScene(SceneNames.LobbyScene);
                    return;
            }
        }
        #region 팝업

        #endregion
        #region 바인드용
        protected void NextText(PointerEventData _)
        {
            GetContents();
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
