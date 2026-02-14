using Manager;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UI;
using UI.Popup;
using UI.Popup.Cell;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;
using Utils.Defines;

namespace ObjectPool
{
    public class UI_ItemInfPool : UIPopup
    {
        private LinkedList<UnityEngine.UI.Button> Items = new LinkedList<UnityEngine.UI.Button>();
        private int _topIndex = 0;

        enum GameObjects
        {

        }
        enum Buttons
        {
            Inventory_0_0,
            Inventory_0_1,
            Inventory_0_2,
            Inventory_0_3,
            Inventory_0_4,
            Inventory_0_5,
            Inventory_0_6,
            Inventory_0_7,
            Inventory_0_8,
            Inventory_0_9,
            Inventory_0_10,
            Inventory_0_11,
            Inventory_0_12,
            Inventory_0_13,
            Inventory_0_14,
            Inventory_0_15,
            Inventory_0_16,
            Inventory_0_17,
            Inventory_0_18,
            Inventory_0_19,
        }
        enum Texts
        {

        }
        public override bool Init()
        {
            if (base.Init() == false)
                return false;
            BindObject(typeof(GameObjects));
            BindButton(typeof(Buttons));
            BindText(typeof(Texts));
            Itemset();
            _rectTransform= GetComponent<RectTransform>();
            _uI_IP = transform.parent.GetComponent<UI_InventoryPopup>();
            return true;
        }

        private void Itemset()
        {
            int end = 20;
            int c = Manager.Managers.Instance.GameManager.OwnedTowers.Count();
            if (c<20)
            {
                end = c;
                transform.parent.GetComponentInChildren<UnityEngine.UI.Slider>().maxValue = end / 9;
            }
            for (int i = 0; i < end; i++) //3으로 써놨지만 ui의 인피니티 풀 개수로 들어갈 예정
            {
                UnityEngine.UI.Button b = GetButton(i + (int)Buttons.Inventory_0_0);
                b.gameObject.BindUIEvent(SelectItem);
                //getbutton(i).image=; 샘플이 없네... 몰라 일단 텍스트
                b.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{Manager.Managers.Instance.GameManager.OwnedTowers[i].TowerData.TowerName}";
                //GetButton(i + (int)Buttons.Item_0).tag = $"{i}";
                b.transform.GetComponent<ItemCell>().TowerSet(Manager.Managers.Instance.GameManager.OwnedTowers[i]);
                Items.AddLast(b);
            }
            for(int j=end;j<20;j++)
            {
                GetButton(j + (int)Buttons.Inventory_0_0).gameObject.SetActive(false);
            }
        }
        private UI_InventoryPopup _uI_IP;
        protected void SelectItem(PointerEventData _)
        {
            Contents.Tower.Tower t = _.pointerPress.GetComponent<ItemCell>().ICTower;
            if (t is null)
            {
                return;
            }
            _uI_IP.OpenTDP(t);
            //Managers.Instance.GameManager.EquipTower(_uI_IP.SelectedSlotIndex, t);
            //_uI_IP.SelectedSlot.text = t.TowerData.TowerName;
        }

        private RectTransform _rectTransform;
        public void Slide(int sliderValue)
        {
            int newIndex = sliderValue;
            int delta = newIndex - _topIndex;
            if (delta == 0)
            {
                return;
            }
            _topIndex = newIndex;
            _rectTransform.anchoredPosition += new Vector2(0, 150 * delta);
            if (delta > 0)
            {
                int moveCount = 5 * delta;
                for (int n = 0; n < moveCount; n++)
                {
                    UnityEngine.UI.Button btn = Items.First.Value;
                    btn.GetComponent<ItemCell>().TowerSet(Manager.Managers.Instance.GameManager.OwnedTowers[(_topIndex * 5) + 15 + n]);
                    btn.GetComponentInChildren<TextMeshProUGUI>().text = Manager.Managers.Instance.GameManager.OwnedTowers[(_topIndex * 5) + 15 + n].TowerData.TowerName;
                    RectTransform rt = btn.GetComponent<RectTransform>();
                    rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, -450 - _rectTransform.anchoredPosition.y);
                    Items.RemoveFirst();
                    Items.AddLast(btn);
                }
            }
            else
            {
                int moveCount = 5 * (-delta);
                for (int n = moveCount - 1; n >= 0; n--)
                {
                    UnityEngine.UI.Button btn = Items.Last.Value;
                    btn.GetComponent<ItemCell>().TowerSet(Manager.Managers.Instance.GameManager.OwnedTowers[(_topIndex * 5) + n]);
                    btn.GetComponentInChildren<TextMeshProUGUI>().text = Manager.Managers.Instance.GameManager.OwnedTowers[(_topIndex * 5) + n].TowerData.TowerName;
                    RectTransform rt = btn.GetComponent<RectTransform>();
                    rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, -_rectTransform.anchoredPosition.y);
                    Items.RemoveLast();
                    Items.AddFirst(btn);
                }
            }

        }
        
    }
}

