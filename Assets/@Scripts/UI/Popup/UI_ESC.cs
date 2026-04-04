using Contents.Tower;
using Manager;
using UI.Scene;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;
using Utils.Defines;
namespace UI.Popup
{
    public class UI_ESC : UIPopup
    {
        private UI_GameScene _ugs;
        enum Buttons
        {
            Back_0,
            Exit_0
        }
        enum Texts
        {
            PauseText_0
        }

        public override bool Init()
        {
            if (base.Init() == false)
                return false;
            BindButton(typeof(Buttons));
            BindText(typeof(Texts));
            _ugs=transform.parent.GetComponent<UI_GameScene>();
            GetButton((int)Buttons.Back_0).gameObject.BindUIEvent(Back);
            GetButton((int)Buttons.Exit_0).gameObject.BindUIEvent(Exit);
            return true;
        }
        private void Awake()
        {
            Init();
        }
        protected void Back(PointerEventData _)
        {
            Managers.Instance.SoundManager.Play(SoundChannels.EFFECT_0, "ButtonPress", false);
            _ugs.ESCClose();
        }

        protected void Exit(PointerEventData _)
        {
            Managers.Instance.SoundManager.Play(SoundChannels.EFFECT_0, "ButtonPress", false);
            Managers.Instance.ResourceManager.LoadAsyncAllIn("LobbySceneLoaded", (key, count, totalCount) =>
            {
                if(count == totalCount)
                {
                    _ugs.ESCClose();
                    Managers.Instance.SceneManagerEx.LoadScene(SceneNames.LobbyScene);
                    Managers.Instance.ResourceManager.ReleaseIn("GameSceneLoaded");
                    Managers.Instance.ResourceManager.ReleaseIn("TutorialGameSceneLoaded"); //어차피 로드된게 없으면 내부적으로 바로 return하므로 여기서 추가.
                }
            });
        }
    }
}