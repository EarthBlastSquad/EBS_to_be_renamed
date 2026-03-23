using Contents.Tower;
using Data;
using Manager.Core;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils.Defines;

namespace Manager.Contents
{
    public class GameManager
    {
        #region GameData
        public GameData _gameData = new GameData();

        public bool IsLoaded = false;

        public List<Tower> OwnedTowers
        {
            get;
            private set;
            //{
            //    _gameData.OwnedTowers = value;
            //    //갱신이 빈번하게 발생하여 렉 발생, Sorting시 무한루프 발생으로 인하여 주석처리
            //    //EquipInfoChanged?.Invoke();
            //}

        } = new List<Tower>();
        public Tower[] EquippedTowers
        {
            get;
            private set;
        } = new Tower[3];
        public bool SoundSet
        {
            get { return _gameData.SoundSet; }
            set
            {
                _gameData.SoundSet = value;
                SaveGame();
            }
        }
        public float SoundValue
        {
            get { return _gameData.SoundValue; }
            set
            {
                _gameData.SoundValue = value;
                SaveGame();
            }
        }

        private ValueTuple<int, bool> _lastGameEndStatus = ((int)ControlValue.INVALID, false);

        public ValueTuple<int, bool> LastGameEndStatus
        {
            get { return _lastGameEndStatus; }
            set { _lastGameEndStatus = value; }
        }


        public bool TryGetClearData(int stageIdx, out ValueTuple<int, bool> clearData)
        {
            if (_gameData.StageClearData.TryGetValue(stageIdx, out clearData))
            {
                return true;
            }

            return false;
        }

