using Data;
using DG.Tweening;
using Manager;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using Utils;
using Utils.Defines;
namespace UI.Scene
{
    public class UI_TitleScene : UIScene
    {
        #region Enum
        enum GameObjects
        {
            Slider_0,
            //SoundSlider_0
        }

        enum Buttons
        {
            //Setting_0,
            StartButton_0,
            //SoundOnOff_0,
            //Back_0
        }

        enum Texts
        {
            //Title_0
        }
        #endregion

        bool isPreload = false;

        public override bool Init()
        {
            if (base.Init() == false)
                return false;
            // 오브젝트 바인딩
            BindObject(typeof(GameObjects));
            BindButton(typeof(Buttons));
            //BindText(typeof(Texts));

            GetObject((int)GameObjects.Slider_0).GetComponent<Slider>().value = 0;

            GetButton((int)Buttons.StartButton_0).gameObject.BindUIEvent((_) =>
            {
                Managers.Instance.SoundManager.Play(SoundChannels.EFFECT_0, "ButtonPress", false);
                if (isPreload)
                    Managers.Instance.ResourceManager.LoadAsyncAllIn("LobbySceneLoaded", (key, count, totalCount) =>
                    {
                        if(count == totalCount)
                        {
                            Managers.Instance.SceneManagerEx.LoadScene(SceneNames.LobbyScene); //이건 일회성으로 뭐 더 볼일 없기도 할 듯 하여
                        }
                    });
                //Managers.Instance.SceneManagerEx.LoadScene(SceneNames.LobbyScene); //이건 일회성으로 뭐 더 볼일 없기도 할 듯 하여
            });
            //GetButton((int)Buttons.Setting_0).gameObject.BindUIEvent(SettingMenu);
            //GetButton((int)Buttons.SoundOnOff_0).gameObject.BindUIEvent(OnOffVolume);
            //GetButton((int)Buttons.Back_0).gameObject.BindUIEvent(TitleMenu);
            //GetObject((int)GameObjects.SoundSlider_0).gameObject.BindUIEvent(SetVolume, UIEventTypes.DRAG);
            GetButton((int)Buttons.StartButton_0).gameObject.SetActive(false);
            //GetButton((int)Buttons.Setting_0).gameObject.SetActive(false);
            //GetButton((int)Buttons.SoundOnOff_0).gameObject.SetActive(false);
            //GetButton((int)Buttons.Back_0).gameObject.SetActive(false);
            //GetObject((int)GameObjects.SoundSlider_0).gameObject.SetActive(false);
            return true;
        }
        #region 팝업
        //protected void SettingMenu(PointerEventData _)
        //{
        //    Managers.Instance.SoundManager.Play(SoundChannels.EFFECT_0, "ButtonPress", false);
        //    GetButton((int)Buttons.StartButton_0).gameObject.SetActive(false);
        //    GetButton((int)Buttons.Setting_0).gameObject.SetActive(false);
        //    GetText((int)Texts.Title_0).gameObject.SetActive(false);
        //    GetButton((int)Buttons.SoundOnOff_0).gameObject.SetActive(true);
        //    GetObject((int)GameObjects.SoundSlider_0).gameObject.SetActive(true);
        //    GetButton((int)Buttons.Back_0).gameObject.SetActive(true);
        //}
        //protected void TitleMenu(PointerEventData _)
        //{
        //    Managers.Instance.SoundManager.Play(SoundChannels.EFFECT_0, "ButtonPress", false);
        //    GetButton((int)Buttons.StartButton_0).gameObject.SetActive(true);
        //    GetButton((int)Buttons.Setting_0).gameObject.SetActive(true);
        //    GetText((int)Texts.Title_0).gameObject.SetActive(true);
        //    GetButton((int)Buttons.Back_0).gameObject.SetActive(false);
        //    GetButton((int)Buttons.SoundOnOff_0).gameObject.SetActive(false);
        //    GetObject((int)GameObjects.SoundSlider_0).gameObject.SetActive(false);
        //}
        #endregion
        #region 바인드용
        //protected void SetVolume(PointerEventData _)
        //{
        //    Managers.Instance.GameManager.SoundValue = _.pointerDrag.transform.GetComponent<Slider>().value/15;
        //    if(Managers.Instance.GameManager.SoundSet==false)
        //    {
        //        return;
        //    }
        //    Managers.Instance.SoundManager.Play(0, "TestSound", true, Managers.Instance.GameManager.SoundValue);
        //}
        //protected void OnOffVolume(PointerEventData _)
        //{
        //    Managers.Instance.GameManager.SoundSet = !Managers.Instance.GameManager.SoundSet;
        //    Managers.Instance.SoundManager.Play(SoundChannels.EFFECT_0, "ButtonPress", false);
        //    if (Managers.Instance.GameManager.SoundSet == false)
        //    {
        //        GetButton((int)Buttons.SoundOnOff_0).GetComponent<Image>().sprite = Managers.Instance.ResourceManager.Load<Sprite>("stone_button_short_off");
        //        GetButton((int)Buttons.SoundOnOff_0).gameObject.GetChildGameObject("SoundOnOff_0_0").GetComponent<Image>().sprite = Managers.Instance.ResourceManager.Load<Sprite>("volume_off");
        //        Managers.Instance.SoundManager.StopAll();
        //    }
        //    else
        //    {
        //        GetButton((int)Buttons.SoundOnOff_0).GetComponent<Image>().sprite = Managers.Instance.ResourceManager.Load<Sprite>("stone_button_short_on");
        //        GetButton((int)Buttons.SoundOnOff_0).gameObject.GetChildGameObject("SoundOnOff_0_0").GetComponent<Image>().sprite = Managers.Instance.ResourceManager.Load<Sprite>("volume_on");
        //        Managers.Instance.SoundManager.Play(0, "TestSound", true, Managers.Instance.GameManager.SoundValue);
        //    }
        //}
        #endregion
        private void Awake()
        {
            Init();
        }
        private void Start()
        {
            //Managers.Instance.ResourceManager.LoadAsyncAllIn("TestPreLoad", (key, count, totalCount) => //PreLoad를 "지우고" TestPreLoad를 만듬? 야!!!!!!!!!
            //{
            //    GetObject((int)GameObjects.Slider_0).GetComponent<Slider>().value = (float)count / totalCount;
            //    if (count == totalCount)
            //    {
            //        isPreload = true;
            //        GetButton((int)Buttons.StartButton_0).gameObject.SetActive(true);
            //        GetButton((int)Buttons.Setting_0).gameObject.SetActive(true);
            //        Managers.Instance.DataManager.Init();
            //        Managers.Instance.GameManager.Init();
            //        Managers.Instance.SoundManager.Init();

            //        StartButtonAnimation();
            //        GetObject((int)GameObjects.SoundSlider_0).transform.GetComponent<Slider>().value = Managers.Instance.GameManager.SoundValue * 15;
            //        if (Managers.Instance.GameManager.SoundSet == true)
            //        {
            //            Managers.Instance.SoundManager.Play(0, "TestSound", true, Managers.Instance.GameManager.SoundValue);
            //        }
            //    }
            //});
            //return;
            Managers.Instance.ResourceManager.LoadAsyncAllIn("TitleSceneLoaded", (key, count, totalCount) =>
            {
                GetObject((int)GameObjects.Slider_0).GetComponent<Slider>().value = (float)count / totalCount;
                if (count == totalCount)
                {
                    isPreload = true;
                    GetButton((int)Buttons.StartButton_0).gameObject.SetActive(true);
                    //GetButton((int)Buttons.Setting_0).gameObject.SetActive(true);
                    Managers.Instance.DataManager.Init();
                    Managers.Instance.GameManager.Init();
                    Managers.Instance.SoundManager.Init();

                    StartButtonAnimation();
                    //GetObject((int)GameObjects.SoundSlider_0).transform.GetComponent<Slider>().value = Managers.Instance.GameManager.SoundValue * 15;
                    if (Managers.Instance.GameManager.SoundSet == true)
                    {
                        Managers.Instance.SoundManager.Play(0, "TitleBGM", true, Managers.Instance.GameManager.SoundValue);
                    }
                }
            });
        }

        void StartButtonAnimation()
        {
            GetButton((int)Buttons.StartButton_0).GetComponentInChildren<TextMeshProUGUI>().DOFade(0, 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutCubic).Play();
        }
    }
}