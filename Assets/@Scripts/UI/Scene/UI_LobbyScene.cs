using Contents.Tower;
using Data;
using DG.Tweening;
using Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UI.Popup;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using Utils;
using Utils.Defines;
using static UnityEngine.InputSystem.InputControlScheme.MatchResult;
namespace UI.Scene
{
    public class UI_LobbyScene : UIScene
    {
        private int _stageIdx = 0;

        #region Enum
        enum GameObjects
        {
            Inventory_0,
            Tutorial_0,
            SoundSlider_0,
            SettingPanel_0
        }

        enum Buttons
        {
            InventoryOpen_0,
            InventoryClose_0,
            GameStart_0,
            StageChange_0,
            StageChange_1,
            TutorialOpen_0,
            Setting_0,
            SoundOnOff_0,
            //임시 방식, 풀링 추가시 교체
            //Item_0,
            //Item_1,
            //Item_2 //TestTower 0,1,2 흠 배열만들까
        }

        enum Texts
        {
            StageName_0,
            StageDescription_0,

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
            
            GetButton((int)Buttons.InventoryOpen_0).gameObject.BindUIEvent(InventoryOpen);
            GetButton((int)Buttons.InventoryClose_0).gameObject.BindUIEvent(MainLobby);
            GetButton((int)Buttons.GameStart_0).gameObject.BindUIEvent(GameStart);
            GetButton((int)Buttons.TutorialOpen_0).gameObject.BindUIEvent(OpenTutorial);
            //SlotSet();
            //ItemSet();
            //GetButton((int)Buttons.Slot_0).gameObject.BindUIEvent(TestGetRandomJsons);
            //GetButton((int)Buttons.Slot_1).gameObject.BindUIEvent(TestGetRandomJsons);
            //GetButton((int)Buttons.Slot_2).gameObject.BindUIEvent(TestGetRandomJsons); //이벤트 해제 만들어둬야할까 X
            GetButton((int)Buttons.StageChange_0).gameObject.BindUIEvent(StageLeft);
            GetButton((int)Buttons.StageChange_1).gameObject.BindUIEvent(StageRight);
            GetButton((int)Buttons.Setting_0).gameObject.BindUIEvent(SettingMenu);
            GetButton((int)Buttons.SoundOnOff_0).gameObject.BindUIEvent(OnOffVolume);
            GetObject((int)GameObjects.SoundSlider_0).gameObject.BindUIEvent(SetVolume, UIEventTypes.DRAG);
            GetButton((int)Buttons.Setting_0).gameObject.SetActive(true);
            GetButton((int)Buttons.SoundOnOff_0).gameObject.SetActive(false);
            GetObject((int)GameObjects.SoundSlider_0).gameObject.SetActive(false);
#if UNITY_EDITOR
            Debug.Log(GetText((int)Texts.StageDescription_0).gameObject.name);
#endif
            MainLobby(default(PointerEventData));
            GetText((int)Texts.StageName_0).text = Managers.Instance.DataManager.StageDic[_stageIdx].StageName;
            GetText((int)Texts.StageDescription_0).text = Managers.Instance.DataManager.StageDic[_stageIdx].StageDescription;

            GetImage((int)Images.Background_0).sprite = Managers.Instance.ResourceManager.Load<Sprite>(Managers.Instance.DataManager.StageDic[_stageIdx].BackgroundImgName);
            
            return true;
        }
        #region 화면 전환
        protected void MainLobby(PointerEventData _)
        {
            Managers.Instance.SoundManager.Play(SoundChannels.EFFECT_0, "ButtonPress", false);
            GetButton((int)Buttons.InventoryOpen_0).gameObject.SetActive(true);
            GetButton((int)Buttons.InventoryClose_0).gameObject.SetActive(false);
            GetButton((int)Buttons.GameStart_0).gameObject.SetActive(true);
            GetObject((int)GameObjects.Inventory_0).gameObject.SetActive(false);
            GetButton((int)Buttons.StageChange_0).gameObject.SetActive(true);
            GetButton((int)Buttons.StageChange_1).gameObject.SetActive(true);
            GetButton((int)Buttons.TutorialOpen_0).gameObject.SetActive(true);
            GetText((int)Texts.StageName_0).gameObject.SetActive(true);
            GetText((int)Texts.StageDescription_0).gameObject.SetActive(true);
            GetButton((int)Buttons.Setting_0).gameObject.SetActive(true);
        }
        protected void InventoryOpen(PointerEventData _)
        {
            //기존거 끄고 키기?
            //팝업으로 할 수 있다지만, 일단 이렇게
            Managers.Instance.SoundManager.Play(SoundChannels.EFFECT_0, "ButtonPress", false);
            GetButton((int)Buttons.InventoryOpen_0).gameObject.SetActive(false);
            GetButton((int)Buttons.InventoryClose_0).gameObject.SetActive(true);
            GetButton((int)Buttons.GameStart_0).gameObject.SetActive(false);
            GetObject((int)GameObjects.Inventory_0).gameObject.SetActive(true);
            GetButton((int)Buttons.StageChange_0).gameObject.SetActive(false);
            GetButton((int)Buttons.StageChange_1).gameObject.SetActive(false);
            GetButton((int)Buttons.TutorialOpen_0).gameObject.SetActive(false);
            GetText((int)Texts.StageName_0).gameObject.SetActive(false);
            GetText((int)Texts.StageDescription_0).gameObject.SetActive(false);
            GetButton((int)Buttons.Setting_0).gameObject.SetActive(false);
        }

