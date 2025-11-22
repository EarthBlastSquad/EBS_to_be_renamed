using Contents.Tower;
using Data;
using DG.Tweening;
using Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEditor.Experimental.GraphView;
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
        #region Enum
        enum GameObjects
        {
            Inventory_0,
        }

        enum Buttons
        {
            InventoryOpen_0,
            InventoryClose_0,
            GameStart_0,

            //임시 방식, 풀링 추가시 교체
            //Item_0,
            //Item_1,
            //Item_2 //TestTower 0,1,2 흠 배열만들까
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
            GetButton((int)Buttons.InventoryClose_0).gameObject.BindUIEvent(MainLobby);
            GetButton((int)Buttons.GameStart_0).gameObject.BindUIEvent(GameStart);
            //SlotSet();
            //ItemSet();
            //GetButton((int)Buttons.Slot_0).gameObject.BindUIEvent(TestGetRandomJsons);
            //GetButton((int)Buttons.Slot_1).gameObject.BindUIEvent(TestGetRandomJsons);
            //GetButton((int)Buttons.Slot_2).gameObject.BindUIEvent(TestGetRandomJsons); //이벤트 해제 만들어둬야할까 X
            MainLobby(default(PointerEventData));
            return true;
        }
        #region 화면 전환
        protected void MainLobby(PointerEventData _)
        {
            GetButton((int)Buttons.InventoryOpen_0).gameObject.SetActive(true);
            GetButton((int)Buttons.InventoryClose_0).gameObject.SetActive(false);
            GetButton((int)Buttons.GameStart_0).gameObject.SetActive(true);
            GetObject((int)GameObjects.Inventory_0).gameObject.SetActive(false);
        }
        protected void InventoryOpen(PointerEventData _)
        {
            //기존거 끄고 키기?
            //팝업으로 할 수 있다지만, 일단 이렇게
            GetButton((int)Buttons.InventoryOpen_0).gameObject.SetActive(false);
            GetButton((int)Buttons.InventoryClose_0).gameObject.SetActive(true);
            GetButton((int)Buttons.GameStart_0).gameObject.SetActive(false);
            GetObject((int)GameObjects.Inventory_0).gameObject.SetActive(true);

        }
        #endregion
        protected void GameStart(PointerEventData _)
        {
            Managers.Instance.SceneManagerEx.LoadScene(SceneNames.Test);
        }
        private void Start()
        {
            
            Init();
        }
    }
}