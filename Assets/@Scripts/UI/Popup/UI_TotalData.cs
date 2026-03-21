using Manager;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;
namespace UI.Popup
{
    public class UI_TotalData :UIPopup
    {

        enum Buttons
        {
            TotalAttackDamages_0,
            TotalTakeDamages_0
        }
        private RectTransform[] _TowerBars = new RectTransform[3];
        private List<GameObject> _towerDatas=new List<GameObject>();
        private List<GameObject> _monsterDatas = new List<GameObject>();
        private Transform _scrollrect;
        public override bool Init()
        {
            if (base.Init() == false)
                return false;
            BindButton(typeof(Buttons));

            Dictionary<int, int> tmd = Managers.Instance.GameManager.TotalMonsterDamages;
            Dictionary<int, int> ttd = Managers.Instance.GameManager.TotalTowerDamages;
            GameObject getUI = Managers.Instance.ResourceManager.Load<GameObject>("UI_TotalData");
            int towerCount = ttd.Count;
            float towerSlotScale = 1000f / (towerCount - 1);
            float towerSlotScale2 = 2f / (towerCount - 1);
            float towerLeft = 500f;
            int maxTowerDamage = 1;
            foreach (int tid in ttd.Values)
            {
                if (maxTowerDamage < tid)
                {
                    maxTowerDamage = tid;
                }
            }
            foreach (var tid in ttd)
            {
                GameObject ui = Instantiate(getUI, transform, false);
                float x = towerLeft;
                towerLeft += towerSlotScale;
                RectTransform rect = ui.GetComponent<RectTransform>();
                rect.position += new Vector3(x, 0, 0);
                rect.localScale *= towerSlotScale2;
                UnityEngine.UI.Image uiImage = ui.GetComponent<UnityEngine.UI.Image>();
                uiImage.sprite = Managers.Instance.ResourceManager.Load<Sprite>(Managers.Instance.DataManager.TowerDic[tid.Key].TowerImgName);
                RectTransform bar = ui.GetChild<RectTransform>("Bar_0");
                bar.sizeDelta = new Vector2(100, 450 * tid.Value / maxTowerDamage);
                TextMeshProUGUI text = bar.GetComponentInChildren<TextMeshProUGUI>();
                text.text = tid.Value.ToString();
                _towerDatas.Add(ui);
                ui.SetActive(false);
            }
            int monsterCount = tmd.Count;
            if(monsterCount > 3)
            {
                monsterCount = 3;
            }
            float monsterSlotScale = 1000f / (monsterCount - 1);
            float monsterSlotScale2 = 2f / (monsterCount - 1);
            float monsterLeft = 300f;
            int maxMonsterDamage = 1;
            foreach (int m in tmd.Values)
            {
                if (maxMonsterDamage < m)
                {
                    maxMonsterDamage = m;
                }
            }
            _scrollrect = gameObject.GetChild<ScrollRect>("ScrollTotal").transform;
            Transform scrolltransform = _scrollrect.GetChild(0).GetChild(0);
            foreach (var mid in tmd)
            {
                GameObject ui = Instantiate(getUI, scrolltransform, false);
                float x = monsterLeft;
                monsterLeft += monsterSlotScale;
                RectTransform rect = ui.GetComponent<RectTransform>();
                rect.position += new Vector3(x, 0, 0);
                rect.localScale *= monsterSlotScale2;
                UnityEngine.UI.Image uiImage = ui.GetComponent<UnityEngine.UI.Image>();
                uiImage.sprite = Managers.Instance.ResourceManager.Load<Sprite>(Managers.Instance.DataManager.MonsterDic[mid.Key].MonsterImgName);
                RectTransform bar = ui.GetChild<RectTransform>("Bar_0");
                bar.sizeDelta = new Vector2(100, 450*mid.Value / maxMonsterDamage);
                TextMeshProUGUI text = bar.GetComponentInChildren<TextMeshProUGUI>();
                text.text = mid.Value.ToString();
                _monsterDatas.Add(ui);
                //ui.SetActive(false);
            }

            RectTransform content = scrolltransform.GetComponent<RectTransform>();
            float width = Mathf.Max(monsterLeft+300f, 1000f);
            content.sizeDelta = new Vector2(width, content.sizeDelta.y);
            GetButton((int)Buttons.TotalTakeDamages_0).gameObject.BindUIEvent(TotalTakeDamages);
            GetButton((int)Buttons.TotalAttackDamages_0).gameObject.BindUIEvent(TotalAttackDamages);
            TotalAttackDamages(default(PointerEventData));
            return true;
        }
        protected void TotalAttackDamages(PointerEventData _)
        {
            for(int i=0;i<_towerDatas.Count;i++)
            {
                _towerDatas[i].SetActive(true);
            }
            _scrollrect.gameObject.SetActive(false);
        }
        protected void TotalTakeDamages(PointerEventData _)
        {
            for (int i = 0; i < _towerDatas.Count; i++)
            {
                _towerDatas[i].SetActive(false);
            }
            _scrollrect.gameObject.SetActive(true);
        }
    }
}

