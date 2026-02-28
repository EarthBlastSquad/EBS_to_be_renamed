using Data;
using Manager;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;
using Utils.Defines;

namespace UI.Popup
{
    public class UI_Tutorial : UIPopup
    {
        private TutorialData _data;
        private int _readIdx;
        private int _tutorialIdx=0;
        #region Enum
        enum Images
        {
            //Background_0,
            ExampleImg_0
        }

        enum Buttons
        {
            Next_0,
            Prev_0,
            Exit_0
        }

        enum Texts
        {
            TutorialDescription_0
        }
        #endregion
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            BindButton(typeof(Buttons));
            BindText(typeof(Texts));
            BindImage(typeof(Images));

            _data = Managers.Instance.DataManager.TutorialDic[_tutorialIdx];
            _readIdx = 0;

            GetButton((int)Buttons.Exit_0).gameObject.BindUIEvent(ClickExit);
            GetButton((int)Buttons.Next_0).gameObject.BindUIEvent(ClickNext);
            GetButton((int)Buttons.Prev_0).gameObject.BindUIEvent(ClickPrev);
            
            if(_data.Content.Count >= 2)
            {
                GetImage((int)Images.ExampleImg_0).sprite = Managers.Instance.ResourceManager.Load<Sprite>(_data.Content[_readIdx]);
                GetText((int)Texts.TutorialDescription_0).text = _data.Content[_readIdx + 1];
            }
            else
            {
                GetText((int)Texts.TutorialDescription_0).text = "";
            }

            return true;
        }

        private void ClickExit(PointerEventData _)
        {
            Managers.Instance.SoundManager.Play(SoundChannels.EFFECT_0, "ButtonPress", false);
            Managers.Instance.UIManager.ClosePopupUI();
        }

        private void ClickNext(PointerEventData _)
        {
            Managers.Instance.SoundManager.Play(SoundChannels.EFFECT_0, "ButtonPress", false);
            if (_readIdx+2 >= _data.Content.Count)
            {
                return;
            }

            _readIdx += 2;
            
            GetImage((int)Images.ExampleImg_0).sprite = Managers.Instance.ResourceManager.Load<Sprite>(_data.Content[_readIdx]);
            GetText((int)Texts.TutorialDescription_0).text = _data.Content[_readIdx + 1];
        }

        private void ClickPrev(PointerEventData _)
        {
            Managers.Instance.SoundManager.Play(SoundChannels.EFFECT_0, "ButtonPress", false);
            if (_readIdx - 2 < 0)
            {
                return;
            }

            _readIdx -= 2;

            GetImage((int)Images.ExampleImg_0).sprite = Managers.Instance.ResourceManager.Load<Sprite>(_data.Content[_readIdx]);
            GetText((int)Texts.TutorialDescription_0).text = _data.Content[_readIdx + 1];
        }
    }
}