        private void OpenTutorial(PointerEventData _)
        {
            Managers.Instance.SoundManager.Play(SoundChannels.EFFECT_0, "ButtonPress", false);
            var popup = Managers.Instance.UIManager.ShowPopupUI<UI_Tutorial>(GameObjects.Tutorial_0.ToString());
        }

        #endregion
        protected void GameStart(PointerEventData _)
        {
            Managers.Instance.SoundManager.Play(SoundChannels.EFFECT_0, "ButtonPress", false);
            Managers.Instance.ResourceManager.LoadAsyncAllIn("GameSceneLoaded", (key, count, totalCount) =>
            {
                if(count == totalCount)
                {
                    Managers.Instance.StageManager.Init(_stageIdx);
                    Managers.Instance.SceneManagerEx.LoadScene(SceneNames.GameScene);
                    Managers.Instance.ResourceManager.ReleaseIn("LobbySceneLoaded");
                }
            });
        }
        private bool _setting = false;
        protected void SettingMenu(PointerEventData _)
        {
            if(_setting==true)
            {
                Managers.Instance.SoundManager.Play(SoundChannels.EFFECT_0, "ButtonPress", false);
                GetButton((int)Buttons.SoundOnOff_0).gameObject.SetActive(false);
                GetObject((int)GameObjects.SoundSlider_0).SetActive(false);
                GetObject((int)GameObjects.SettingPanel_0).SetActive(false);
            }
            else
            {
                Managers.Instance.SoundManager.Play(SoundChannels.EFFECT_0, "ButtonPress", false);
                GetObject((int)GameObjects.SettingPanel_0).SetActive(true);
                GetButton((int)Buttons.SoundOnOff_0).gameObject.SetActive(true);
                GetObject((int)GameObjects.SoundSlider_0).SetActive(true);
            }
            _setting = !_setting;
        }
        protected void StageRight(PointerEventData _)
        {
            Managers.Instance.SoundManager.Play(SoundChannels.EFFECT_0, "ButtonPress", false);
            StageData data = Managers.Instance.DataManager.StageDic[_stageIdx];
            _stageIdx = data.NextIdx;
            data = Managers.Instance.DataManager.StageDic[_stageIdx];
            GetText((int)Texts.StageName_0).text = data.StageName;
            GetText((int)Texts.StageDescription_0).text = data.StageDescription;
            
            GetImage((int)Images.Background_0).sprite = Managers.Instance.ResourceManager.Load<Sprite>(Managers.Instance.DataManager.StageDic[_stageIdx].BackgroundImgName);
        }
        protected void StageLeft(PointerEventData _)
        {
            Managers.Instance.SoundManager.Play(SoundChannels.EFFECT_0, "ButtonPress", false);
            StageData data = Managers.Instance.DataManager.StageDic[_stageIdx];
            _stageIdx = data.PrevIdx;
            data = Managers.Instance.DataManager.StageDic[_stageIdx];
            GetText((int)Texts.StageName_0).text = data.StageName;
            GetText((int)Texts.StageDescription_0).text = data.StageDescription;
            
            GetImage((int)Images.Background_0).sprite = Managers.Instance.ResourceManager.Load<Sprite>(Managers.Instance.DataManager.StageDic[_stageIdx].BackgroundImgName);
        }
        protected void SetVolume(PointerEventData _)
        {
            Managers.Instance.GameManager.SoundValue = _.pointerDrag.transform.GetComponent<Slider>().value / 15;
            if (Managers.Instance.GameManager.SoundSet == false)
            {
                return;
            }
            Managers.Instance.SoundManager.Play(0, "TestSound", true, Managers.Instance.GameManager.SoundValue);
        }
        protected void OnOffVolume(PointerEventData _)
        {
            Managers.Instance.GameManager.SoundSet = !Managers.Instance.GameManager.SoundSet;
            Managers.Instance.SoundManager.Play(SoundChannels.EFFECT_0, "ButtonPress", false);
            if (Managers.Instance.GameManager.SoundSet == false)
            {
                GetButton((int)Buttons.SoundOnOff_0).GetComponent<Image>().sprite = Managers.Instance.ResourceManager.Load<Sprite>("stone_button_short_off");
                GetButton((int)Buttons.SoundOnOff_0).gameObject.GetChildGameObject("SoundOnOff_0_0").GetComponent<Image>().sprite = Managers.Instance.ResourceManager.Load<Sprite>("volume_off");
                Managers.Instance.SoundManager.StopAll();
            }
            else
            {
                GetButton((int)Buttons.SoundOnOff_0).GetComponent<Image>().sprite = Managers.Instance.ResourceManager.Load<Sprite>("stone_button_short_on");
                GetButton((int)Buttons.SoundOnOff_0).gameObject.GetChildGameObject("SoundOnOff_0_0").GetComponent<Image>().sprite = Managers.Instance.ResourceManager.Load<Sprite>("volume_on");
                Managers.Instance.SoundManager.Play(0, "TestSound", true, Managers.Instance.GameManager.SoundValue);
            }
        }
        private void Start()
        {
            
            Init();
        }
    }
}