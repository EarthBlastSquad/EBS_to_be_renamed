using System;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    //여기에 데이터 클래스하고 로더 구현
    [Serializable]
    public class CharacterData
    {
        public int CharacterId;
        //public Define.PlayerCharacterType Type;
        //public Define.PlayerCharacterAttackType AttackType;
        //public float BaseHp;
        //public float BaseAttack;
        //public float BaseDefense;
        //public float BaseMoveSpeed;
        public List<OrbData> Orbs;
        public string SpriteName;

        public CharacterData(CharacterData original)
        {
            this.CharacterId = original.CharacterId;
            //this.Type = original.Type;
            //this.AttackType = original.AttackType;
            //this.BaseHp = original.BaseHp;
            //this.BaseAttack = original.BaseAttack;
            //this.BaseDefense = original.BaseDefense;
            //this.BaseMoveSpeed = original.BaseMoveSpeed;
            this.SpriteName = original.SpriteName;

            // Orbs도 깊은 복사
            this.Orbs = new List<OrbData>();
            foreach (var orb in original.Orbs)
            {
                this.Orbs.Add(new OrbData(orb)); // OrbData도 복사 생성자 필요
            }
        }
        public CharacterData() { }
    }

    [Serializable]
    public class MonsterData
    {
        public int MonsterId;
        public string MonsterName;
        public float MaxHp;
        public float AttackDamage;
        public float MoveSpeed;
        public int ExpReward;
        public int GoldReward;
        public List<int> DropItemIds;
        public string SpriteName;

    }

    [Serializable]
    public class OrbData
    {
        public int ItemId;
        //public string ItemName;
        //public string Description;
        //public float CoolTime;
        //public Define.OrbType Type;  // 또는 int Type
        //public Define.PlayerCharacterType AttackType;  // 또는 int AttackType
        public string SpriteName;

        // 복사 생성자 추가
        public OrbData(OrbData original)
        {
            this.ItemId = original.ItemId;
            //this.ItemName = original.ItemName;
            //this.Description = original.Description;
            //this.CoolTime = original.CoolTime;
            //this.Type = original.Type;
            //this.AttackType = original.AttackType;
            //this.SpriteName = original.SpriteName;
        }

        // 기본 생성자 (JSON 파싱용)
        public OrbData() { }
    }

    [Serializable]
    public class SkillData
    {
        public int SkillId;
        public string SkillName;
        public float Damage;
        public float CoolTime;
        public float Duration;
        public int ManaCost;
        public string SpriteName;
    }
}