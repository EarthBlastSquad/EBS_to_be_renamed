using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
namespace Data
{
    //여기에 데이터 클래스하고 로더 구현

    //[Serializable]
    //public class OrbData
    //{
    //    public int ItemId;
    //    //public string ItemName;
    //    //public string Description;
    //    //public float CoolTime;
    //    //public Define.OrbType Type;  // 또는 int Type
    //    //public Define.PlayerCharacterType AttackType;  // 또는 int AttackType
    //    public string SpriteName;

    //    // 복사 생성자 추가
    //    public OrbData(OrbData original)
    //    {
    //        this.ItemId = original.ItemId;
    //        //this.ItemName = original.ItemName;
    //        //this.Description = original.Description;
    //        //this.CoolTime = original.CoolTime;
    //        //this.Type = original.Type;
    //        //this.AttackType = original.AttackType;
    //        //this.SpriteName = original.SpriteName;
    //    }

    //    // 기본 생성자 (JSON 파싱용)
    //    public OrbData() { }
    //}//세팅 예제

    [Serializable]
    public class TowerData
    {
        public int TowerId;
        public string TowerName;
        //각종 파라미터들
        public TowerData(TowerData original)
        {
            this.TowerId = original.TowerId;
            this.TowerName= original.TowerName;
        }
        public TowerData() { }
    }
    [Serializable]
    public class MonsterData
    {
        public int MonsterId;
        public int MonsterSpeed;
        public int MonsterHP;
        public string PrefabName;
        //각종 파라미터들
        public MonsterData(MonsterData original)
        {
            this.MonsterId = original.MonsterId;
            this.MonsterSpeed = original.MonsterSpeed;
            this.MonsterHP = original.MonsterHP;
            this.PrefabName = original.PrefabName;
        }
        public MonsterData() { }
    }

    [Serializable]
    public class WaveData
    {
        public int WaveTimeLimit;
        public int WaveIdx;
        public int MobSpawnRate;
        public int AreaUnlockXSize;
        public List<int> MobIDs;

        //각종 파라미터들
        public WaveData(WaveData original)
        {
            WaveTimeLimit = original.WaveTimeLimit;
            WaveIdx = original.WaveIdx;
            MobIDs = original.MobIDs;
            MobSpawnRate = original.MobSpawnRate;
            AreaUnlockXSize = original.AreaUnlockXSize;
        }
        public WaveData() { }
    }
}