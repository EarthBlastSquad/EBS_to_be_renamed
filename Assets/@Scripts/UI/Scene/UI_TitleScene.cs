using Data;
using DG.Tweening;
using Manager;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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
            Slider
        }

        enum Buttons
        {
            Setting_0,
            StartButton_1
        }

        enum Texts
        {
            Title_0
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
            BindText(typeof(Texts));

            GetObject((int)GameObjects.Slider).GetComponent<Slider>().value = 0;

            GetButton((int)Buttons.StartButton_1).gameObject.BindUIEvent((_) =>
            {
                if (isPreload)
                   Managers.Instance.SceneManagerEx.LoadScene(SceneNames.LobbyScene);
            });
            GetButton((int)Buttons.StartButton_1).gameObject.SetActive(false);
            GetButton((int)Buttons.Setting_0).gameObject.SetActive(false);
            return true;
        }

        private void Awake()
        {
            Init();
        }
        private void Start()
        {
            Managers.Instance.ResourceManager.LoadAsyncAllIn("PreLoad", (key, count, totalCount) =>
            {
                GetObject((int)GameObjects.Slider).GetComponent<Slider>().value = (float)count / totalCount;
                if (count == totalCount)
                {
                    isPreload = true;
                    GetButton((int)Buttons.StartButton_1).gameObject.SetActive(true);
                    GetButton((int)Buttons.Setting_0).gameObject.SetActive(true);
                    Managers.Instance.DataManager.Init();
                    //Managers.Instance.GameManager.Init();
                    //Managers.Instance.TimeManager.Init(); //지금 생각할 게 아님
                    StartButtonAnimation();
                }
            });
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.F1))
            {
                Managers.Instance.UIManager.ShowToast("test");
            }
        }
#endif
        void StartButtonAnimation()
        {
            GetText((int)Texts.Title_0).DOFade(0, 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutCubic).Play();
        }
    }
}