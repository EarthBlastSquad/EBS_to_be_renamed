using Contents.Tower;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
namespace Data
{
    [Serializable]
    [Preserve]
    public class GameData //순수하게 데이터를 들기만 하는 클래스
    {
        //public string UserName = "Player";
        public List<int> OwnedTowers = new List<int>(); //어떤걸 갖고 있는지만 저장할 용도로서 타워 자체를 담아두는것보단,그냥 ID랑 Name 등 수치만 있는거 그대로 갖다 써본것.
        //public Dictionary<EquipmentType, Equipment> EquippedEquipments = new Dictionary<EquipmentType, Equipment>(); //이제 Slot에 들어간 것은 타워로 해서 게임씬에서 쓸 수 있게 해주기, 근데 꼭 딕셔너리로 해야되나? 어차피 slot_0,1,2 인데?
        public int[] EquippedTowers= new int[3]; //그냥 배열로 들자, 왜 배열이냐? 개수제한인 3칸만큼 딱 되니까

        public List<StageClearData> StageClearData = new List<StageClearData>();

        public int LastStageIdx = 0;
        #region 사운드

        public bool SoundSet = true;
        public float SoundValue = 15;

        #endregion

        #region 재화
        //public int Currency=100;
        #endregion
    }
    [Serializable]
    public class StageClearData
    {
        public int StageId;
        public int Wave;
        public bool Cleared;
        public StageClearData(int id,int wave,bool cleared)
        {
            StageId = id;
            Wave = wave;
            Cleared = cleared;
        }
    }
}
