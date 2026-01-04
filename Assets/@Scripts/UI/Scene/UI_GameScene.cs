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
    public class UI_GameScene : UIScene
    {
        #region Enum
        enum GameObjects
        {
            TowerDataPopUp_0
        }

        enum Buttons
        {
            Slot_0,
            Slot_1,
            Slot_2,
        }

        enum Texts
        {

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
            return true;
        }
        #region 팝업

        #endregion
        #region 바인드용

        #endregion
        private void Awake()
        {
            Init();
        }


#if UNITY_EDITOR

#endif

    }
}