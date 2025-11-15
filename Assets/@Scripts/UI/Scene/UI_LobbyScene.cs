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

        }

        enum Buttons
        {
            InventoryOpen_0,
            InventoryClose_0,
            GameStart_0,
            Slot_0,
            Slot_1,
            Slot_2,
            //임시 방식, 풀링 추가시 교체
            Item_0,
            Item_1,
            Item_2 //TestTower 0,1,2 흠 배열만들까
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
            SlotSet();
            ItemSet();
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
            GetButton((int)Buttons.Slot_0).gameObject.SetActive(false);
            GetButton((int)Buttons.Slot_1).gameObject.SetActive(false);
            GetButton((int)Buttons.Slot_2).gameObject.SetActive(false);
            GetButton((int)Buttons.Item_0).gameObject.SetActive(false);
            GetButton((int)Buttons.Item_1).gameObject.SetActive(false);
            GetButton((int)Buttons.Item_2).gameObject.SetActive(false);
        }
        protected void InventoryOpen(PointerEventData _)
        {
            //기존거 끄고 키기?
            //팝업으로 할 수 있다지만, 일단 이렇게
            GetButton((int)Buttons.InventoryOpen_0).gameObject.SetActive(false);
            GetButton((int)Buttons.InventoryClose_0).gameObject.SetActive(true);
            GetButton((int)Buttons.GameStart_0).gameObject.SetActive(false);
            GetButton((int)Buttons.Slot_0).gameObject.SetActive(true);
            GetButton((int)Buttons.Slot_1).gameObject.SetActive(true);
            GetButton((int)Buttons.Slot_2).gameObject.SetActive(true);
            GetButton((int)Buttons.Item_0).gameObject.SetActive(true);
            GetButton((int)Buttons.Item_1).gameObject.SetActive(true);
            GetButton((int)Buttons.Item_2).gameObject.SetActive(true);
            //아이템들 넣기

        }
        #endregion
        //protected void TestGetRandomJsons(PointerEventData _)
        //{
        //    _.pointerPress.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{Managers.Instance.DataManager.TowerDic[1].TowerName}";
        //}
        #region 버튼 세팅
        private void ItemSet()
        {
            for (int i = 0; i < 3; i++) //3으로 써놨지만 UI의 인피니티 풀 개수로 들어갈 예정
            {
                GetButton(i + (int)Buttons.Item_0).gameObject.BindUIEvent(SelectItem);
                //GetButton(i).image=; 샘플이 없네... 몰라 일단 텍스트
                GetButton(i+ (int)Buttons.Item_0).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text=$"{Managers.Instance.GameManager.OwnedTowers[i].TowerData.TowerName}";
                GetButton(i + (int)Buttons.Item_0).tag = $"{i}";
            }
        }
        private void SlotSet()
        {
            for(int i=0; i<3; i++)
            {
                GetButton(i + (int)Buttons.Slot_0).gameObject.BindUIEvent(SelectSlot);
                if (Managers.Instance.GameManager.EquippedTowers[i] is default(Tower))
                {
                    continue;
                }
                GetButton(i+(int)Buttons.Slot_0).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{Managers.Instance.GameManager.EquippedTowers[i].TowerData.TowerName}";
            }
        }
        #endregion
        #region 버튼전용 함수들
        private TextMeshProUGUI SelectedSlot; //string으로는 옅은 복사가 안되는 것 같음
        private sbyte SelectedSlotIndex;
        protected void SelectSlot(PointerEventData _)
        {
            SelectedSlot=_.pointerPress.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            sbyte.TryParse(Regex.Match(_.pointerPress.transform.name, @" ?_(\d+)$").Groups[1].Value, out SelectedSlotIndex);
            SelectedSlot.text = "";
        }
        protected void SelectItem(PointerEventData _)
        {
            Managers.Instance.GameManager.EquipTower(SelectedSlotIndex, Managers.Instance.GameManager.OwnedTowers[int.Parse(_.pointerPress.transform.tag)]);
            SelectedSlot.text = Managers.Instance.GameManager.OwnedTowers[int.Parse(_.pointerPress.transform.tag)].TowerData.TowerName;
        }
        #endregion
        private void Awake()
        {

        }
        private void Start()
        {
            
            Init();
        }
    }
}