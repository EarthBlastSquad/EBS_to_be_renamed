using UnityEngine;
using System.Collections.Generic;
using System;
using Contents.Tower;
namespace Data
{
    [Serializable]
    public class GameData //순수하게 데이터를 들기만 하는 클래스
    {
        //public string UserName = "Player";
        public List<Tower> OwnedTowers = new List<Tower>(); //어떤걸 갖고 있는지만 저장할 용도로서 타워 자체를 담아두는것보단,그냥 ID랑 Name 등 수치만 있는거 그대로 갖다 써본것.
        //public Dictionary<EquipmentType, Equipment> EquippedEquipments = new Dictionary<EquipmentType, Equipment>(); //이제 Slot에 들어간 것은 타워로 해서 게임씬에서 쓸 수 있게 해주기, 근데 꼭 딕셔너리로 해야되나? 어차피 slot_0,1,2 인데?
        public Tower[] EquippedTowers=new Tower[3]; //그냥 배열로 들자, 왜 배열이냐? 개수제한인 3칸만큼 딱 되니까
        #region 사운드

        public bool SoundSet = true;
        public float SoundValue = 15;

        #endregion
    }
}
