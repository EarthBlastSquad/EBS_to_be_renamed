using Contents.Tower;
using Data;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Manager.Contents
{
    public class GameManager
    {
        #region GameData
        public GameData _gameData = new GameData();

        public bool IsLoaded = false;
        
        public List<Tower> OwnedTowers
        {
            get { return _gameData.OwnedTowers; }
            set
            {
                _gameData.OwnedTowers = value;
                //갱신이 빈번하게 발생하여 렉 발생, Sorting시 무한루프 발생으로 인하여 주석처리
                //EquipInfoChanged?.Invoke();
            }
        }
        public Tower[] EquippedTowers
        {
            get { return _gameData.EquippedTowers; }
            set
            {
                _gameData.EquippedTowers = value;
            }
        }
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

#if UNITY_EDITOR
        private void GetTest()
        {
            GetTower(1);
            GetTower(2);
            GetTower(3);
        }
#endif
        public void Init()
        {
            _path = Application.persistentDataPath + "/SaveData.json";
#if UNITY_EDITOR
            //GetTest(); //나중에 떼야됨
#endif
            if (LoadGame()==false)
            {
                return;
            }

            //SaveGame(); //Init()인 만큼 없을때 생성하게 하는 의도도 있음.

        }
        #region Save,Load
        private string _path; //SaveData.Json은 Addressables로 관리하는게 아님
        public void SaveGame()
        {
            File.WriteAllText(_path, JsonConvert.SerializeObject(_gameData, Formatting.Indented));
        }
        public bool LoadGame()
        {
            if(IsLoaded==true)
            {
                return false;
            }
            if (File.Exists(_path)==false)
            {
                GetTest();
                SaveGame();
            }
            _gameData = JsonConvert.DeserializeObject<GameData>(File.ReadAllText(_path)); //게임 진행도 데이터 불러오기
#if UNITY_EDITOR
            Debug.Log("불러왔다");
            Debug.Log(OwnedTowers[0]);
#endif
            for (int i = 0; i < OwnedTowers.Count; i++)
            {
                if (OwnedTowers[i].IsEquipped==true)
                {
                    EquipTower(OwnedTowers[i].Slot, OwnedTowers[i]);
                }
            } //보유 타워 중 장착되었나? 로 체크하는 구조

            IsLoaded = true;
            return true;
        }
        #endregion
        #region Inventory
        public void EquipTower(sbyte index = -1, Tower t=null)
        {
            if(index==-1 || t==null)
            {
                return;
            }
            //슬롯에 저장
            EquippedTowers[index] = t;
            SaveGame();
        }
        public void UnEquipItem(Tower equipment)
        {
            if (EquippedTowers[equipment.Slot] == equipment)
            {
                equipment.IsEquipped = false;
                EquippedTowers[equipment.Slot]=default(Tower);
                equipment.Slot = -1;

            }
            SaveGame();
            //장비 해제 관련 이벤트 추가할거 있으면 말하고
        }
        public void GetTower(int towerID=-1) //enum을 안 쓰고 하길래 그대로 일단 구조는 따라함, 딱히 갓챠나 그런건 우선순위에 없어서 void로 해둠
        {
            if (towerID==-1)
            {
                return;
            }
            Tower t = new Tower(towerID);
            OwnedTowers.Add(t);
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
    }
}