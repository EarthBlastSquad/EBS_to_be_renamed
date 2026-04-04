using Data;
using Manager;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;
using Utils.Defines;

namespace UI.Popup
{
    public class UI_License : UIPopup
    {

        enum Buttons
        {
            Exit_0
        }
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            BindButton(typeof(Buttons));

            GetButton((int)Buttons.Exit_0).gameObject.BindUIEvent(ClickExit);
            return true;
        }


        private void ClickExit(PointerEventData _)
        {
            Managers.Instance.SoundManager.Play(SoundChannels.EFFECT_0, "ButtonPress", false);
            Managers.Instance.UIManager.ClosePopupUI();
        }

    }
}
