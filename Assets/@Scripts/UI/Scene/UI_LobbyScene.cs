using Data;
using DG.Tweening;
using Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using Utils;
using Utils.Defines;
namespace UI.Scene
{
    public class UI_LobbyScene : UIScene
    {
        #region Enum
        enum GameObjects
        {

        }

        enum Buttons
        {
            InventoryOpen_0,
            GameStart_0,
            Slot_0,
            Slot_1,
            Slot_2
        }

        enum Texts
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

            GetButton((int)Buttons.InventoryOpen_0).gameObject.BindUIEvent(InventoryOpen);
            GetButton((int)Buttons.Slot_0).gameObject.BindUIEvent(TestGetRandomJsons);
            GetButton((int)Buttons.Slot_1).gameObject.BindUIEvent(TestGetRandomJsons);
            GetButton((int)Buttons.Slot_2).gameObject.BindUIEvent(TestGetRandomJsons); //이벤트 해제 만들어둬야할까
            GetButton((int)Buttons.Slot_0).gameObject.SetActive(false);
            GetButton((int)Buttons.Slot_1).gameObject.SetActive(false);
            GetButton((int)Buttons.Slot_2).gameObject.SetActive(false);
            return true;
        }
        protected void InventoryOpen(PointerEventData _)
        {
            //기존거 끄고 키기?
            //팝업으로 할 수 있다지만, 일단 이렇게
            GetButton((int)Buttons.InventoryOpen_0).gameObject.SetActive(false);
            GetButton((int)Buttons.GameStart_0).gameObject.SetActive(false);
            GetButton((int)Buttons.Slot_0).gameObject.SetActive(true);
            GetButton((int)Buttons.Slot_1).gameObject.SetActive(true);
            GetButton((int)Buttons.Slot_2).gameObject.SetActive(true);
        }
        protected void TestGetRandomJsons(PointerEventData _)
        {
            _.pointerPress.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{Managers.Instance.DataManager.TowerDic[1].TowerName}";
        }
        private void Awake()
        {

        }
        private void Start()
        {
            Init();
        }
    }
}