        public void SetClearData(int stageIdx, ValueTuple<int, bool> clearData)
        {
            _gameData.StageClearData[stageIdx] = clearData;
            LastGameEndStatus = clearData;
            SaveGame();
        }

#if UNITY_EDITOR
        private void GetTest()
        {
            OwnedTowers.Clear();
            for (int i = 0; i < 70; i++)
            {
                GetTower(i);
            }

        }
#endif
        public void Init()
        {
            _path = Application.persistentDataPath + "/SaveData.json";
#if UNITY_EDITOR
            //GetTest(); //나중에 떼야됨
#endif
            if (LoadGame() == false)
            {
                return;
            }

            //SaveGame(); //Init()인 만큼 없을때 생성하게 하는 의도도 있음.

        }
        private void TowerFetch()
        {
            foreach (Tower t in OwnedTowers)
            {
#if UNITY_EDITOR
                Debug.Log(t.TowerData.TowerName);
#endif
                if (Managers.Instance.DataManager.TowerDic.TryGetValue(t.TowerData.TowerId, out TowerData td) == true)
                {
#if UNITY_EDITOR
                    Debug.Log("패치");
#endif
                    var tFields = t.TowerData.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    var tProps = t.TowerData.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                    var dFields = td.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    var dProps = td.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                    foreach (var tf in tFields)
                    {
                        foreach (var df in dFields)
                        {
                            if (tf.Name == df.Name && tf.FieldType == df.FieldType)
                            {
                                tf.SetValue(t.TowerData, df.GetValue(td));
                                for (int i = 0; i < EquippedTowers.Count(); i++)
                                {
                                    if (EquippedTowers[i].key == t.key)
                                    {
                                        EquippedTowers[i] = t;
                                    }
                                }
                                break;
                            }
                        }
                    }
                    foreach (var tp in tProps)
                    {
                        if (tp.CanWrite)
                        {
                            foreach (var dp in dProps)
                            {

                                if (dp.CanRead && tp.Name == dp.Name && tp.PropertyType == dp.PropertyType)
                                {
                                    tp.SetValue(t.TowerData, dp.GetValue(td));
                                    for (int i = 0; i < EquippedTowers.Count(); i++)
                                    {
                                        if (EquippedTowers[i].key == t.key)
                                        {
                                            EquippedTowers[i] = t;
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
        #region Save,Load
        private string _path; //SaveData.Json은 Addressables로 관리하는게 아님
        public void SaveGame()
        {
            //_gameData.Currency = Managers.Instance.CurrencyManager.GetCurrency;
            File.WriteAllText(_path, JsonConvert.SerializeObject(_gameData, Formatting.Indented));
        }
        public bool LoadGame()
        {
            if (IsLoaded == true)
            {
                return false;
            }
            if (File.Exists(_path) == false)
            {
                OwnedTowers.Clear();
                _gameData.OwnedTowers.Clear();
                for (int i = 1; i < 6; i++)
                {
                    GetTower(i);
                }
                EquippedTowers = new Tower[3] { new Tower(1), new Tower(2), new Tower(3) };
                EquippedTowers[0].Slot = 0;
                EquippedTowers[1].Slot = 1;
                EquippedTowers[2].Slot = 2;
                SaveGame();
            }
            _gameData = JsonConvert.DeserializeObject<GameData>(File.ReadAllText(_path)); //게임 진행도 데이터 불러오기
            //Manager.Managers.Instance.CurrencyManager.RestoreCurrency(_gameData.Currency, typeof(GameManager));
            //TowerFetch();
            if (OwnedTowers.Count() < Managers.Instance.DataManager.TowerDic.Count())
            {
#if UNITY_EDITOR
                Debug.Log("패치");
#endif
                OwnedTowers.Clear();
                _gameData.OwnedTowers.Clear();
                for (int i = 1; i < 6; i++)
                {
                    GetTower(i);
                }
                if (EquippedTowers.Count() < 3)
                {
                    EquippedTowers = new Tower[3];
                    for (int i = 0; i < 3; i++)
                    {
                        EquippedTowers[i] = new Tower(-1);
                        EquippedTowers[i].Slot = (sbyte)i;
                    }
                    _gameData.EquippedTowers = new int[3] { -1, -1, -1 };
                }
            }
#if UNITY_EDITOR
            Debug.Log("불러왔다");
            Debug.Log(OwnedTowers[0]);
            Debug.Log(Manager.Managers.Instance.CurrencyManager.GetCurrency);
#endif
            for (int i = 0; i < 3; i++)
            {
                if (_gameData.EquippedTowers[i] != -1)
                {

                    EquipTower((sbyte)i, OwnedTowers[i]);

                }
            } //보유 타워 중 장착되었나? 로 체크하는 구조

            IsLoaded = true;
            SaveGame();
            return true;
        }
        #endregion
        #region Inventory
        public bool EquipTower(sbyte index, Tower t)
        {
            if (index == -1 || t == null || EquippedTowers.Contains(t) == true)
            {
                return false;
            }
            //슬롯에 저장
            EquippedTowers[index] = t;
            EquippedTowers[index].Slot = index;
            _gameData.EquippedTowers[index] = t.TowerData.TowerId;
            t.IsEquipped = true;
            SaveGame();
            return true;
        }
        public void UnEquipItem(Tower equipment)
        {
            if (EquippedTowers[equipment.Slot] == equipment)
            {
                equipment.IsEquipped = false;
                EquippedTowers[equipment.Slot] = default(Tower);
                _gameData.EquippedTowers[equipment.Slot] = EquippedTowers[equipment.Slot].TowerData.TowerId;
                equipment.Slot = -1;
                equipment.IsEquipped = false;
            }
            SaveGame();
            //장비 해제 관련 이벤트 추가할거 있으면 말하고
        }
        public void GetTower(int towerID = -1) //enum을 안 쓰고 하길래 그대로 일단 구조는 따라함, 딱히 갓챠나 그런건 우선순위에 없어서 void로 해둠
        {
            if (towerID == -1)
            {
                return;
            }
            Tower t = new Tower(towerID);
            if (t.TowerData is null)
            {
                return;
            }
            OwnedTowers.Add(t);
            _gameData.OwnedTowers.Add(towerID);
            SaveGame();
        }
        #endregion
        //public bool LoadGame()
        //{
        //    if (PlayerPrefs.GetInt("ISFIRST", 1) == 1)
        //    {
        //        string path = Application.persistentDataPath + "/SaveData.json";
        //        if (File.Exists(path))
        //            File.Delete(path);
        //        return false;
        //    }

        //    if (File.Exists(_path) == false)
        //        return false;

        //    string fileStr = File.ReadAllText(_path);
        //    GameData data = JsonConvert.DeserializeObject<GameData>(fileStr);
        //    if (data != null)
        //        _gameData = data;

        //    EquippedEquipments = new Dictionary<EquipmentType, Equipment>();
        //    for (int i = 0; i < OwnedEquipments.Count; i++)
        //    {
        //        if (OwnedEquipments[i].IsEquipped)
        //        {
        //            EquipItem(OwnedEquipments[i].EquipmentData.EquipmentType, OwnedEquipments[i]);
        //        }
        //    }
        //    IsLoaded = true;
        //    return true;
        //} //얘처럼 json 파싱 만들기 겸+Init 최상단 return용도 flase으로 연계
        #endregion

        #region GameStatus
        public bool IsGamePaused { get; set; } = false;
        public bool IsDragging { get; set; } = false;

        public Dictionary<int, int> TotalTowerDamages=new Dictionary<int, int>(); //id,totalDamage
        public Dictionary<int, int> TotalMonsterDamages = new Dictionary<int, int>();
        public void GameStart()
        {
            IsGamePaused = false;
            IsDragging = false;
            TotalTowerDamages = new Dictionary<int, int>();
            TotalMonsterDamages = new Dictionary<int, int>();
            foreach (Tower tower in EquippedTowers)
            {
                TotalTowerDamages[tower.TowerData.TowerId] = 0;
            }

            Dictionary<int, WaveData> dm = Managers.Instance.DataManager.WaveDic;
            WaveData waveData = dm[Managers.Instance.StageManager.GetNowStageData().WaveIdx];
            HashSet<int> visits = new HashSet<int>();
            do
            {
                visits.Add(waveData.WaveIdx);
                foreach (int id in waveData.MobIDs)
                {
                    TotalMonsterDamages[id] = 0;
                }
            } while (waveData.NextWaveIdx != (int)ControlValue.INVALID && dm.TryGetValue(waveData.NextWaveIdx, out waveData) == true && visits.Contains(waveData.WaveIdx) == false);

        }

#endregion
    }
}