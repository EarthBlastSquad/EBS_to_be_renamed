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
        public float InvincibilityTime;
        public string TowerName;
        public string PrefabName;
        public string TowerImgName;
        public int TowerHP;
        public int DemendedCurrency;
        public string TowerDescription;
        public string HitSound;
        public int SkillId;
        //각종 파라미터들
        public TowerData(TowerData original)
        {
            this.TowerId = original.TowerId;
            this.InvincibilityTime = original.InvincibilityTime;
            this.TowerName = original.TowerName;
            this.TowerImgName = original.TowerImgName;
            this.PrefabName = original.PrefabName;
            this.TowerDescription = original.TowerDescription;
            this.DemendedCurrency = original.DemendedCurrency;
            this.TowerHP = original.TowerHP;
            this.HitSound = original.HitSound;
            this.SkillId = original.SkillId;
        }
        public TowerData() { }
    }
    [Serializable]
    public class MonsterData
    {
        public int MonsterId;
        public float InvincibilityTime;
        public float MonsterSpeed;
        public int MonsterHP;
        public string PrefabName;
        public string MonsterImgName;
        public int RewardCurrency;
        public string HitSound;
        public int SkillID;
        //각종 파라미터들
        public MonsterData(MonsterData original)
        {
            this.MonsterId = original.MonsterId;
            this.InvincibilityTime = original.InvincibilityTime;
            this.MonsterSpeed = original.MonsterSpeed;
            this.MonsterHP = original.MonsterHP;
            this.PrefabName = original.PrefabName;
            this.MonsterImgName = original.MonsterImgName;
            this.RewardCurrency = original.RewardCurrency;
            this.HitSound = original.HitSound;
            this.SkillID = original.SkillID;
        }
        public MonsterData() { }
    }

    [Serializable]
    public class WaveData
    {
        public int WaveIdx;
        public int WaveNumber;
        public int WaveTimeLimit;
        public int MobSpawnRate;
        public int NextWaveIdx;
        public List<int> MobIDs;

        //각종 파라미터들
        public WaveData(WaveData original)
        {
            WaveTimeLimit = original.WaveTimeLimit;
            WaveNumber = original.WaveNumber;
            WaveIdx = original.WaveIdx;
            MobIDs = original.MobIDs;
            MobSpawnRate = original.MobSpawnRate;
            NextWaveIdx = original.NextWaveIdx;
        }
        public WaveData() { }
    }

    [Serializable]
    public class AreaUnlockData
    {
        public int IDX;
        public int DemendedCurrency;
        public int UnlockXSize;
        public int NextUnlockData;

        public AreaUnlockData(AreaUnlockData original)
        {
            this.IDX = original.IDX;
            this.DemendedCurrency = original.DemendedCurrency;
            this.UnlockXSize = original.UnlockXSize;
            this.NextUnlockData = original.NextUnlockData;
        }

        public AreaUnlockData() { }
    }

    [Serializable]
    public class StageData
    {
        public int StageIdx;
        public int PrevIdx;
        public int NextIdx;
        public int WaveIdx;
        public int AreaUnlockIdx;
        public int TotalWaveCnt;
        public string StageName;
        public string StageDescription;

        public StageData(StageData original)
        {
            StageIdx = original.StageIdx;
            PrevIdx = original.PrevIdx;
            NextIdx = original.NextIdx;
            WaveIdx = original.WaveIdx;
            AreaUnlockIdx = original.AreaUnlockIdx;
            TotalWaveCnt = original.TotalWaveCnt;
            StageName = original.StageName;
            StageDescription = original.StageDescription;
        }

        public StageData() { }
    }

    [Serializable]
    public class SkillData
    {
        public int SkillID;
        public List<Vector2Int> AttackPos;
        public int Damage;
        public float Cooldown;
        public List<int> AttackableLayers;
        public bool CanAttackMultiple;
        public float SpeedPerCell;
        public string FiringSFX;
        public string SFXName;
        public string AttackEffectName;
        public string AttackObjectImgName;
        public string PrefabName;
        public string SkillDescription;

        public SkillData(SkillData original)
        {
            this.SkillID = original.SkillID;
            this.AttackPos = original.AttackPos;
            this.Damage = original.Damage;
            this.Cooldown = original.Cooldown;
            this.AttackableLayers = original.AttackableLayers;
            this.CanAttackMultiple = original.CanAttackMultiple;
            this.SpeedPerCell = original.SpeedPerCell;
            this.FiringSFX = original.FiringSFX;
            this.SFXName = original.SFXName;
            this.AttackEffectName = original.AttackEffectName;
            this.AttackObjectImgName = original.AttackObjectImgName;
            this.PrefabName = original.PrefabName;
            this.SkillDescription = original.SkillDescription;
        }

        public SkillData() { }
    }
}