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
        private int _stageIdx = -6667750;

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
            StageChange_0,
            StageChange_1
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
            GetButton((int)Buttons.StageChange_0).gameObject.BindUIEvent(StageLeft);
            GetButton((int)Buttons.StageChange_1).gameObject.BindUIEvent(StageRight);
#if UNITY_EDITOR
            Debug.Log(GetText((int)Texts.StageDescription_0).gameObject.name);
#endif
            MainLobby(default(PointerEventData));
            GetText((int)Texts.StageName_0).text = Managers.Instance.DataManager.StageDic[_stageIdx].StageName;
            GetText((int)Texts.StageDescription_0).text = Managers.Instance.DataManager.StageDic[_stageIdx].StageDescription;
            return true;
        }
        #region 화면 전환
        protected void MainLobby(PointerEventData _)
        {
            GetButton((int)Buttons.InventoryOpen_0).gameObject.SetActive(true);
            GetButton((int)Buttons.InventoryClose_0).gameObject.SetActive(false);
            GetButton((int)Buttons.GameStart_0).gameObject.SetActive(true);
            GetObject((int)GameObjects.Inventory_0).gameObject.SetActive(false);
            GetButton((int)Buttons.StageChange_0).gameObject.SetActive(true);
            GetButton((int)Buttons.StageChange_1).gameObject.SetActive(true);
            GetText((int)Texts.StageName_0).gameObject.SetActive(true);
            GetText((int)Texts.StageDescription_0).gameObject.SetActive(true);
        }
        protected void InventoryOpen(PointerEventData _)
        {
            //기존거 끄고 키기?
            //팝업으로 할 수 있다지만, 일단 이렇게
            GetButton((int)Buttons.InventoryOpen_0).gameObject.SetActive(false);
            GetButton((int)Buttons.InventoryClose_0).gameObject.SetActive(true);
            GetButton((int)Buttons.GameStart_0).gameObject.SetActive(false);
            GetObject((int)GameObjects.Inventory_0).gameObject.SetActive(true);
            GetButton((int)Buttons.StageChange_0).gameObject.SetActive(false);
            GetButton((int)Buttons.StageChange_1).gameObject.SetActive(false);
            GetText((int)Texts.StageName_0).gameObject.SetActive(false);
            GetText((int)Texts.StageDescription_0).gameObject.SetActive(false);
        }
        #endregion
        protected void GameStart(PointerEventData _)
        {
            Managers.Instance.StageManager.Init(_stageIdx);
            Managers.Instance.SceneManagerEx.LoadScene(SceneNames.GameScene);
        }

        protected void StageRight(PointerEventData _)
        {
            if(++_stageIdx> Managers.Instance.DataManager.StageDic.Count)
            {
                //_stageIdx = 0;
            }
            GetText((int)Texts.StageName_0).text = Managers.Instance.DataManager.StageDic[_stageIdx].StageName;
            GetText((int)Texts.StageDescription_0).text = Managers.Instance.DataManager.StageDic[_stageIdx].StageDescription;
        }
        protected void StageLeft(PointerEventData _)
        {
            if(--_stageIdx<0)
            {
                //_stageIdx = Managers.Instance.DataManager.StageDic.Count;
            }
            GetText((int)Texts.StageName_0).text = Managers.Instance.DataManager.StageDic[_stageIdx].StageName;
            GetText((int)Texts.StageDescription_0).text = Managers.Instance.DataManager.StageDic[_stageIdx].StageDescription;
        }
        private void Start()
        {
            
            Init();
        }
    }
}